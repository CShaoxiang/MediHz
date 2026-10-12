import { useState, type FormEvent } from 'react'
import { Button } from '../Shadcn/button'
import { DialogFooter, DialogHeader, DialogTitle } from '../Shadcn/dialog'
import { Field, FieldError, FieldLabel } from '../Shadcn/field'
import { Input } from '../Shadcn/input'
import type { LabelGroup } from '../../Types/labels'
import { createLabelGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'

interface IProps {
  onSubmit: (labelGroup: LabelGroup) => void
}

export default function CreateLabelGroupForm({ onSubmit }: IProps) {
  const [name, setName] = useState('')
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (name.trim() === '') {
      setError('Group name is required')
      return
    }

    createLabelGroup(name)
      .then((result) => {
        if (result.ok) {
          onSubmit(result.value)
          setName('')
        } else showApiError(result.error)
      })
      .catch((e) => console.error(e))
  }

  return (
    <form onSubmit={handleSubmit} className="contents">
      <DialogHeader>
        <DialogTitle>Create a New Label Group</DialogTitle>
      </DialogHeader>
      <Field data-invalid={!!error}>
        <FieldLabel htmlFor="label-group-name">Group name</FieldLabel>
        <Input
          id="label-group-name"
          placeholder="Group name"
          value={name}
          aria-invalid={!!error}
          onChange={(e) => {
            setName(e.target.value)
            setError(null)
          }}
        />
        <FieldError>{error}</FieldError>
      </Field>
      <DialogFooter>
        <Button type="submit">Create</Button>
      </DialogFooter>
    </form>
  )
}
