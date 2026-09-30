import { useEffect, useState } from 'react'
import Card from '../Components/Common/Card'
import Modal from '../Components/Common/Modal'
import CreateProjectForm from '../Components/Projects/CreateProjectForm'
import { fetchProjects } from '../Api/projects'
import type { Project } from '../Types/projects'
import { useNavigate } from 'react-router-dom'
import Grid from '../Components/Common/Grid'
import { LuBookAudio, LuBookPlus } from 'react-icons/lu'

export default function ProjectsListingPage() {
  const [isOpen, setIsOpen] = useState(false)
  const [projects, setProjects] = useState<Project[]>([])
  const navigate = useNavigate()

  useEffect(() => {
    fetchProjects()
      .then((result) => {
        if (result.ok) setProjects(result.value)
        else console.error(result.error)
      })
      .catch((e) => console.error(e))
  }, [])

  const handleOnSubmit = (createdProject: Project) => {
    setProjects((existingProjects) => [createdProject, ...existingProjects])
    setIsOpen(false)
  }

  const handleClickProject = (projectId: string) => {
    navigate(`/projects/${projectId}`)
  }

  return (
    <>
      <Grid>
        <Card title={'Create Project'} onClick={() => setIsOpen(true)}>
          <LuBookPlus className="icon-small-white" />
        </Card>
        {projects.map((project, i) => (
          <Card key={i} title={project.name} onClick={() => handleClickProject(project.id)}>
            {' '}
            <LuBookAudio className="icon-small" />{' '}
          </Card>
        ))}
      </Grid>
      <Modal modalHeader={''} state={isOpen} setState={setIsOpen}>
        <CreateProjectForm onSubmit={handleOnSubmit} />
      </Modal>
    </>
  )
}
