import { useEffect, useState } from 'react'
import Card from '../Components/Common/Card'
import Modal from '../Components/Common/Modal'
import CreateProjectForm from '../Components/Projects/CreateProjectForm'
import { deleteProject, fetchProjects, renameProject } from '../Api/projects'
import type { Project } from '../Types/projects'
import { useNavigate } from 'react-router-dom'
import Grid from '../Components/Common/Grid'
import { LuBookAudio, LuBookPlus } from 'react-icons/lu'
import RenameProjectForm from '../Components/Projects/RenameProjectForm'
import DeleteProjectForm from '../Components/Projects/DeleteProjectForm'
import { exportProject } from '../Api/exports'
import { downloadJsonFile } from '../Utilities/download'

export default function ProjectsListingPage() {
  const [isOpen, setIsOpen] = useState(false)
  const [isOpenRename, setIsOpenRename] = useState(false)
  const [isOpenDelete, setIsOpenDelete] = useState(false)

  const [projects, setProjects] = useState<Project[]>([])
  const [selectedProject, setSelectedProject] = useState<Project>()
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

  const handleClickProject = (project: Project) => {
    navigate(`/projects/${project.id}`, { state: { projectName: project.name } })
  }

  const projectActions = (project: Project) => [
    {
      label: 'Rename',
      onClick: () => {
        setSelectedProject(project)
        setIsOpenRename(true)
      },
    },
    {
      label: 'Export',
      onClick: () => {
        exportProject(project.id).then((res) => {
          if (res.ok) {
            downloadJsonFile(res.value, `${project.name || 'project'}_export.json`)
          } else {
            console.error(res.error)
            alert(res.error?.detail || 'Failed to export project.')
          }
        })
      },
    },
    {
      label: 'Delete',
      onClick: () => {
        setSelectedProject(project)
        setIsOpenDelete(true)
      },
      classname: 'text-red font-bold',
    },
  ]

  const handleRename = (project: Project) => {
    renameProject(project.id, project.name).then((res) => {
      if (res.ok) {
        setProjects((existingProjects) =>
          existingProjects.map((p) => (p.id === project.id ? project : p)),
        )
      } else {
        console.error(res.error)
      }
    })

    setIsOpenRename(false)
  }

  const handleDelete = (project: Project) => {
    deleteProject(project.id).then((res) => {
      if (res.ok) {
        setProjects((existingProjects) => existingProjects.filter((p) => p.id !== project.id))
      } else {
        console.error(res.error)
      }
    })

    setIsOpenDelete(false)
  }
  return (
    <>
      <Grid>
        <Card title={'Create Project'} onClick={() => setIsOpen(true)}>
          <LuBookPlus className="icon-small" />
        </Card>
        {projects.map((project, i) => (
          <Card
            key={i}
            title={project.name}
            onClick={() => handleClickProject(project)}
            actions={projectActions(project)}
          >
            {' '}
            <LuBookAudio className="icon-small" />{' '}
          </Card>
        ))}
      </Grid>
      <Modal modalHeader={''} state={isOpen} setState={setIsOpen}>
        <CreateProjectForm onSubmit={handleOnSubmit} />
      </Modal>
      <Modal modalHeader={''} state={isOpenRename} setState={setIsOpenRename}>
        <RenameProjectForm
          onSubmit={handleRename}
          project={selectedProject}
          setProject={setSelectedProject}
        />
      </Modal>
      <Modal modalHeader={''} state={isOpenDelete} setState={setIsOpenDelete}>
        <DeleteProjectForm onSubmit={handleDelete} project={selectedProject} />
      </Modal>
    </>
  )
}
