import { useState } from 'react'
import Button from '../Common/Button'
import ListGroup from '../Common/ListGroup'
import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'
import type { ApiResult } from '../../Types/api'
import { showApiError } from '../../Api/helpers'
import { FiX } from 'react-icons/fi'

interface AnnotationProp {
  canEdit?: boolean
  audioId: string
  region: Region
  selectedregionData: RegionData | undefined
  setRegionData: React.Dispatch<React.SetStateAction<Map<string, RegionData>>>
  regionTags: Set<string>
  setRegionTags: React.Dispatch<React.SetStateAction<Set<string>>>
  connection?: HubConnection
}

export default function Annotations({
  canEdit = false,
  audioId,
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
  const [draftTags, setDraftTags] = useState<Set<string> | null>(null)
  const [saving, setSaving] = useState(false)

  const tags = draftTags ?? annotation.tags

  const editTags = (edit: (tags: Set<string>) => void) => {
    const nextTags = new Set(tags)
    edit(nextTags)
    setDraftTags(nextTags)
  }

  const addCustomTag = () => {
    const tag = customTag.trim()
    if (!tag) return
    editTags((next) => next.add(tag))
    setCustomTag('')
  }

  const handleSave = async () => {
    if (saving) return

    setSaving(true)
    const result = await onSave(tags)
    setSaving(false)

    if (result.ok) {
      setDraftTags(null)
    } else {
      showApiError(result.error)
    }
  }

  const handleSave = () => {
    if (!canEdit) return
    const nextRegionData = {
      ...selectedRegion,
      description: description,
      tags: regionTags,
    }

    setRegionData((currentRegionData) => {
      const updatedMap = new Map(currentRegionData)
      updatedMap.set(region.id, nextRegionData)
      return updatedMap
    })

    sendRegionData(nextRegionData)
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

  if (!canEdit)
    return (
      <section className="rounded-xl bg-purple-700 p-4 text-white" aria-label="Annotation details">
        <p>
          Start: {selectedRegion.startTime?.toFixed(2)} · End: {selectedRegion.endTime?.toFixed(2)}
        </p>
        <p>Tags: {Array.from(selectedRegion.tags || []).join(', ') || 'None'}</p>
      </section>
    )

  return (
    <div className=" min-h-0 max-w-full rounded-xl bg-purple-700 p-4 font-medium text-white">
      <div className="mb-4 flex items-start justify-between gap-3">
        <section className="min-w-0">
          <h5>Start Time: {annotation.startTime.toFixed(2)} </h5>{' '}
          <h5>End Time: {annotation.endTime.toFixed(2)}</h5>
        </section>
        <div className="flex shrink-0 items-center self-start">
          <span className="mr-2">{draftTags ? <span>Unsaved</span> : null}</span>
          <Button label={saving ? 'SAVING...' : 'SAVE'} border={true} onClick={handleSave} />
        </div>
      </div>

      <div className="relative m-1 flex h-max min-h-0 flex-col overflow rounded-xl bg-white/20 p-2 font-normal">
        <ListGroup
          heading={'Tags' + (tags.size > 0 ? ` (${tags.size})` : '')}
          items={Array.from(tags)}
          onSelect={(item) => editTags((next) => next.delete(item))}
          hoverable={<FiX className="icon-small size-6" />}
        />
        <input
          className="mt-2 w-full shrink-0 rounded-xl border-2 border-dotted bg-purple-700/20 p-1"
          type="text"
          value={customTag}
          onChange={(event) => setCustomTag(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === 'Enter') {
              addCustomTag()
            }
          }}
          placeholder="Add custom tag"
        />
      </div>
    </div>
  )
}
