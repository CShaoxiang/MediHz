import { useEffect, useId, useRef, useState } from 'react'
import WaveSurfer from 'wavesurfer.js'
import RegionsPlugin, { type Region } from 'wavesurfer.js/dist/plugins/regions.js'
import Spectrogram from 'wavesurfer.js/dist/plugins/spectrogram.js'
import { Button } from '../Shadcn/button'
import { FiMusic, FiPause, FiPlay } from 'react-icons/fi'
import { Slider } from '../Shadcn/slider'
import { Toggle } from '../Shadcn/toggle'
import { ButtonGroup } from '../Shadcn/button-group'
import { Popover, PopoverContent, PopoverTitle, PopoverTrigger } from '../Shadcn/popover'
import { Tooltip, TooltipContent, TooltipTrigger } from '../Shadcn/tooltip'
import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'

const NEUTRAL_REGION_COLOR = 'var(--region)'
const SELECTED_REGION_COLOR = 'var(--region-selected)'

const sliderValue = (value: number | readonly number[]) =>
  typeof value === 'number' ? value : value[0]

const themeColor = (name: string) =>
  getComputedStyle(document.documentElement).getPropertyValue(name)

const NEUTRAL_REGION_SPECTROGRAM_COLOR = 'var(--spectrogram-region)'
const SELECTED_REGION_SPECTROGRAM_COLOR = 'var(--spectrogram-region-selected)'

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
  const [audioDuration, setAudioDuration] = useState<number>(0)
  const zoom = useRef(0)

  // Zoom Controls
  const applyZoom = (next: number) => {
    zoom.current = Math.min(Math.max(next, 0), 1000)
    waveSurferInstance.current?.zoom(zoom.current)
  }

  const zoomIn = () => applyZoom(zoom.current + 100)
  const zoomOut = () => applyZoom(zoom.current - 100)
  const resetZoom = () => applyZoom(0)

  //Intalises Audio Waveform configs
  useEffect(() => {
    if (!audio || !waveformRef.current) return

    const wavesurfer = WaveSurfer.create({
      container: waveformRef.current,
      waveColor: themeColor('--waveform'),
      progressColor: themeColor('--waveform-progress'),
      barWidth: 2,
      barRadius: 10,
      height: 200,
      cursorWidth: 2,
      cursorColor: themeColor('--foreground'),
      normalize: true,
      hideScrollbar: true,
      autoScroll: false,
    })

    const centerOnPlayhead = () => {
      const duration = wavesurfer.getDuration()
      if (!duration) return
      const playheadX =
        (wavesurfer.getCurrentTime() / duration) * wavesurfer.getWrapper().scrollWidth
      wavesurfer.setScroll(playheadX - wavesurfer.getWidth() / 2)
    }

    const regionsPlugin = wavesurfer.registerPlugin(RegionsPlugin.create())

    wavesurfer.on('ready', (duration) => {
      setAudioDuration(duration)
      setRegions(regionsPlugin)
    })

    wavesurfer.on('timeupdate', (time) => {
      setPlaybar(time)
      centerOnPlayhead()
    })
    wavesurfer.on('zoom', centerOnPlayhead)
    wavesurfer.on('redrawcomplete', centerOnPlayhead)

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

  // React's onWheel is passive, so preventDefault only works on a listener added directly
  useEffect(() => {
    const container = waveformRef.current
    if (!regions || !container) return

    const handleWheel = (event: WheelEvent) => {
      event.preventDefault()
      applyZoom(zoom.current + (event.deltaY < 0 ? 25 : -25))
    }

    container.addEventListener('wheel', handleWheel, { passive: false })
    return () => container.removeEventListener('wheel', handleWheel)
  }, [regions])

  // Spectrogram and waveform use the same regions plugin and selection colors.
  useEffect(() => {
    const wave = waveSurferInstance.current
    if (!wave || !regions) return
    if (spectro) {
      regionNeturalColor.current = NEUTRAL_REGION_SPECTROGRAM_COLOR
      regionSelectedColor.current = SELECTED_REGION_SPECTROGRAM_COLOR
      wave.setOptions({ height: 0 })
      if (!spectrogramInstance.current) {
        spectrogramInstance.current = wave.registerPlugin(Spectrogram.create())
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

  const changeVolume = (newVolume: number) => {
    setVolume(newVolume)
    waveSurferInstance.current?.setVolume(newVolume / 100)
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
  const playbarLabelId = useId()
  const volumeLabelId = useId()

  const changePlaybar = (time: number) => {
    waveSurferInstance.current?.setTime(time)
    setPlaybar(time)
  }
  return (
    <div className="space-y-4">
      <div ref={waveformRef} className="w-full bg-secondary rounded-3xl shadow overflow-hidden" />

      <div className="flex items-center gap-3">
        <span id={playbarLabelId} className="sr-only">
          Audio playback position
        </span>
        <Slider
          aria-labelledby={playbarLabelId}
          max={audioDuration}
          step={0.001}
          value={[playbar]}
          onValueChange={(value) => changePlaybar(sliderValue(value))}
        />

        <span className="shrink-0 text-sm text-muted-foreground tabular-nums">
          {formatTime(playbar)} / {formatTime(audioDuration)}
        </span>
      </div>

      <div className="flex gap-2 items-center">
        <Tooltip>
          <TooltipTrigger
            render={
              <Button
                size="icon"
                aria-label={play ? 'Pause audio' : 'Play audio'}
                onClick={playPause}
              />
            }
          >
            {play ? <FiPause aria-hidden="true" /> : <FiPlay aria-hidden="true" />}
          </TooltipTrigger>
          <TooltipContent>{play ? 'Pause' : 'Play'}</TooltipContent>
        </Tooltip>

        <Popover>
          <Tooltip>
            <TooltipTrigger
              render={
                <PopoverTrigger
                  render={<Button variant="outline" size="icon" aria-label="Volume" />}
                />
              }
            >
              <FiMusic aria-hidden="true" />
            </TooltipTrigger>
            <TooltipContent>Volume</TooltipContent>
          </Tooltip>
          <PopoverContent className="w-48">
            <PopoverTitle id={volumeLabelId}>Volume</PopoverTitle>
            <Slider
              aria-labelledby={volumeLabelId}
              value={[volume]}
              onValueChange={(value) => changeVolume(sliderValue(value))}
            />
          </PopoverContent>
        </Popover>

        {canEdit && (
          <>
            <Toggle variant="outline" pressed={highlight} onPressedChange={setHighlight}>
              Highlight
            </Toggle>
            <Toggle variant="outline" pressed={editing} onPressedChange={setEditing}>
              Editing
            </Toggle>
            <Button variant="destructive" onClick={handleDelete}>
              Delete
            </Button>
          </>
        )}
        <ButtonGroup aria-label="Zoom">
          <Button variant="outline" onClick={zoomOut}>
            Zoom Out
          </Button>
          <Button variant="outline" onClick={resetZoom}>
            Reset
          </Button>
          <Button variant="outline" onClick={zoomIn}>
            Zoom In
          </Button>
        </ButtonGroup>
        <Toggle variant="outline" pressed={spectro} onPressedChange={setSpectro}>
          Spectrogram
        </Toggle>
      </div>
    </div>
  )
}
