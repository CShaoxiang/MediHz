import { useState, type FormEvent } from 'react'
import { Button } from '../Shadcn/button'
import { Field, FieldError, FieldGroup, FieldLabel } from '../Shadcn/field'
import { Input } from '../Shadcn/input'
import LabelGroupSelect from './LabelGroupSelect'
import type { Label, LabelGroup } from '../../Types/labels'
import { createLabelInGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'

interface IProps {
  groupId: string | null
  labelGroups: LabelGroup[]
  onSubmit: (label: Label) => void
}

export default function CreateLabelForm({ groupId, labelGroups, onSubmit }: IProps) {
  const [name, setName] = useState('')
  const [targetGroupId, setTargetGroupId] = useState<string | null>(groupId ?? null)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (name.trim() === '') {
      setError('Label name is required')
      return
    }

    createLabelInGroup(targetGroupId, name)
      .then((result) => {
        if (result.ok) {
          onSubmit(result.value)
          setName('')
        } else showApiError(result.error)
      })
      .catch((e) => console.error(e))
  }

  return (
    <form onSubmit={handleSubmit}>
      <FieldGroup>
        <Field data-invalid={!!error}>
          <FieldLabel htmlFor="label-name">Label name</FieldLabel>
          <Input
            id="label-name"
            placeholder="Label name"
            value={name}
            aria-invalid={!!error}
            onChange={(e) => {
              setName(e.target.value)
              setError(null)
            }}
          />
          <FieldError>{error}</FieldError>
        </Field>
        <Field>
          <FieldLabel htmlFor="label-group">Label group</FieldLabel>
          <LabelGroupSelect
            id="label-group"
            groups={labelGroups}
            value={targetGroupId}
            onValueChange={setTargetGroupId}
          />
        </Field>
        <Button type="submit">Add</Button>
      </FieldGroup>
    </form>
  )
}
