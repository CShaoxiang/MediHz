import { useParams } from 'react-router-dom'
import WaveformRender from '../Components/Waveform/WaveformRender'
import Annotations from '../Components/Annotations/Annotation'
import { useEffect, useState } from 'react'
import type { Region } from 'wavesurfer.js/dist/plugins/regions.js'
import type { RegionData } from '../Interfaces/RegionData'
import type { HubConnection } from '@microsoft/signalr'
import { getAudioFileUrl, getAnnotations } from '../Api/audio'
import { fetchProject, fetchSingleAudio } from '../Api/projects'
import { hubConnection } from '../Api/hub'
import type { AnnotationDto } from '../Types/audio'

export default function AnnotationPage() {
  const { projectId, audioId } = useParams<{ projectId: string; audioId: string }>()
  if (!projectId || !audioId) return <p role="alert">Audio not found.</p>
  return (
    <AnnotationContent key={`${projectId}/${audioId}`} projectId={projectId} audioId={audioId} />
  )
}

function AnnotationContent({ projectId, audioId }: { projectId: string; audioId: string }) {
  const [selectedRegion, setSelectedRegion] = useState<Region | null>(null)
  const [regionData, setRegionData] = useState<Map<string, RegionData>>(new Map())
  const [regionTags, setRegionTags] = useState<Set<string>>(new Set())
  const [savedAnnotations, setSavedAnnotations] = useState<AnnotationDto[]>([])
  const [connection, setConnection] = useState<HubConnection>()
  const [loaded, setLoaded] = useState<{ url: string; name: string; canWrite: boolean }>()
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
        const annotations = await getAnnotations(audioId)
        if (!annotations.ok) throw new Error(annotations.error.detail || 'Annotations unavailable.')
        const audio = await getAudioFileUrl(projectId, audioId)
        if (!audio.ok) throw new Error(audio.error.detail || 'Audio unavailable.')
        objectUrl = audio.value
        if (!active) {
          URL.revokeObjectURL(objectUrl)
          return
        }
        setSavedAnnotations(annotations.value)
        setLoaded({
          url: audio.value,
          name: metadata.value.fileName,
          canWrite: project.value.role === 'Owner' || project.value.role === 'Annotator',
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

  useEffect(() => {
    if (!loaded) return
    let active = true
    let live: HubConnection | undefined
    async function connect() {
      try {
        live = await hubConnection()
        if (!active) {
          await live.stop()
          return
        }
        const join = () => live!.invoke('JoinAudioGroup', audioId)
        live.onreconnected(() => {
          void join().catch(() => setError('Project access could not be restored.'))
        })
        await join()
        if (!active) {
          await live.stop()
          return
        }
        setConnection(live)
      } catch {
        if (active) setError('Unable to connect to project updates. Please reload.')
      }
    }
    void connect()
    return () => {
      active = false
      if (live) void live.stop()
    }
  }, [loaded, audioId])

  if (error) return <p role="alert">{error}</p>
  if (!loaded) return <p role="status">Loading audio…</p>

  return (
    <div className="w-full">
      <h1 className="text-xl m-1">{loaded.name}</h1>
      {!loaded.canWrite && <p>Viewer access — annotations are read-only.</p>}
      <WaveformRender
        audio={loaded.url}
        audioId={audioId}
        canEdit={loaded.canWrite}
        setSelectedRegion={setSelectedRegion}
        setRegionData={setRegionData}
        connection={connection}
        savedAnnotations={savedAnnotations}
      />
      {selected && (
        <div className="m-2 flex min-h-0 max-h-[calc(100svh-12rem)] overflow-hidden">
          <Annotations
            audioId={audioId}
            region={selectedRegion}
            canEdit={loaded.canWrite}
            selectedregionData={regionData.get(selectedRegion.id)}
            setRegionData={setRegionData}
            regionTags={regionTags}
            setRegionTags={setRegionTags}
            connection={connection}
          />
        </div>
      )}
    </div>
  )
}
