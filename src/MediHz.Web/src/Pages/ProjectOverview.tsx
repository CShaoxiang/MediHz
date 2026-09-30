import Card from '../Components/Common/Card'
import AudioUploader from '../Components/AudioUploader'
import type { AudioFile } from '../Types/audio'
import { useEffect, useState } from 'react'
import { fetchProjectAudioFiles } from '../Api/projects'
import { useNavigate, useParams } from 'react-router-dom'
import Grid from '../Components/Common/Grid'
import { LuAudioLines } from 'react-icons/lu'

type PageParams = {
  projectId: string
}

export default function ProjectOverviewPage() {
  const [audioFiles, setAudioFiles] = useState<AudioFile[]>([])
  const { projectId } = useParams<PageParams>()
  const navigate = useNavigate()

  const audioActions = [
    { label: 'Delete', onClick: () => console.log('delete') },
    { label: 'Rename', onClick: () => {} },
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

  return (
    <div className="">
      <h1 className="text-2xl font-bold p-4 text-center">Project Overview</h1>
      <Grid>
        {projectId && <AudioUploader projectId={projectId} onUploadFiles={onUploadFiles} />}
        {audioFiles.map((audioFile, i) => (
          <Card
            key={i}
            title={audioFile.fileName}
            onClick={() => {
              navigate(`${audioFile.id}`)
            }}
            actions={audioActions}
          >
            <LuAudioLines className="icon-small" />
          </Card>
        ))}
      </Grid>
    </div>
  )
}
