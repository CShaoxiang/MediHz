import { useState, type FormEvent } from 'react'
import { Button } from '../Shadcn/button'
import { DialogFooter, DialogHeader, DialogTitle } from '../Shadcn/dialog'
import { Field, FieldError, FieldLabel } from '../Shadcn/field'
import { Input } from '../Shadcn/input'
import type { Project } from '../../Types/projects'
import { createProject } from '../../Api/projects'

interface IProps {
  onSubmit: (project: Project) => void
}

export default function CreateProjectForm({ onSubmit }: IProps) {
  const [name, setName] = useState('')
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (name.trim() === '') {
      setError('Project name is required')
      return
    }

    createProject({ name })
      .then((result) => {
        if (result.ok) onSubmit(result.value)
        else console.error(result.error)
      })
      .catch((e) => console.error(e))
  }

  return (
    <form onSubmit={handleSubmit} className="contents">
      <DialogHeader>
        <DialogTitle>New Project</DialogTitle>
      </DialogHeader>
      <Field data-invalid={!!error}>
        <FieldLabel htmlFor="project-name">Project name</FieldLabel>
        <Input
          id="project-name"
          placeholder="Name"
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
