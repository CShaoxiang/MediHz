import { useEffect, useRef, useState, type ChangeEvent } from 'react'
import WaveSurfer from 'wavesurfer.js'
import RegionsPlugin, { type Region } from 'wavesurfer.js/dist/plugins/regions.js'
import Spectrogram from 'wavesurfer.js/dist/plugins/spectrogram.js'
import Button from '../Common/Button'
import { FiMusic, FiPause, FiPlay } from 'react-icons/fi'
import Slider from '../Common/Slider'
import Playbar from './Playbar'
import HoverInfo from '../Common/HoverInfo'
import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'

const NEUTRAL_REGION_COLOR = 'rgba(70, 115, 51, 0.2)'
const SELECTED_REGION_COLOR = 'rgba(131, 0, 219, 0.2)'

const NEUTRAL_REGION_SPECTROGRAM_COLOR = 'rgba(51, 239, 230, 0.4)'
const SELECTED_REGION_SPECTROGRAM_COLOR = 'rgba(76, 51, 239, 0.4)'

interface WaveformProp {
  canEdit?: boolean
  audio?: string
  annotations: Map<string, AnnotationRegion>
  selectedId: string | null
  onSelect: (id: string) => void
  onDraw: (id: string, startTime: number, endTime: number) => void
  onMove: (id: string, startTime: number, endTime: number) => void
  onDelete: (id: string) => void
}

