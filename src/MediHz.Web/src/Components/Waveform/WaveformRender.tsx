import { useCallback, useEffect, useRef, useState, type ChangeEvent, type Dispatch } from 'react'
import WaveSurfer from 'wavesurfer.js'
import RegionsPlugin, { type Region } from 'wavesurfer.js/dist/plugins/regions.js'
import Button from '../Common/Button'
import { FiMusic, FiPause, FiPlay } from 'react-icons/fi'
import Slider from '../Common/Slider'
import Playbar from './Playbar'
import HoverInfo from '../Common/HoverInfo'
import type { HubConnection } from '@microsoft/signalr'
import { sendData } from '../../Api/hub'
import type { RegionData } from '../../Interfaces/RegionData'

const NEUTRAL_REGION_COLOR = 'rgba(70, 115, 51, 0.2)'
const SELECTED_REGION_COLOR = 'rgba(131, 0, 219, 0.2)'

interface WaveformProp {
  audio: string
  audioId: string
  setSelectedRegion: Dispatch<React.SetStateAction<Region | null>>
  setRegionData: Dispatch<React.SetStateAction<Map<string, RegionData>>>
  connection?: HubConnection
}

export default function WaveformRender({
  audio,
  audioId,
  setSelectedRegion,
  setRegionData,
  connection,
}: WaveformProp) {
  // Waveform rendering
  const waveformRef = useRef<HTMLDivElement | null>(null)
  const waveSurferInstance = useRef<WaveSurfer | null>(null)
  const selectedRegionRef = useRef<Region | null>(null)

  // Region Plugin Rendering
  const regionInstance = useRef<RegionsPlugin>(null)
  const disableSelection = useRef<(() => void) | null>(null)

  // Waveform interactions
  const [playbar, setPlaybar] = useState(0)
  const [highlight, setHighlight] = useState(false)
  const [editing, setEditing] = useState(false)
  const [play, setPlay] = useState(false)
  const [volSlide, setVolSlide] = useState(false)

  const createRegionData = useCallback(
    (region: Region) => {
      setRegionData((currentRegionData) => {
        const nextRegionData = new Map(currentRegionData)
        nextRegionData.set(region.id, {
          startTime: region.start,
          endTime: region.end,
          tags: new Set<string>(),
          description: '',
        })
        return nextRegionData
      })
    },
    [setRegionData],
  )
  // Update region data when a region is edited
  const updateRegionData = useCallback(
    (region: Region) => {
      setRegionData((currentRegionData) => {
        const nextRegionData = new Map(currentRegionData)
        nextRegionData.set(region.id, {
          ...currentRegionData.get(region.id),
          startTime: region.start,
          endTime: region.end,
        })
        return nextRegionData
      })
    },
    [setRegionData],
  )

  const sendRegionData = useCallback(() => {
    if (selectedRegionRef.current && connection) {
      sendData(connection, audioId , selectedRegionRef.current.id)
    }
  }, [connection])
  const [audioDuration, setAudioDuration] = useState<number>(0)
  const [zoom, setZoom] = useState(20)

  // Zoom Controls
  const zoomIn = () => {
    const newZoom = Math.min(zoom + 100, 1000)
    setZoom(newZoom)
    waveSurferInstance.current?.zoom(newZoom)
  }

  const zoomOut = () => {
    const newZoom = Math.max(zoom - 100, 20)
    setZoom(newZoom)
    waveSurferInstance.current?.zoom(newZoom)
  }

  const resetZoom = () => {
    setZoom(20)
    waveSurferInstance.current?.zoom(20)
  }

  //Intalises Audio Waveform configs
  useEffect(() => {
    if (!waveformRef.current) return

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

    const regions = wavesurfer.registerPlugin(RegionsPlugin.create())
    regionInstance.current = regions

    wavesurfer.on('ready', (duration) => {
      setAudioDuration(duration)
    })

    wavesurfer.on('timeupdate', (time) => {
      setPlaybar(time)
    })  


    wavesurfer.load(audio)
    
    waveSurferInstance.current = wavesurfer

    return () => {
      //Memory Cleanup
      disableSelection.current?.()
      regionInstance.current = null

      waveSurferInstance.current = null
      wavesurfer.destroy()
    }
  }, [audio])

  const syncSelectedRegion = useCallback(
    (nextRegion: Region) => {
      const previousRegion = selectedRegionRef.current

      if (previousRegion && previousRegion !== nextRegion) {
        previousRegion.setOptions({ color: NEUTRAL_REGION_COLOR })
      }

      nextRegion.setOptions({ color: SELECTED_REGION_COLOR })

      selectedRegionRef.current = nextRegion
      setSelectedRegion(nextRegion)
    },
    [setSelectedRegion],
  )

  useEffect(() => {
    if (!regionInstance.current) return

    const handleRegionClicked = (region: Region) => {
      syncSelectedRegion(region)
    }

    const handleRegionUpdated = (region: Region) => {
      syncSelectedRegion(region)
      updateRegionData(region)
      sendRegionData()
    }

    const handleRegionCreated = (region: Region) => {
      syncSelectedRegion(region)
      region.drag = false
      region.resize = false
      createRegionData(region)
      sendRegionData()
    }
    regionInstance.current.on('region-clicked', handleRegionClicked)
    regionInstance.current.on('region-updated', handleRegionUpdated)
    regionInstance.current.on('region-created', handleRegionCreated)

    return () => {
      regionInstance.current?.un('region-clicked', handleRegionClicked)
      regionInstance.current?.un('region-updated', handleRegionUpdated)
      regionInstance.current?.un('region-created', handleRegionCreated)
    }
  }, [connection,audio])

  //Manages region selection
  useEffect(() => {
    if (!regionInstance.current) return

    if (highlight) {
      disableSelection.current = regionInstance.current.enableDragSelection({
        color: NEUTRAL_REGION_COLOR,
      })
      return
    }

    disableSelection.current?.()
    disableSelection.current = null
  }, [highlight])

  // Manages toggled editing
  useEffect(() => {
    if (!regionInstance.current) return

    // Update all existing regions
    regionInstance.current.getRegions().forEach((region) => {
      region.drag = editing
      region.resize = editing
    })
  }, [editing])

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
    if (selectedRegionRef.current) {
      selectedRegionRef.current.remove()
      setSelectedRegion(null)

      setRegionData((currentRegionData) => {
        const nextRegionData = new Map(currentRegionData)
        nextRegionData.delete(selectedRegionRef.current!.id)
        return nextRegionData
      })
    }
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
      <div ref={waveformRef} className="w-full bg-violet-100 rounded-3xl shadow overflow-hidden" />

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
        <Button onClick={playPause}>
          {play ? (
            <FiPause className="icon-small size-7" />
          ) : (
            <FiPlay className="icon-small size-7" />
          )}
        </Button>
        <Button onClick={() => setVolSlide(!volSlide)}>
          <FiMusic className="icon-small size-7" />
        </Button>

        {volSlide && (
          <div className="max-w-20 mt-2">
            <Slider min={0} max={100} state={volume} onChange={changeVolume} />
          </div>
        )}

        <Button label={highlight ? 'Disable Highlight' : 'Highlight'} onClick={handleHighlight} />
        <Button label={editing ? 'Disable Editing' : 'Editing'} onClick={handleEditing} />
        <Button label="Delete" onClick={handleDelete} />
        <Button label="Zoom In" onClick={zoomIn} />
        <Button label="Zoom Out" onClick={zoomOut} />
        <Button label="Reset Zoom" onClick={resetZoom} />
      </div>
    </div>
  )
}
