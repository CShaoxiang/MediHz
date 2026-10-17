import { useCallback, useEffect, useState } from 'react'
import type { HubConnection } from '@microsoft/signalr'
import { getAnnotations } from '../Api/audio'
import {
  createAudioHubConnection,
  deleteAnnotation,
  HUB_UNAVAILABLE,
  saveAnnotation,
  startHubConnection,
  updateAnnotation,
} from '../Api/hub'
import type { AnnotationRegion } from '../Interfaces/AnnotationRegion'
import type { ApiResult, ProblemDetails } from '../Types/api'
import type { AnnotationDto } from '../Types/audio'

type AnnotationMap = Map<string, AnnotationRegion>

const ANNOTATION_DELETED: ProblemDetails = {
  title: 'Annotation deleted',
  detail: 'This annotation was deleted by another user.',
  status: 404,
}

function roundTime(seconds: number) {
  return Math.round(seconds * 10_000) / 10_000
}

function fromDto(dto: AnnotationDto): AnnotationRegion {
  return {
    id: dto.id,
    saved: true,
    changed: false,
    startTime: dto.startTimeSeconds,
    endTime: dto.endTimeSeconds,
    tags: new Set(dto.labelNames),
    authorId: dto.authorId,
    authorDisplayName: dto.authorDisplayName,
  }
}

function withAnnotation(annotations: AnnotationMap, annotation: AnnotationRegion) {
  return new Map(annotations).set(annotation.id, annotation)
}

function withoutAnnotation(annotations: AnnotationMap, id: string) {
  if (!annotations.has(id)) return annotations
  const next = new Map(annotations)
  next.delete(id)
  return next
}

export function useAnnotations(audioId: string) {
  const [annotations, setAnnotations] = useState<AnnotationMap>(new Map())
  const [connection, setConnection] = useState<HubConnection>()

  const loadAnnotations = useCallback(async () => {
    const res = await getAnnotations(audioId)
    if (!res.ok) return

    setAnnotations((current) => {
      const next: AnnotationMap = new Map([...current].filter(([, a]) => !a.saved))
      res.value.forEach((dto) => next.set(dto.id, fromDto(dto)))
      return next
    })
  }, [audioId])

  useEffect(() => {
    const hub = createAudioHubConnection(audioId)
    hub.on('ReceiveAnnotationUpdate', (dto: AnnotationDto) =>
      setAnnotations((current) => withAnnotation(current, fromDto(dto))),
    )
    hub.on('ReceiveAnnotationDelete', (id: string) =>
      setAnnotations((current) => withoutAnnotation(current, id)),
    )
    hub.onreconnected(async () => {
      try {
        await hub.invoke('JoinAudioGroup', audioId)
        await loadAnnotations()
      } catch (error) {
        console.error('Could not rejoin audio updates:', error)
      }
    })

    const connect = async () => {
      if (await startHubConnection(hub, audioId)) {
        setConnection(hub)
      }
      await loadAnnotations()
    }

    connect()

    return () => {
      hub.stop()
    }
  }, [audioId, loadAnnotations])

  const addAnnotation = useCallback((id: string, startTime: number, endTime: number) => {
    setAnnotations((current) =>
      withAnnotation(current, {
        id,
        saved: false,
        changed: false,
        startTime: roundTime(startTime),
        endTime: roundTime(endTime),
        tags: new Set(),
      }),
    )
  }, [])

  const moveAnnotation = useCallback((id: string, startTime: number, endTime: number) => {
    setAnnotations((current) => {
      const annotation = current.get(id)
      if (!annotation) return current
      return withAnnotation(current, {
        ...annotation,
        startTime: roundTime(startTime),
        endTime: roundTime(endTime),
        changed: true,
      })
    })
  }, [])

  const restoreAnnotationTime = useCallback((id: string, startTime: number, endTime: number) => {
    setAnnotations((current) => {
      const annotation = current.get(id)
      if (!annotation) return current
      return withAnnotation(current, {
        ...annotation,
        startTime,
        endTime,
        changed: false,
      })
    })
  }, [])

  const saveTags = async (id: string, tags: Set<string>): Promise<ApiResult<null>> => {
    const annotation = annotations.get(id)
    if (!annotation) return { ok: false, error: ANNOTATION_DELETED }
    if (!connection) return { ok: false, error: HUB_UNAVAILABLE }

    const next = { ...annotation, tags, saved: true , changed: false}
    setAnnotations((current) => withAnnotation(current, next))

    const result = annotation.saved
      ? await updateAnnotation(connection, audioId, next)
      : await saveAnnotation(connection, audioId, next)

    if (!result.ok) {
      setAnnotations((current) =>
        current.get(id) === next ? withAnnotation(current, annotation) : current,
      )
    } else if (!annotation.saved) {
      await loadAnnotations()
    }
    return result
  }

  const removeAnnotation = async (id: string): Promise<ApiResult<null>> => {
    const annotation = annotations.get(id)
    if (!annotation) return { ok: true, value: null }

    setAnnotations((current) => withoutAnnotation(current, id))
    if (!annotation.saved) return { ok: true, value: null }

    const result = connection
      ? await deleteAnnotation(connection, id, audioId)
      : { ok: false as const, error: HUB_UNAVAILABLE }

    if (!result.ok) {
      setAnnotations((current) => (current.has(id) ? current : withAnnotation(current, annotation)))
    }
    return result
  }

  return {
    annotations,
    addAnnotation,
    moveAnnotation,
    restoreAnnotationTime,
    saveTags,
    removeAnnotation,
  }
}
