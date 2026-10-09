import { useState } from 'react'
import { FiX } from 'react-icons/fi'
import { showApiError } from '../../Api/helpers'
import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'
import type { ApiResult } from '../../Types/api'
import Button from '../Common/Button'
import ListGroup from '../Common/ListGroup'

interface AnnotationProp {
  annotation: AnnotationRegion
  canEdit: boolean
  onSave: (tags: Set<string>) => Promise<ApiResult<null>>
}

export default function Annotations({ annotation, canEdit, onSave }: AnnotationProp) {
  const [customTag, setCustomTag] = useState('')
  const [draftTags, setDraftTags] = useState<Set<string> | null>(null)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const tags = draftTags ?? annotation.tags

  const editTags = (edit: (tags: Set<string>) => void) => {
    if (!canEdit) return
    const nextTags = new Set(tags)
    edit(nextTags)
    setDraftTags(nextTags)
    setError('')
  }

  const addCustomTag = () => {
    const tag = customTag.trim()
    if (!tag) return
    editTags((next) => next.add(tag))
    setCustomTag('')
  }

  const handleSave = async () => {
    if (!canEdit || saving) return
    const nextTags = new Set(tags)
    const pendingTag = customTag.trim()
    if (pendingTag) nextTags.add(pendingTag)
    if (nextTags.size === 0) {
      setError('Add at least one tag before saving.')
      return
    }
    setError('')
    setSaving(true)
    const result = await onSave(nextTags)
    setSaving(false)
    if (result.ok) {
      setCustomTag('')
      setDraftTags(null)
    } else showApiError(result.error)
  }

  return (
    <section
      className="min-h-0 max-w-full rounded-xl bg-purple-700 p-4 font-medium text-white"
      aria-label="Annotation details"
    >
      <div className="mb-4 flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p>Start Time: {annotation.startTime.toFixed(2)}</p>
          <p>End Time: {annotation.endTime.toFixed(2)}</p>
          {annotation.authorDisplayName && (
            <p className="mt-1 text-sm">Annotated by: {annotation.authorDisplayName}</p>
          )}
        </div>
        {canEdit && (
          <div className="flex shrink-0 items-center self-start">
            <span className="mr-2">{draftTags ? 'Unsaved' : null}</span>
            <Button label={saving ? 'SAVING...' : 'SAVE'} border={true} onClick={handleSave} />
          </div>
        )}
      </div>

      <div className="relative m-1 flex h-max min-h-0 flex-col rounded-xl bg-white/20 p-2 font-normal">
        <ListGroup
          heading={'Tags' + (tags.size > 0 ? ` (${tags.size})` : '')}
          items={Array.from(tags)}
          onSelect={canEdit ? (item) => editTags((next) => next.delete(item)) : undefined}
          hoverable={canEdit ? <FiX className="icon-small size-6" /> : undefined}
        />
        {canEdit && (
          <input
            className="mt-2 w-full shrink-0 rounded-xl border-2 border-dotted bg-purple-700/20 p-1"
            type="text"
            value={customTag}
            onChange={(event) => {
              setCustomTag(event.target.value)
              setError('')
            }}
            onKeyDown={(event) => {
              if (event.key === 'Enter') addCustomTag()
            }}
            placeholder="Add custom tag"
            aria-label="Add custom tag"
          />
        )}
        {error && <p role="alert">{error}</p>}
      </div>
    </section>
  )
}
