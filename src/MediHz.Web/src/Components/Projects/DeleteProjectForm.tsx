import Button from '../Common/Button'
import type { Project } from '../../Types/projects'

interface IProps {
  project?: Project
  onSubmit: (project: Project) => void
}

export default function DeleteProjectForm({ project, onSubmit }: IProps) {
  const handleSubmit = async () => {
    if (project) {
      onSubmit(project)
    }
  }

  return (
    <div className="flex flex-col gap-2">
      <h2>Delete Project</h2>
      <p>Do you want to delete {project?.name}?</p>
      <Button label="Delete" onClick={handleSubmit} />
    </div>
  )
}
