import { useState } from 'react'
import Button from '../Common/Button'
import ListGroup from '../Common/ListGroup'
import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'
import type { ApiResult } from '../../Types/api'
import { showApiError } from '../../Api/helpers'
import { FiX } from 'react-icons/fi'

interface AnnotationProp {
  annotation: AnnotationRegion
  onSave: (tags: Set<string>) => Promise<ApiResult<null>>
}

export default function Annotations({ annotation, onSave }: AnnotationProp) {
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

  return (
    <div className="max-h-full min-h-0 max-w-full overflow-hidden rounded-xl bg-purple-700 p-4 font-medium text-white">
      <span className="flex min-h-0 gap-2">
        <section>
          <h5>Start Time: {annotation.startTime.toFixed(2)} </h5>{' '}
          <h5>End Time: {annotation.endTime.toFixed(2)}</h5>
          <div className="relative m-1 flex h-full min-h-0 flex-col overflow-hidden rounded-xl bg-white/20 p-2 font-normal">
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
        </section>
      </span>
      <footer className="flex justify-end align-center mt-4 items-center">
        <span className="mr-4">{draftTags ? <span>Unsaved</span> : null}</span>
        <Button label={saving ? 'SAVING...' : 'SAVE'} border={true} onClick={handleSave} />
      </footer>
    </div>
  )
}