export default function WaveformRender({
  canEdit = false,
  audio,
  annotations,
  selectedId,
  onSelect,
  onDraw,
  onMove,
  onDelete,
}: WaveformProp) {
  // Waveform rendering
  const waveformRef = useRef<HTMLDivElement | null>(null)
  const spectrogramContainer = useRef<HTMLDivElement | null>(null)
  const waveSurferInstance = useRef<WaveSurfer | null>(null)

  // Region Plugin Rendering
  const regionInstance = useRef<RegionsPlugin>(null)
  const spectrogramInstance = useRef<Spectrogram>(null)

  const regionNeturalColor = useRef<string>(NEUTRAL_REGION_COLOR)
  const regionSelectedColor = useRef<string>(SELECTED_REGION_COLOR)
  const [regions, setRegions] = useState<RegionsPlugin | null>(null)
  const syncingRegions = useRef(false)

  // Waveform interactions
  const [spectro, setSpectro] = useState(false)
  const [playbar, setPlaybar] = useState(0)
  const [highlight, setHighlight] = useState(false)
  const [editing, setEditing] = useState(false)
  const [play, setPlay] = useState(false)
  const [volSlide, setVolSlide] = useState(false)
  const [audioDuration, setAudioDuration] = useState<number>(0)
  const [zoom, setZoom] = useState(0)

  // Zoom Controls
  const zoomIn = () => {
    const newZoom = Math.min(zoom + 100, 1000)
    setZoom(newZoom)
    waveSurferInstance.current?.zoom(newZoom)
  }

  const zoomOut = () => {
    const newZoom = Math.max(zoom - 100, 0)
    setZoom(newZoom)
    waveSurferInstance.current?.zoom(newZoom)
  }

  const resetZoom = () => {
    setZoom(0)
    waveSurferInstance.current?.zoom(0)
  }

  //Intalises Audio Waveform configs
  useEffect(() => {
    if (!audio || !waveformRef.current) return

    const wavesurfer = WaveSurfer.create({
      container: waveformRef.current,
      waveColor: '#8300db7e',
      progressColor: '#f5076a',
      barWidth: 2,
      barRadius: 10,
      height: 200,
      cursorWidth: 2,
      cursorColor: '#000000',
      normalize: true,
    })

    const regionsPlugin = wavesurfer.registerPlugin(RegionsPlugin.create())

    wavesurfer.on('ready', (duration) => {
      setAudioDuration(duration)
      setRegions(regionsPlugin)
    })

    wavesurfer.on('timeupdate', (time) => {
      setPlaybar(time)
    })

    wavesurfer.load(audio).catch((e) => {
      if (e.name !== 'AbortError') {
        console.error('Error loading wavesurfer:', e)
      }
    })
    waveSurferInstance.current = wavesurfer
    regionInstance.current = regionsPlugin

    return () => {
      //Memory Cleanup
      setRegions(null)
      waveSurferInstance.current = null
      wavesurfer.destroy()
    }
  }, [audio])

  const validateRegion = (region: Region) => {
    if (region.end - region.start < 0.5) {
      region.setOptions({ end: region.start + 0.5 })
    }
  }

  const playRegion = (region: Region, e: MouseEvent) => {
    e.stopPropagation()
    region.play(true)
  }
  useEffect(() => {
    if (!regions) return

    const unsubscribes = [
      regions.on('region-created', (region) => {
        if (syncingRegions.current) return
        if (!canEdit) {
          region.remove()
          return
        }
        // Drawn by the user: give it the ID the server will also use
        region.setOptions({ id: crypto.randomUUID() })
        validateRegion(region)
        onDraw(region.id, region.start, region.end)
      }),
      regions.on('region-clicked', (region, e) => {
        onSelect(region.id)
        playRegion(region, e)
      }),
      // Tracks drags as they happen so a sync mid-drag doesn't snap the region back
      regions.on('region-update', (region) => {
        if (!canEdit) return
        validateRegion(region)
        onMove(region.id, region.start, region.end)
      }),
      regions.on('region-updated', (region) => onSelect(region.id)),
    ]

    return () => unsubscribes.forEach((unsubscribe) => unsubscribe())
  }, [regions, canEdit, onSelect, onDraw, onMove])

  // Make the waveform's regions match the annotations
  useEffect(() => {
    if (!regions) return

    const existing = new Map(regions.getRegions().map((region) => [region.id, region]))
    existing.forEach((region, id) => {
      if (!annotations.has(id)) region.remove()
    })

    annotations.forEach((annotation, id) => {
      const options = {
        start: annotation.startTime,
        end: annotation.endTime,
        color: id === selectedId ? regionSelectedColor.current : regionNeturalColor.current,
        drag: canEdit && editing,
        resize: canEdit && editing,
      }

      const region = existing.get(id)
      if (region) {
        region.setOptions(options)
        return
      }

      syncingRegions.current = true
      regions.addRegion({ id, ...options })
      syncingRegions.current = false
    })
  }, [regions, annotations, selectedId, editing, canEdit])

  const handleWheel = (delta: number) => {
    console.log('Wheel event detected. Delta:', delta)
    if (delta < 0) {
      const newZoom = Math.min(zoom + 25, 1000)
      setZoom(newZoom)
      waveSurferInstance.current?.zoom(newZoom)
    } else {
      const newZoom = Math.max(zoom - 25, 0)
      setZoom(newZoom)
      waveSurferInstance.current?.zoom(newZoom)
    }
  }

  // Spectrogram and waveform use the same regions plugin and selection colors.
  useEffect(() => {
    const wave = waveSurferInstance.current
    if (!wave || !regions) return
    if (spectro && spectrogramContainer.current) {
      regionNeturalColor.current = NEUTRAL_REGION_SPECTROGRAM_COLOR
      regionSelectedColor.current = SELECTED_REGION_SPECTROGRAM_COLOR
      wave.setOptions({ height: 0 })
      if (!spectrogramInstance.current) {
        spectrogramInstance.current = wave.registerPlugin(
          Spectrogram.create({ container: spectrogramContainer.current }),
        )
      }
    } else {
      regionNeturalColor.current = NEUTRAL_REGION_COLOR
      regionSelectedColor.current = SELECTED_REGION_COLOR
      wave.setOptions({ height: 200 })
      spectrogramInstance.current?.destroy()
      spectrogramInstance.current = null
    }
    regions.getRegions().forEach((region) => {
      region.setOptions({
        color: region.id === selectedId ? regionSelectedColor.current : regionNeturalColor.current,
      })
    })
  }, [spectro, regions, selectedId])

  useEffect(() => {
    if (!regions || !canEdit || !highlight) return
    return regions.enableDragSelection({ color: regionNeturalColor.current })
  }, [regions, canEdit, highlight])

  const playPause = () => {
    setPlay(!play)
    waveSurferInstance.current?.playPause()
  }

  const changeVolume = (event: ChangeEvent<HTMLInputElement>) => {
    const newVolume = Number(event.target.value)
    setVolume(newVolume)
    waveSurferInstance.current?.setVolume(newVolume / 100)
  }

  const handleHighlight = () => {
    setHighlight(!highlight)
  }

  const handleEditing = () => {
    setEditing(!editing)
  }

  const handleDelete = () => {
    if (!canEdit) return
    if (selectedId) onDelete(selectedId)
  }

  const formatTime = (time: number) => {
    const minutes = Math.floor(time / 60)
    const seconds = Math.floor(time % 60)
    return `${minutes}:${seconds.toString().padStart(2, '0')}`
  }

  const [volume, setVolume] = useState(100)

  const changePlaybar = (time: number) => {
    waveSurferInstance.current?.setTime(time)
    setPlaybar(time)
  }
  return (
    <div className="space-y-4">
      <div
        ref={waveformRef}
        className="w-full bg-violet-100 rounded-3xl shadow overflow-hidden"
        onWheel={(e) => {
          e.preventDefault()
          handleWheel(e.deltaY)
        }}
      />
      <div
        ref={spectrogramContainer}
        className={spectro ? 'w-full overflow-hidden rounded-3xl' : 'hidden'}
      />

      <div className="-ml-1 -mr-1 info-box-parent">
        <Playbar
          audioDuration={audioDuration}
          state={playbar}
          onChange={(e) => changePlaybar(Number(e.target.value))}
        />

        <HoverInfo>
          {formatTime(playbar)} / {formatTime(audioDuration || 0)}
        </HoverInfo>
      </div>

      <div className="flex gap-2 items-center">
        <Button ariaLabel={play ? 'Pause audio' : 'Play audio'} onClick={playPause}>
          {play ? (
            <FiPause className="icon-small size-7" aria-hidden="true" />
          ) : (
            <FiPlay className="icon-small size-7" aria-hidden="true" />
          )}
        </Button>

        <Button
          ariaLabel={volSlide ? 'Hide volume control' : 'Show volume control'}
          onClick={() => setVolSlide(!volSlide)}
        >
          <FiMusic className="icon-small size-7" aria-hidden="true" />
        </Button>

        {volSlide && (
          <div className="max-w-20 mt-2">
            <Slider min={0} max={100} state={volume} onChange={changeVolume} ariaLabel="Volume" />
          </div>
        )}

        {canEdit && (
          <>
            <Button
              label={highlight ? 'Disable Highlight' : 'Highlight'}
              onClick={handleHighlight}
            />
            <Button label={editing ? 'Disable Editing' : 'Editing'} onClick={handleEditing} />
            <Button label="Delete" onClick={handleDelete} />
          </>
        )}
        <Button label="Zoom In" onClick={zoomIn} />
        <Button label="Zoom Out" onClick={zoomOut} />
        <Button label="Reset Zoom" onClick={resetZoom} />
        <Button label="Toggle Spectrogram" onClick={() => setSpectro(!spectro)} />
      </div>
    </div>
  )
}
