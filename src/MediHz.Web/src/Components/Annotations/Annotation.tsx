import { useState, type FormEvent } from 'react'
import { FiX } from 'react-icons/fi'
import { showApiError } from '../../Api/helpers'
import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'
import type { ApiResult } from '../../Types/api'
import { Button } from '../Shadcn/button'
import { Input } from '../Shadcn/input'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '../Shadcn/card'
import { Badge } from '../Shadcn/badge'
import { Spinner } from '../Shadcn/spinner'

interface AnnotationProp {
  annotation: AnnotationRegion
  selectedTimestamp: { start: number; end: number } | null
  
  canEdit: boolean
  onChange: (startTime: number, endTime: number) => void
  onSave: (tags: Set<string>) => Promise<ApiResult<null>>
}

export default function Annotations({ annotation,selectedTimestamp, canEdit, onChange, onSave }: AnnotationProp) {
  const [customTag, setCustomTag] = useState('')
  const [draftTags, setDraftTags] = useState<Set<string> | null>(null)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const tags = draftTags ?? annotation.tags

  const [timestamp, setTimestamp] = useState<{start: number, end: number}>({start: annotation.startTime, end: annotation.endTime})

  const editTags = (edit: (tags: Set<string>) => void) => {
    if (!canEdit) return
    const nextTags = new Set(tags)
    edit(nextTags)
    setDraftTags(nextTags)
    setError('')
  }

  const changeinTimestamp = !(selectedTimestamp?.start == annotation.startTime && selectedTimestamp.end == annotation.endTime)

  const addCustomTag = (event: FormEvent) => {
    event.preventDefault()
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
    onChange(annotation.startTime, annotation.endTime)
    if (result.ok) {
      setCustomTag('')
      setDraftTags(null)
    } else showApiError(result.error)
  }

  return (
    <Card className="max-h-full min-h-0" aria-label="Annotation details">
      <CardHeader>
        <CardTitle>Annotation</CardTitle>
        <CardDescription>
          <p>
            Start Time: {annotation.startTime.toFixed(2)} · End Time:{' '}
            {annotation.endTime.toFixed(2)}
          </p>
          {annotation.authorDisplayName && <p>Annotated by: {annotation.authorDisplayName}</p>}
        </CardDescription>
      </CardHeader>
      <CardContent className="flex min-h-0 flex-1 flex-col gap-2">
        <h3 className="font-medium">Tags{tags.size > 0 && ` (${tags.size})`}</h3>
        {tags.size === 0 ? (
          <p className="text-muted-foreground">No tags yet</p>
        ) : (
          <div className="flex min-h-0 flex-wrap gap-1.5 overflow-y-auto">
            {Array.from(tags).map((tag) =>
              canEdit ? (
                <Badge
                  key={tag}
                  variant="secondary"
                  render={
                    <button
                      type="button"
                      aria-label={`Remove tag ${tag}`}
                      onClick={() => editTags((next) => next.delete(tag))}
                    />
                  }
                >
                  {tag}
                  <FiX data-icon="inline-end" />
                </Badge>
              ) : (
                <Badge key={tag} variant="secondary">
                  {tag}
                </Badge>
              ),
            )}
          </div>
        )}
        {canEdit && (
          <form onSubmit={addCustomTag} className="shrink-0">
            <Input
              aria-label="Add custom tag"
              placeholder="Add custom tag"
              value={customTag}
              onChange={(event) => {
                setCustomTag(event.target.value)
                setError('')
              }}
            />
          </form>
        )}
        {error && (
          <p role="alert" className="text-destructive">
            {error}
          </p>
        )}
      </CardContent>
      {canEdit && (
        <CardFooter className="justify-end gap-4">
          {(!annotation.saved||changeinTimestamp || draftTags) && <span className="text-muted-foreground">Unsaved</span>}
          <Button disabled={saving} onClick={handleSave}>
            {saving && <Spinner data-icon="inline-start" />}
            {saving ? 'Saving' : 'Save'}
          </Button>
        </CardFooter>
      )}
    </Card>
    )
}
