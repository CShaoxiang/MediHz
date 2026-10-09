import Card from '../Components/Common/Card'
import AudioUploader from '../Components/AudioUploader'
import type { AudioFile } from '../Types/audio'
import { useEffect, useState } from 'react'
import { fetchProject, fetchProjectAudioFiles } from '../Api/projects'
import { useNavigate, useParams } from 'react-router-dom'
import type { Project } from '../Types/projects'
import AddProjectMemberForm from '../Components/Projects/AddProjectMemberForm'
import Grid from '../Components/Common/Grid'
import { LuAudioLines } from 'react-icons/lu'
import Modal from '../Components/Common/Modal'
import RenameAudioForm from '../Components/Audio/RenameAudioForm'
import { deleteAudioFile, renameAudioFile } from '../Api/audio'
import DeleteAudioForm from '../Components/Audio/DeleteAudioForm'
import { exportFile } from '../Api/exports'
import { downloadJsonFile } from '../Utilities/download'

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

  const audioActions = (audio: AudioFile) => [
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
            alert(res.error?.detail || 'Failed to export file.')
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
      classname: 'text-red font-bold',
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

  if (error) return <p role="alert">{error}</p>
  if (!project || project.id !== projectId) return <p role="status">Loading project…</p>
  const canWrite = project.role === 'Owner' || project.role === 'Annotator'

  return (
    <div className="mx-auto w-full max-w-6xl px-4 pb-10 pt-8 sm:px-6">
      <header className="flex flex-wrap items-center justify-between gap-4 border-b border-slate-200 pb-6">
        <div className="min-w-0">
          <p className="mb-1 text-sm font-medium text-purple-700">Project overview</p>
          <h1 className="wrap-break-word text-2xl font-semibold tracking-tight text-slate-900 sm:text-3xl">
            {project.name}
          </h1>
          {project.role === 'Viewer' && (
            <p className="mt-2 text-sm text-slate-600">Viewer access · Read only</p>
          )}
        </div>
        {project.role === 'Owner' && (
          <AddProjectMemberForm key={project.id} projectId={project.id} />
        )}
      </header>
      <section aria-labelledby="audio-files-heading" className="pt-7">
        <div className="mb-5 flex items-baseline justify-between gap-4">
          <h2 id="audio-files-heading" className="text-lg font-semibold text-slate-900">
            Audio files
          </h2>
          <span className="text-sm text-slate-500">
            {audioFiles.length} {audioFiles.length === 1 ? 'file' : 'files'}
          </span>
        </div>
        <Grid>
          {projectId && canWrite && (
            <AudioUploader projectId={projectId} onUploadFiles={onUploadFiles} />
          )}
          {audioFiles.map((audioFile) => (
            <Card
              key={audioFile.id}
              title={audioFile.fileName}
              onClick={() => {
                navigate(`${audioFile.id}`)
              }}
              actions={canWrite ? audioActions(audioFile) : []}
            >
              <LuAudioLines className="icon-small" />
            </Card>
          ))}
        </Grid>
      </section>
      <Modal modalHeader={''} state={isOpenRename} setState={setIsOpenRename}>
        <RenameAudioForm onSubmit={handleRename} audio={selectedAudio} />
      </Modal>
      <Modal modalHeader={''} state={isOpenDelete} setState={setIsOpenDelete}>
        <DeleteAudioForm onSubmit={handleDelete} audio={selectedAudio} />
      </Modal>
    </div>
  )
}
