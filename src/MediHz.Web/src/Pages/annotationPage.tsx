import { useCallback, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getAudioFileUrl } from '../Api/audio'
import { showApiError } from '../Api/helpers'
import { fetchProject, fetchSingleAudio } from '../Api/projects'
import Annotations from '../Components/Annotations/Annotation'
import AnnotationList from '../Components/Annotations/AnnotationList'
import WaveformRender from '../Components/Waveform/WaveformRender'
import { useAnnotations } from '../Hooks/useAnnotations'
import { useAuth } from '../Auth/AuthContext'
import PageHeader from '../Components/Common/PageHeader'
import EmptyState from '../Components/Common/EmptyState'
import { Badge } from '../Components/Shadcn/badge'
import { LuCircleAlert, LuMousePointerClick } from 'react-icons/lu'

type PageParams = { projectId: string; audioId: string }

export default function AnnotationPage() {
  const { projectId, audioId } = useParams<PageParams>()
  if (!projectId || !audioId) {
    return (
      <EmptyState
        icon={<LuCircleAlert />}
        title="Audio not found"
        description="This audio file doesn't exist."
      />
    )
  }
  return (
    <AnnotationWorkspace key={`${projectId}/${audioId}`} projectId={projectId} audioId={audioId} />
  )
}

function AnnotationWorkspace({ projectId, audioId }: PageParams) {
  const { annotations, addAnnotation, moveAnnotation, saveTags, removeAnnotation } =
    useAnnotations(audioId)
  const { user } = useAuth()
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [loaded, setLoaded] = useState<{
    url: string
    name: string
    projectName: string
    canEdit: boolean
  }>()
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
          projectName: project.value.name,
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

  if (error) {
    return <EmptyState icon={<LuCircleAlert />} title="Audio unavailable" description={error} />
  }
  if (!loaded) {
    return (
      <p role="status" className="text-muted-foreground">
        Loading audio…
      </p>
    )
  }

  return (
    <div className="flex flex-col gap-6 lg:h-full">
      <PageHeader
        breadcrumbs={[
          { label: 'Projects', to: '/projects' },
          { label: loaded.projectName, to: `/projects/${projectId}` },
        ]}
        title={loaded.name}
        actions={!loaded.canEdit && <Badge variant="secondary">Read only</Badge>}
      />
      <WaveformRender
        audio={loaded.url}
        annotations={annotations}
        currentUserId={user?.userId}
        selectedId={selectedId}
        canEdit={loaded.canEdit}
        onSelect={setSelectedId}
        onDraw={handleDraw}
        onMove={moveAnnotation}
        onDelete={handleDelete}
      />
      <div className="grid items-start gap-6 lg:min-h-48 lg:flex-1 lg:grid-cols-2 lg:grid-rows-1">
        {selected ? (
          <Annotations
            key={selected.id}
            annotation={selected}
            canEdit={loaded.canEdit}
            onSave={(tags) => saveTags(selected.id, tags)}
          />
        ) : (
          <EmptyState
            icon={<LuMousePointerClick />}
            title="No region selected"
            description={
              loaded.canEdit
                ? 'Select a region on the waveform or from the list, or turn on Highlight and drag across the waveform to create one.'
                : 'Select a region on the waveform or from the list to see its tags.'
            }
          />
        )}
        <AnnotationList
          annotations={annotations}
          selectedId={selectedId}
          onSelect={setSelectedId}
        />
      </div>
    </div>
  )
}
