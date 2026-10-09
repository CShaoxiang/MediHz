import { useCallback, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getAudioFileUrl } from '../Api/audio'
import { showApiError } from '../Api/helpers'
import { fetchProject, fetchSingleAudio } from '../Api/projects'
import Annotations from '../Components/Annotations/Annotation'
import WaveformRender from '../Components/Waveform/WaveformRender'
import { useAnnotations } from '../Hooks/useAnnotations'

type PageParams = { projectId: string; audioId: string }

export default function AnnotationPage() {
  const { projectId, audioId } = useParams<PageParams>()
  if (!projectId || !audioId) return <p role="alert">Audio not found.</p>
  return (
    <AnnotationWorkspace key={`${projectId}/${audioId}`} projectId={projectId} audioId={audioId} />
  )
}

function AnnotationWorkspace({ projectId, audioId }: PageParams) {
  const { annotations, addAnnotation, moveAnnotation, saveTags, removeAnnotation } =
    useAnnotations(audioId)
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [loaded, setLoaded] = useState<{ url: string; name: string; canEdit: boolean }>()
  const [error, setError] = useState('')

  useEffect(() => {
    let active = true
    let objectUrl: string | undefined
    async function load() {
      try {
        const project = await fetchProject(projectId)
        if (!project.ok) throw new Error(project.error.detail || 'Project unavailable.')
        const metadata = await fetchSingleAudio(projectId, audioId)
        if (!metadata.ok) throw new Error(metadata.error.detail || 'Audio unavailable.')
        const audio = await getAudioFileUrl(projectId, audioId)
        if (!audio.ok) throw new Error(audio.error.detail || 'Audio unavailable.')
        objectUrl = audio.value
        if (!active) {
          URL.revokeObjectURL(objectUrl)
          return
        }
        setLoaded({
          url: objectUrl,
          name: metadata.value.fileName,
          canEdit: project.value.role === 'Owner' || project.value.role === 'Annotator',
        })
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : 'Unable to load audio.')
      }
    }
    void load()
    return () => {
      active = false
      if (objectUrl) URL.revokeObjectURL(objectUrl)
    }
  }, [projectId, audioId])

  const handleDraw = useCallback(
    (id: string, startTime: number, endTime: number) => {
      if (!loaded?.canEdit) return
      addAnnotation(id, startTime, endTime)
      setSelectedId(id)
    },
    [addAnnotation, loaded?.canEdit],
  )

  const handleDelete = async (id: string) => {
    if (!loaded?.canEdit) return
    setSelectedId(null)
    const result = await removeAnnotation(id)
    if (!result.ok) showApiError(result.error)
  }

  const selected = selectedId ? annotations.get(selectedId) : undefined

  if (error) return <p role="alert">{error}</p>
  if (!loaded) return <p role="status">Loading audio…</p>

  return (
    <div className="w-full">
      <h1 className="m-1 text-xl">{loaded.name}</h1>
      {!loaded.canEdit && <p>Viewer access — annotations are read-only.</p>}
      <WaveformRender
        audio={loaded.url}
        annotations={annotations}
        selectedId={selectedId}
        canEdit={loaded.canEdit}
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
            canEdit={loaded.canEdit}
            onSave={(tags) => saveTags(selected.id, tags)}
          />
        </div>
      )}
    </div>
  )
}
