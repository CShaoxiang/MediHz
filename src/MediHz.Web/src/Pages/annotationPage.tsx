import { useParams } from 'react-router-dom'
import WaveformRender from '../Components/Waveform/WaveformRender'
import Annotations from '../Components/Annotations/Annotation'
import { useEffect, useState } from 'react'
import type { Region } from 'wavesurfer.js/dist/plugins/regions.js'
import type { RegionData } from '../Interfaces/RegionData'
import type { HubConnection } from '@microsoft/signalr'
import { getAudioFileUrl } from '../Api/audio'
import { fetchSingleAudio } from '../Api/projects'
import { hubConnection } from '../Api/hub'

type PageParams = {
  projectId: string
  audioId: string
}

export default function AnnotationPage() {
  const { projectId, audioId } = useParams<PageParams>()

  const [selectedRegion, setSelectedRegion] = useState<Region | null>(null)
  const [regionData, setRegionData] = useState<Map<string, RegionData>>(new Map())
  const [regionTags, setRegionTags] = useState<Set<string>>(new Set())

  const [connection, setConnection] = useState<HubConnection>()

  const [audio, setAudio] = useState<string>('src\TestAudio\Test.mp3')
  const [audioName, setAudioName] = useState<string>()

  useEffect(() => {
    if (!connection) {
      async function init() {
        const connection = await hubConnection()
        setConnection(connection)
      }

      init()
    }
  }, [])

  useEffect(() => {
    const fetchAudio = async () => {
      if (projectId && audioId) {
        const res = await fetchSingleAudio(projectId, audioId)

        if (res.ok) {
          setAudioName(res.value.fileName)
        }

        await getAudioFileUrl(projectId, audioId).then((res) => {
          if (res.ok) {
            setAudio(res.value)
          }
        })
      }
    }

    fetchAudio()
  }, [])

  return (
    <div className="w-full">
      <h1 className="text-xl m-1">{audioName}</h1>
      <WaveformRender
        audio={audio}
        audioId={audioId ? audioId : 'Error: No AudioID'}
        setSelectedRegion={setSelectedRegion}
        setRegionData={setRegionData}
        connection={connection}
      />
      {selectedRegion && (
        <div className="m-2 flex min-h-0 max-h-[calc(100svh-12rem)] overflow-hidden">
          <Annotations
            region={selectedRegion}
            selectedregionData={regionData.get(selectedRegion.id)}
            setRegionData={setRegionData}
            regionTags={regionTags}
            setRegionTags={setRegionTags}
            connection={connection}
          />
        </div>
      )}
    </div>
  )
}
