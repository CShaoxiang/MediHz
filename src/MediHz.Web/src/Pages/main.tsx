import { useEffect, useState } from 'react'
import SideBar from '../Components/Common/SideBar'
import WaveformRender from '../Components/Waveform/WaveformRender'
import type { Region } from 'wavesurfer.js/dist/plugins/regions.js'
import Annotations from '../Components/Annotations/Annotation'
import type { RegionData } from '../Interfaces/RegionData'
import { hubConnection } from '../Api/hub'
import { HubConnection } from '@microsoft/signalr'

export default function MainPage() {
  const [selectedRegion, setSelectedRegion] = useState<Region | null>(null)
  const [regionData, setRegionData] = useState<Map<string, RegionData>>(new Map())
  const [regionTags, setRegionTags] = useState<Set<string>>(new Set())

  const [connection, setConnection] = useState<HubConnection>()

  const addRegionTag = (Tag: string) => {
    setRegionTags((currentTags) => {
      const nextTags = new Set(currentTags)
      nextTags.add(Tag)
      return nextTags
    })
  }

  useEffect(() => {
    if (!connection) {
      async function init() {
        const connection = await hubConnection()
        setConnection(connection)
      }

      init()
    }
  }, [connection])

  return (
    <div className="flex items-start gap-4">
      <div className="flex items-start gap-4 mr-4">
        <aside className="flex flex-col gap-4">
          <SideBar
            title="Audio Files"
            buttons={[
              { label: 'Audio 1', onClick: () => {} },
              { label: 'Audio 2', onClick: () => {} },
            ]}
          />

          <SideBar
            title="Annotations"
            lists={[
              {
                heading: 'Existing Tags',
                items: ['Inhale', 'Exhale', 'Wheeze', 'Crackle'],
                onSelect: addRegionTag,
              },
            ]}
          />
        </aside>
      </div>
      <div className="w-full">
        <WaveformRender
          audio="src\TestAudio\Test.mp3"
          audioId="Test Audio"
          setSelectedRegion={setSelectedRegion}
          setRegionData={setRegionData}
          connection={connection}
        />
        {selectedRegion && (
          <div className="m-2 flex min-h-0 max-h-[calc(100svh-12rem)] overflow-hidden">
            <Annotations
              audioId="Test Audio"
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
    </div>
  )
}
