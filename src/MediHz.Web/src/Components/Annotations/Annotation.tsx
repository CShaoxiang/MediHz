import { useState } from 'react'
import Button from '../Common/Button'
import ListGroup from '../Common/ListGroup'
import type { Region } from 'wavesurfer.js/dist/plugins/regions.js'
import type { RegionData } from '../../Interfaces/RegionData'
import { FiX } from 'react-icons/fi'
import type { HubConnection } from '@microsoft/signalr'
import { sendData } from '../../Api/hub'

interface AnnotationProp {
  region: Region
  selectedregionData: RegionData | undefined
  setRegionData: React.Dispatch<React.SetStateAction<Map<string, RegionData>>>
  regionTags: Set<string>
  setRegionTags: React.Dispatch<React.SetStateAction<Set<string>>>
  connection?: HubConnection
}

export default function Annotations({
  region,
  selectedregionData,
  setRegionData,
  regionTags,
  setRegionTags,
  connection,
}: AnnotationProp) {
  const selectedRegion = selectedregionData
  const [description, setDescription] = useState(selectedRegion?.description || '...')
  const [customTag, setCustomTag] = useState<string>('')
  const [prevSelectedRegion, setPrevSelectedRegion] = useState(selectedRegion)

  const sendRegionData = () => {
    if (selectedRegion && connection) {
      sendData(connection, 'test audio', region.id)
    }
  }

  const handleSave = () => {
    setRegionData((currentRegionData) => {
      const nextRegionData = new Map(currentRegionData)
      nextRegionData.set(region.id, {
        ...selectedRegion,
        description: description,
        tags: regionTags,
      })
      return nextRegionData
    })

    sendRegionData()
  }

  const delRegionTag = (tag: string) => {
    const updatedTags = new Set(regionTags)
    updatedTags.delete(tag)
    setRegionTags(updatedTags)
  }

  const addRegionTag = (Tag: string) => {
    setRegionTags((currentTags) => {
      const nextTags = new Set(currentTags)
      nextTags.add(Tag)
      return nextTags
    })
  }

  if (selectedRegion !== prevSelectedRegion) {
    setPrevSelectedRegion(selectedRegion)
    if (selectedRegion) {
      setRegionTags(selectedRegion.tags || new Set())
      setDescription(selectedRegion.description || '')
    }
  }

  if (!selectedRegion) {
    return null
  }

  return (
    <div className="max-h-full min-h-0 max-w-full overflow-hidden rounded-xl bg-purple-700 p-4 font-medium text-white">
      <span className="flex min-h-0 gap-2">
        <section>
          <h5>Start Time: {selectedRegion.startTime?.toFixed(2)} </h5>{' '}
          <h5>End Time: {selectedRegion.endTime?.toFixed(2)}</h5>
          <div className="relative m-1 flex h-full min-h-0 flex-col overflow-hidden rounded-xl bg-white/20 p-2 font-normal">
            <ListGroup
              heading={'Tags' + (regionTags.size > 0 ? ` (${regionTags.size})` : '')}
              items={Array.from(regionTags)}
              onSelect={(item) => delRegionTag(item)}
              hoverable={<FiX className="icon-small size-6" />}
            />
            <input
              className="mt-2 w-full shrink-0 rounded-xl border-2 border-dotted bg-purple-700/20 p-1"
              type="text"
              value={customTag}
              onChange={(event) => setCustomTag(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === 'Enter') {
                  addRegionTag(customTag)
                }
              }}
              placeholder="Add custom tag"
            />
          </div>
        </section>
        <div>
          <section className="h-auto bg-white/20 rounded-xl p-1">
            Description:
            <ul className="p-1">
              <textarea
                value={description}
                onChange={(event) => setDescription(event.target.value)}
                placeholder="Enter description"
              />
            </ul>
          </section>
        </div>
      </span>
      <footer className="flex justify-end align-center mt-4 items-center">
        <span className="mr-4">
          {selectedRegion?.description !== description || selectedRegion?.tags !== regionTags ? (
            <span>Unsaved</span>
          ) : null}
        </span>
        <Button label="SAVE" border={true} onClick={handleSave} />
      </footer>
    </div>
  )
}
