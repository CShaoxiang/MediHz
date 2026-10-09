import { useState } from 'react'
import Button from '../Common/Button'
import TextInput from '../Common/TextInput'
import type { Project } from '../../Types/projects'
import { createProject } from '../../Api/projects'

interface IProps {
  onSubmit: (project: Project) => void
}

export default function CreateProjectForm({ onSubmit }: IProps) {
  const [project, setProject] = useState<Pick<Project, 'name'>>({
    name: '',
  })

  const validateProject = (project: Pick<Project, 'name'>): boolean => {
    return project.name.trim() !== ''
  }

  const handleSubmit = async () => {
    if (!validateProject(project)) {
      alert('Invalid project details')
      return
    }

    createProject(project)
      .then((result) => {
        if (result.ok) onSubmit(result.value)
        else console.error(result.error)
      })
      .catch((e) => console.error(e))
  }

  return (
    <div className="flex flex-col gap-2">
      <h2>New Project</h2>
      <label htmlFor="project-name" className="text-sm font-medium text-gray-700">
        Project name
      </label>

      <TextInput
        id="project-name"
        type="text"
        placeholder="Name"
        value={project.name}
        onChange={(e) => setProject({ ...project, name: e.target.value })}
      />
      <Button label="Create +" onClick={handleSubmit} />
    </div>
  )
}
