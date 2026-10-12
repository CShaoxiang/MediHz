import { useState, type FormEvent } from 'react'
import { Button } from '../Shadcn/button'
import { DialogFooter, DialogHeader, DialogTitle } from '../Shadcn/dialog'
import { Field, FieldError, FieldLabel } from '../Shadcn/field'
import { Input } from '../Shadcn/input'
import type { Project } from '../../Types/projects'

interface IProps {
  project?: Project
  setProject?: (project: Project) => void
  onSubmit: (project: Project) => void
}

export default function RenameProjectForm({ project, setProject, onSubmit }: IProps) {
  const [error, setError] = useState<string | null>(null)

  if (!project || !setProject) {
    return <div>Project not found</div>
  }

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (project.name.trim() === '') {
      setError('Project name is required')
      return
    }
    onSubmit(project)
  }

  return (
    <form onSubmit={handleSubmit} className="contents">
      <DialogHeader>
        <DialogTitle>Rename Project</DialogTitle>
      </DialogHeader>
      <Field data-invalid={!!error}>
        <FieldLabel htmlFor="project-rename">Project name</FieldLabel>
        <Input
          id="project-rename"
          placeholder="Name"
          value={project.name}
          aria-invalid={!!error}
          onChange={(e) => {
            setProject({ ...project, name: e.target.value })
            setError(null)
          }}
        />
        <FieldError>{error}</FieldError>
      </Field>
      <DialogFooter>
        <Button type="submit">Rename</Button>
      </DialogFooter>
    </form>
  )
}
