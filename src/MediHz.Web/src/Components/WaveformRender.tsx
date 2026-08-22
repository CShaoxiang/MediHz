import { useEffect, useRef, useState, type ChangeEvent } from 'react'
import WaveSurfer from 'wavesurfer.js'
import RegionsPlugin from 'wavesurfer.js/dist/plugins/regions.js'
import Button from './Common/Button'
import Slider from './Common/Slider'
import HoverInfo from './Common/HoverInfo'

interface WaveformProp {
  audio: string
}

export default function WaveformRender({ audio }: WaveformProp) {
  const waveformRef = useRef<HTMLDivElement | null>(null)
  const waveSurferInstance = useRef<WaveSurfer | null>(null)

  const regionInstance = useRef<RegionsPlugin>(null)
  const disableSelection = useRef<() => void>(null)

  const [playbar, setPlaybar] = useState(0)
  const [toggleHighlight, setHighlight] = useState(false)
  const [toggleEditing, setEditing] = useState(false)
  const [startTime, setStartTime] = useState<number | null>(null)
  const [endTime, setEndTime] = useState<number | null>(null)

  const audioDuration = useRef<number>(null)

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
      audioDuration.current = duration
    })

    wavesurfer.on('timeupdate', (time) => {
      setPlaybar(time)
    })

    //Tells region data
    regions.on('region-clicked', (region) => {
      setStartTime(region.start)
      setEndTime(region.end)
    })

    //Tells region that was updated
    regions.on('region-updated', (region) => {
      setStartTime(region.start)
      setEndTime(region.end)
    })

    //Set editing on newly created regions based on current editing mode
    regions.on('region-created', (region) => {
      region.drag = false
      region.resize = false

      setStartTime(region.start)
      setEndTime(region.end)
    })

    wavesurfer.load(audio)
    waveSurferInstance.current = wavesurfer

    return () => {
      //Memory Cleanup
      disableSelection.current?.()
      regionInstance.current = null
      wavesurfer.destroy()
      waveSurferInstance.current = null
    }
  }, [])

  //Manages region selection
  useEffect(() => {
    if (!regionInstance.current) return

    if (toggleHighlight) {
      disableSelection.current = regionInstance.current.enableDragSelection({
        color: 'rgba(181, 50, 68, 0.2)',
      })
      return
    }

    disableSelection.current?.()
    disableSelection.current = null
  }, [toggleHighlight])

  // Manages toggled editing
  useEffect(() => {
    if (!regionInstance.current) return

    // Update all existing regions
    regionInstance.current.getRegions().forEach((region) => {
      region.drag = toggleEditing
      region.resize = toggleEditing
    })
  }, [toggleEditing])

  const playPause = () => {
    waveSurferInstance.current?.playPause()
  }

  const changeVolume = (event: ChangeEvent<HTMLInputElement>) => {
    const newVolume = Number(event.target.value)
    setVolume(newVolume)
    waveSurferInstance.current?.setVolume(newVolume / 100)
  }

  const movePlaybar = (event: ChangeEvent<HTMLInputElement>) => {
    const newTime = Number(event.target.value)
    waveSurferInstance.current?.setTime(newTime)
    setPlaybar(newTime)
  }

  const handleHighlight = () => {
    setHighlight(!toggleHighlight)
  }

  const handleEditing = () => {
    setEditing(!toggleEditing)
  }

  const formatTime = (time: number) => {
    const minutes = Math.floor(time / 60)
    const seconds = Math.floor(time % 60)
    return `${minutes}:${seconds.toString().padStart(2, '0')}`
  }
  
  const [volume, setVolume] = useState(100)

  return (
    <div className="space-y-4">
      <div ref={waveformRef} className="w-full bg-violet-100 rounded-3xl shadow overflow-hidden" />

      <>
        <div className="-ml-1 -mr-1 info-box-parent">
          <Slider
            min={0}
            max={audioDuration.current ? audioDuration.current : 0}
            state={playbar}
            onChange={movePlaybar}
          />

          <HoverInfo>
            {formatTime(playbar)} / {formatTime(audioDuration.current || 0)}
          </HoverInfo>
        </div>

        <div className="flex gap-2 items-center">
          <Button
            label={toggleHighlight ? 'Disable Highlight' : 'Highlight'}
            onClick={handleHighlight}
          />

          <Button label={toggleEditing ? 'Disable Editing' : 'Editing'} onClick={handleEditing} />

          <Button label="Play/Pause" onClick={playPause} />

          <div className="flex align-center font-bold">Volume</div>

          <div className="max-w-20 mt-2">
            <Slider min={0} max={100} state={volume} onChange={changeVolume} />
          </div>
        </div>

        {startTime !== null && endTime !== null && (
          <div className="flex gap-4 text-sm">
            <p>Start: {startTime.toFixed(2)}s</p>
            <p>End: {endTime.toFixed(2)}s</p>
          </div>
        )}
      </>
    </div>
  )
}
