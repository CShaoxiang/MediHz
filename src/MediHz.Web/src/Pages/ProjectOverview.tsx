import { toast } from 'sonner'
import ItemCard from '../Components/Common/ItemCard'
import AudioUploader from '../Components/AudioUploader'
import type { AudioFile } from '../Types/audio'
import type { MenuAction } from '../Types/actions'
import { useEffect, useState } from 'react'
import { fetchProject, fetchProjectAudioFiles } from '../Api/projects'
import { useNavigate, useParams } from 'react-router-dom'
import type { Project } from '../Types/projects'
import AddProjectMemberForm from '../Components/Projects/AddProjectMemberForm'
import PageHeader from '../Components/Common/PageHeader'
import EmptyState from '../Components/Common/EmptyState'
import { Badge } from '../Components/Shadcn/badge'
import Grid from '../Components/Common/Grid'
import { LuAudioLines, LuCircleAlert } from 'react-icons/lu'
import { Dialog, DialogContent } from '../Components/Shadcn/dialog'
import RenameAudioForm from '../Components/Audio/RenameAudioForm'
import { deleteAudioFile, renameAudioFile } from '../Api/audio'
import { exportFile } from '../Api/exports'
import { downloadJsonFile } from '../Utilities/download'
import ConfirmDeleteDialog from '../Components/Common/ConfirmDeleteDialog'
import MemberPresence from '../Components/Projects/MemberPresence'

type PageParams = {
  projectId: string
}

export default function ProjectOverviewPage() {
  const [audioFiles, setAudioFiles] = useState<AudioFile[]>([])
  const [isOpenRename, setIsOpenRename] = useState(false)
  const [isOpenDelete, setIsOpenDelete] = useState(false)
  const [selectedAudio, setSelectedAudio] = useState<AudioFile>()
  const [project, setProject] = useState<Project>()
  const [error, setError] = useState('')
  const { projectId } = useParams<PageParams>()
  const navigate = useNavigate()

  const audioActions = (audio: AudioFile): MenuAction[] => [
    {
      label: 'Rename',
      onClick: () => {
        setIsOpenRename(true)
        setSelectedAudio(audio)
      },
    },
    {
      label: 'Export',
      onClick: () => {
        exportFile(audio.id).then((res) => {
          if (res.ok) {
            downloadJsonFile(res.value, `${audio.fileName || 'file'}_export.json`)
          } else {
            console.error(res.error)
            toast.error(res.error?.detail || 'Failed to export file.')
          }
        })
      },
    },
    {
      label: 'Delete',
      onClick: () => {
        setIsOpenDelete(true)
        setSelectedAudio(audio)
      },
      variant: 'destructive',
    },
  ]

  useEffect(() => {
    let active = true
    async function load() {
      if (!projectId) return
      try {
        const result = await fetchProject(projectId)
        if (!active) return
        if (!result.ok) {
          setError(result.error.detail || 'Project unavailable.')
          return
        }
        const files = await fetchProjectAudioFiles(projectId)
        if (!active) return
        if (!files.ok) {
          setError(files.error.detail || 'Unable to load audio.')
          return
        }
        setProject(result.value)
        setAudioFiles(files.value)
        setError('')
      } catch {
        if (active) setError('Unable to load the project. Please try again.')
      }
    }
    void load()
    return () => {
      active = false
    }
  }, [projectId])

  const onUploadFiles = (uploadedFiles: AudioFile[]) => {
    setAudioFiles((current) => [...uploadedFiles, ...current])
  }

  const handleRename = (audio: AudioFile) => {
    if (projectId) {
      renameAudioFile(projectId, audio.id, audio.fileName)
        .then((res) =>
          res.ok
            ? setAudioFiles((current) => current.map((e) => (e.id == audio.id ? audio : e)))
            : console.error(res.error),
        )
        .catch((e) => console.error(e))
    }
    setIsOpenRename(false)
  }

  const handleDelete = (audio: AudioFile) => {
    if (projectId) {
      deleteAudioFile(projectId, audio.id)
        .then((res) =>
          res.ok
            ? setAudioFiles((current) => current.filter((e) => e.id != audio.id))
            : console.error(res.error),
        )
        .catch((e) => console.error(e))
    }
    setIsOpenDelete(false)
  }

  if (error) {
    return <EmptyState icon={<LuCircleAlert />} title="Project unavailable" description={error} />
  }
  if (!project || project.id !== projectId) {
    return (
      <p role="status" className="text-muted-foreground">
        Loading project…
      </p>
    )
  }
  const canWrite = project.role === 'Owner' || project.role === 'Annotator'

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        breadcrumbs={[{ label: 'Projects', to: '/projects' }]}
        title={project.name}
        actions={
          <div className="flex flex-wrap items-center gap-4">
            <MemberPresence projectId={project.id} />
            {project.role === 'Owner' ? (
              <AddProjectMemberForm key={project.id} projectId={project.id} />
            ) : (
              !canWrite && <Badge variant="secondary">Read only</Badge>
            )}
          </div>
        }
      />
      {canWrite && <AudioUploader projectId={project.id} onUploadFiles={onUploadFiles} />}
      {audioFiles.length === 0 ? (
        <EmptyState
          icon={<LuAudioLines />}
          title="No audio files yet"
          description={
            canWrite
              ? 'Drop MP3 or WAV files above to add them to this project.'
              : 'Audio uploaded to this project will appear here.'
          }
        />
      ) : (
        <Grid>
          {audioFiles.map((audioFile) => (
            <ItemCard
              key={audioFile.id}
              title={audioFile.fileName}
              onClick={() => {
                navigate(`${audioFile.id}`)
              }}
              actions={canWrite ? audioActions(audioFile) : []}
              icon={<LuAudioLines />}
            />
          ))}
        </Grid>
      )}
      <Dialog open={isOpenRename} onOpenChange={setIsOpenRename}>
        <DialogContent>
          <RenameAudioForm onSubmit={handleRename} audio={selectedAudio} />
        </DialogContent>
      </Dialog>
      <ConfirmDeleteDialog
        open={isOpenDelete}
        onOpenChange={setIsOpenDelete}
        title="Delete Audio"
        description={`Do you want to delete ${selectedAudio?.fileName}?`}
        onConfirm={() => selectedAudio && handleDelete(selectedAudio)}
      />
    </div>
  )
}
