import { toast } from 'sonner'
import { useEffect, useState } from 'react'
import ItemCard from '../Components/Common/ItemCard'
import { Dialog, DialogContent } from '../Components/Shadcn/dialog'
import CreateProjectForm from '../Components/Projects/CreateProjectForm'
import { deleteProject, fetchProjects, renameProject } from '../Api/projects'
import type { Project } from '../Types/projects'
import type { MenuAction } from '../Types/actions'
import { useNavigate } from 'react-router-dom'
import Grid from '../Components/Common/Grid'
import { LuBookAudio, LuPlus } from 'react-icons/lu'
import { Button } from '../Components/Shadcn/button'
import PageHeader from '../Components/Common/PageHeader'
import EmptyState from '../Components/Common/EmptyState'
import RenameProjectForm from '../Components/Projects/RenameProjectForm'
import { exportProject } from '../Api/exports'
import { downloadJsonFile } from '../Utilities/download'
import ConfirmDeleteDialog from '../Components/Common/ConfirmDeleteDialog'

export default function ProjectsListingPage() {
  const [isOpen, setIsOpen] = useState(false)
  const [isOpenRename, setIsOpenRename] = useState(false)
  const [isOpenDelete, setIsOpenDelete] = useState(false)

  const [projects, setProjects] = useState<Project[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [selectedProject, setSelectedProject] = useState<Project>()
  const navigate = useNavigate()

  useEffect(() => {
    fetchProjects()
      .then((result) => {
        if (result.ok) setProjects(result.value)
        else console.error(result.error)
      })
      .catch((e) => console.error(e))
      .finally(() => setIsLoading(false))
  }, [])

  const handleOnSubmit = (createdProject: Project) => {
    setProjects((existingProjects) => [createdProject, ...existingProjects])
    setIsOpen(false)
  }

  const handleClickProject = (project: Project) => {
    navigate(`/projects/${project.id}`)
  }

  const projectActions = (project: Project): MenuAction[] => [
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
            toast.error(res.error?.detail || 'Failed to export project.')
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
      variant: 'destructive',
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
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Projects"
        actions={
          <Button onClick={() => setIsOpen(true)}>
            <LuPlus data-icon="inline-start" />
            New project
          </Button>
        }
      />
      {!isLoading && projects.length === 0 ? (
        <EmptyState
          icon={<LuBookAudio />}
          title="No projects yet"
          description="Create a project to start uploading and annotating audio."
        >
          <Button onClick={() => setIsOpen(true)}>
            <LuPlus data-icon="inline-start" />
            New project
          </Button>
        </EmptyState>
      ) : (
        <Grid>
          {projects.map((project) => (
            <ItemCard
              key={project.id}
              title={project.name}
              onClick={() => handleClickProject(project)}
              actions={project.role === 'Owner' ? projectActions(project) : []}
              icon={<LuBookAudio />}
            />
          ))}
        </Grid>
      )}
      <Dialog open={isOpen} onOpenChange={setIsOpen}>
        <DialogContent>
          <CreateProjectForm onSubmit={handleOnSubmit} />
        </DialogContent>
      </Dialog>
      <Dialog open={isOpenRename} onOpenChange={setIsOpenRename}>
        <DialogContent>
          <RenameProjectForm
            onSubmit={handleRename}
            project={selectedProject}
            setProject={setSelectedProject}
          />
        </DialogContent>
      </Dialog>
      <ConfirmDeleteDialog
        open={isOpenDelete}
        onOpenChange={setIsOpenDelete}
        title="Delete Project"
        description={`Do you want to delete ${selectedProject?.name}?`}
        onConfirm={() => selectedProject && handleDelete(selectedProject)}
      />
    </div>
  )
}
