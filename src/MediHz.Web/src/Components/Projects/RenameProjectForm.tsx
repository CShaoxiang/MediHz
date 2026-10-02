import Button from '../Common/Button'
import TextInput from '../Common/TextInput'
import type { Project } from '../../Types/projects'
import type { OmitId } from '../../Types/helpers'

interface IProps {
  project?: Project
  setProject?: (project: Project) => void
  onSubmit: (project: Project) => void
}

export default function RenameProjectForm({ project, setProject, onSubmit }: IProps) {

  if (!project || !setProject) {
    return <div>Project not found</div>
  }

  const validateProject = (project: OmitId<Project>): boolean => {
    return project.name.trim() !== ''
  }

  const handleSubmit = async () => {
    if (!validateProject(project)) {
      alert('Invalid project details')
      return
    }
    onSubmit(project)
  }

  return (
    <div className="flex flex-col gap-2">
      <h2>Rename Project</h2>
      <TextInput
        type="text"
        placeholder="Name"
        value={project.name}
        onChange={(e) => setProject({ ...project, name: e.target.value })}
      />
      <Button label="Rename" onClick={handleSubmit} />
    </div>
  )
}
