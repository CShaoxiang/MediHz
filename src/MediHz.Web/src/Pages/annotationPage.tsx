import { useCallback, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getAudioFileUrl } from '../Api/audio'
import { showApiError } from '../Api/helpers'
import { fetchProject, fetchSingleAudio } from '../Api/projects'
import type { ApiResult } from '../Types/api'
import Annotations from '../Components/Annotations/Annotation'
import AnnotationList from '../Components/Annotations/AnnotationList'
import LabelPalette, {
  ALL_GROUPS_ID,
  UNASSIGNED_GROUP_ID,
} from '../Components/Annotations/LabelPalette'
import WaveformRender from '../Components/Waveform/WaveformRender'
import { useAnnotations } from '../Hooks/useAnnotations'
import { useLabels } from '../Hooks/useLabels'
import { useAuth } from '../Auth/AuthContext'
import PageHeader from '../Components/Common/PageHeader'
import EmptyState from '../Components/Common/EmptyState'
import { Badge } from '../Components/Shadcn/badge'
import { LuCircleAlert, LuMousePointerClick } from 'react-icons/lu'
import MemberPresence from '../Components/Projects/MemberPresence'

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
  const { labels, labelsByGroup, prePackagedGroups } = useLabels()
  const { user } = useAuth()
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [selectedTimestamp, setSelectedTimestamp] = useState<{
    start: number
    end: number
  } | null>(null)
  const [draftTags, setDraftTags] = useState<Set<string> | null>(null)
  const [activeGroupId, setActiveGroupId] = useState<string>(() => {
    try {
      return localStorage.getItem('medihz:activeLabelGroup') || ALL_GROUPS_ID
    } catch {
      return ALL_GROUPS_ID
    }
  })
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

  const hasStoredGroup = () => {
    try {
      return !!localStorage.getItem('medihz:activeLabelGroup')
    } catch {
      return false
    }
  }

  const effectiveActiveGroupId =
    activeGroupId === ALL_GROUPS_ID && prePackagedGroups.length > 0 && !hasStoredGroup()
      ? prePackagedGroups[0].id
      : activeGroupId

  const handleActiveGroupChange = (groupId: string) => {
    setActiveGroupId(groupId)
    try {
      localStorage.setItem('medihz:activeLabelGroup', groupId)
    } catch {
      // Ignore localStorage availability issues
    }
  }

  const handleSelectRegion = (id: string) => {
    setSelectedId(id)
    setDraftTags(null)
    const target = annotations.get(id)
    if (target) {
      setSelectedTimestamp({ start: target.startTime, end: target.endTime })
    }
  }

  const handleDraw = useCallback(
    (id: string, startTime: number, endTime: number) => {
      if (!loaded?.canEdit) return
      addAnnotation(id, startTime, endTime)
      setSelectedId(id)
      setSelectedTimestamp({ start: startTime, end: endTime })
      setDraftTags(null)
    },
    [addAnnotation, loaded?.canEdit],
  )

  const handleSwitch = (startTime: number, endTime: number) => {
    setSelectedTimestamp({ start: startTime, end: endTime })
  }

  const handleDelete = async (id: string) => {
    if (!loaded?.canEdit) return
    setSelectedId(null)
    setSelectedTimestamp(null)
    setDraftTags(null)
    const result = await removeAnnotation(id)
    if (!result.ok) showApiError(result.error)
  }

  const selected = selectedId ? annotations.get(selectedId) : undefined

  const handleToggleTag = useCallback(
    (tagName: string) => {
      if (!selected || !loaded?.canEdit) return
      const current = new Set(draftTags ?? selected.tags)
      if (current.has(tagName)) {
        current.delete(tagName)
      } else {
        current.add(tagName)
      }
      setDraftTags(current)
    },
    [selected, draftTags, loaded?.canEdit],
  )

  // Hotkeys 1-9 for rapid tagging
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (!selected || !loaded?.canEdit) return
      const target = e.target as HTMLElement | null
      if (
        target?.tagName === 'INPUT' ||
        target?.tagName === 'TEXTAREA' ||
        target?.isContentEditable
      ) {
        return
      }

      const num = parseInt(e.key, 10)
      if (num >= 1 && num <= 9) {
        const groupLabels =
          effectiveActiveGroupId === ALL_GROUPS_ID
            ? labels
            : effectiveActiveGroupId === UNASSIGNED_GROUP_ID
              ? (labelsByGroup.get(null) ?? [])
              : (labelsByGroup.get(effectiveActiveGroupId) ?? [])

        const targetLabel = groupLabels[num - 1]
        if (targetLabel) {
          e.preventDefault()
          handleToggleTag(targetLabel.name)
        }
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [selected, loaded?.canEdit, effectiveActiveGroupId, labels, labelsByGroup, handleToggleTag])

  const handleSaveTags = async (tags: Set<string>): Promise<ApiResult<null>> => {
    if (!selected)
      return {
        ok: false as const,
        error: { title: 'No region', detail: 'No region selected', status: 400 },
      }
    const result = await saveTags(selected.id, tags)
    if (result.ok) {
      setDraftTags(null)
    }
    return result
  }

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
        actions={
          <div className="flex flex-wrap items-center gap-4">
            <MemberPresence projectId={projectId} />
            {!loaded.canEdit && <Badge variant="secondary">Read only</Badge>}
          </div>
        }
      />
      <WaveformRender
        audio={loaded.url}
        annotations={annotations}
        currentUserId={user?.userId}
        selectedId={selectedId}
        selectedTimestamp={selectedTimestamp}
        canEdit={loaded.canEdit}
        onSelect={handleSelectRegion}
        onChange={handleSwitch}
        onDraw={handleDraw}
        onMove={moveAnnotation}
        onDelete={handleDelete}
      />
      <div className="grid items-start gap-6 lg:min-h-48 lg:flex-1 lg:grid-cols-3 lg:grid-rows-1">
        <LabelPalette
          activeGroupId={effectiveActiveGroupId}
          onActiveGroupChange={handleActiveGroupChange}
          selectedTags={selected ? (draftTags ?? selected.tags) : undefined}
          onToggleTag={handleToggleTag}
          canEdit={loaded.canEdit}
          hasSelection={!!selected}
        />
        {selected ? (
          <Annotations
            key={selected.id}
            annotation={selected}
            selectedTimestamp={selectedTimestamp}
            draftTags={draftTags}
            onDraftTagsChange={setDraftTags}
            canEdit={loaded.canEdit}
            onChange={handleSwitch}
            onSave={handleSaveTags}
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
          onSelect={handleSelectRegion}
        />
      </div>
    </div>
  )
}
