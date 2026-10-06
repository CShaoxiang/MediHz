import { useParams } from 'react-router-dom'
import WaveformRender from '../Components/Waveform/WaveformRender'
import Annotations from '../Components/Annotations/Annotation'
import { useCallback, useEffect, useState } from 'react'
import { getAudioFileUrl } from '../Api/audio'
import { fetchSingleAudio } from '../Api/projects'
import { showApiError } from '../Api/helpers'
import { useAnnotations } from '../Hooks/useAnnotations'

type PageParams = {
  projectId: string
  audioId: string
}

export default function AnnotationPage() {
  const { projectId, audioId } = useParams<PageParams>()
  if (!projectId || !audioId) return null

  return <AnnotationWorkspace key={audioId} projectId={projectId} audioId={audioId} />
}

function AnnotationWorkspace({ projectId, audioId }: PageParams) {
  const { annotations, addAnnotation, moveAnnotation, saveTags, removeAnnotation } =
    useAnnotations(audioId)
  const [selectedId, setSelectedId] = useState<string | null>(null)

  const [audio, setAudio] = useState<string>()
  const [audioName, setAudioName] = useState<string>()

  useEffect(() => {
    const fetchAudio = async () => {
      const res = await fetchSingleAudio(projectId, audioId)
      if (res.ok) {
        setAudioName(res.value.fileName)
      }

      const urlRes = await getAudioFileUrl(projectId, audioId)
      if (urlRes.ok) {
        setAudio(urlRes.value)
      }
    }

    fetchAudio()
  }, [projectId, audioId])

  const handleDraw = useCallback(
    (id: string, startTime: number, endTime: number) => {
      addAnnotation(id, startTime, endTime)
      setSelectedId(id)
    },
    [addAnnotation],
  )

  const handleDelete = async (id: string) => {
    setSelectedId(null)
    const result = await removeAnnotation(id)
    if (!result.ok) showApiError(result.error)
  }

  const selected = selectedId ? annotations.get(selectedId) : undefined

  return (
    <div className="w-full">
      <h1 className="text-xl m-1">{audioName}</h1>
      <WaveformRender
        audio={audio}
        annotations={annotations}
        selectedId={selectedId}
        onSelect={setSelectedId}
        onDraw={handleDraw}
        onMove={moveAnnotation}
        onDelete={handleDelete}
      />
      {selected && (
        <div className="m-2 flex min-h-0 max-h-[calc(100svh-12rem)] overflow-hidden">
          <Annotations
            key={selected.id}
            annotation={selected}
            onSave={(tags) => saveTags(selected.id, tags)}
          />
        </div>
      )}
    </div>
  )
}
