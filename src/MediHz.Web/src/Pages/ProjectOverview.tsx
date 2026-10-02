import Card from '../Components/Common/Card'
import AudioUploader from '../Components/AudioUploader'
import type { AudioFile } from '../Types/audio'
import { useEffect, useState } from 'react'
import { fetchProjectAudioFiles } from '../Api/projects'
import { useLocation, useNavigate, useParams } from 'react-router-dom'
import Grid from '../Components/Common/Grid'
import { LuAudioLines } from 'react-icons/lu'
import Modal from '../Components/Common/Modal'
import RenameAudioForm from '../Components/Audio/RenameAudioForm'
import { deleteAudioFile, renameAudioFile } from '../Api/audio'
import DeleteAudioForm from '../Components/Audio/DeleteAudioForm'

type PageParams = {
  projectId: string
}

export default function ProjectOverviewPage() {
  const [audioFiles, setAudioFiles] = useState<AudioFile[]>([])
  const [isOpenRename, setIsOpenRename] = useState(false)
  const [isOpenDelete, setIsOpenDelete] = useState(false)
  const [selectedAudio, setSelectedAudio] = useState<AudioFile>()
  const location = useLocation()
  const { projectId } = useParams<PageParams>()
  const navigate = useNavigate()

  const audioActions = (audio: AudioFile) => [
    {
      label: 'Rename',
      onClick: () => {
        ;(setIsOpenRename(true), setSelectedAudio(audio))
      },
    },
    {
      label: 'Delete',
      onClick: () => {
        ;(setIsOpenDelete(true), setSelectedAudio(audio))
      },
      classname: 'text-red font-bold',
    },
  ]

  useEffect(() => {
    if (projectId) {
      fetchProjectAudioFiles(projectId)
        .then((result) => {
          if (result.ok) setAudioFiles(result.value)
          else console.error(result.error)
        })
        .catch((e) => console.error(e))
    } else {
      console.warn('projectId is undefined')
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

  return (
    <div className="">
      <h1 className="text-2xl font-bold p-4 text-center">
        {location.state?.projectName}'s
        <br />
        Project Overview:
      </h1>
      <Grid>
        {projectId && <AudioUploader projectId={projectId} onUploadFiles={onUploadFiles} />}
        {audioFiles.map((audioFile, i) => (
          <Card
            key={i}
            title={audioFile.fileName}
            onClick={() => {
              navigate(`${audioFile.id}`)
            }}
            actions={audioActions(audioFile)}
          >
            <LuAudioLines className="icon-small" />
          </Card>
        ))}
      </Grid>
      <Modal modalHeader={''} state={isOpenRename} setState={setIsOpenRename}>
        <RenameAudioForm onSubmit={handleRename} audio={selectedAudio} />
      </Modal>
      <Modal modalHeader={''} state={isOpenDelete} setState={setIsOpenDelete}>
        <DeleteAudioForm onSubmit={handleDelete} audio={selectedAudio} />
      </Modal>
    </div>
  )
}
