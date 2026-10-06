import * as SignalR from '@microsoft/signalr'
import type { AnnotationRegion } from '../Interfaces/AnnotationRegion'
import { getToken } from '../Auth/tokenStorage'
import type { ApiResult, ProblemDetails } from '../Types/api'

const HUB_BASE = '/hubs'

export function createAudioHubConnection(audioId: string) {
  return new SignalR.HubConnectionBuilder()
    .withUrl(`${HUB_BASE}/audio?audioId=${encodeURIComponent(audioId)}`, {
      skipNegotiation: true,
      transport: SignalR.HttpTransportType.WebSockets,
      accessTokenFactory: () => getToken() || '',
    })
    .withAutomaticReconnect()
    .build()
}

export async function startHubConnection(connection: SignalR.HubConnection) {
  try {
    await connection.start()
    console.log('SignalR connection established.')
    return true
  } catch (error) {
    console.error('Error establishing SignalR connection:', error)
    return false
  }
}

function toAnnotationCommand(audioId: string, annotation: AnnotationRegion) {
  return {
    annotationId: annotation.id,
    audioFileId: audioId,
    labelNames: Array.from(annotation.tags),
    startTimeSeconds: annotation.startTime,
    endTimeSeconds: annotation.endTime,
  }
}

export const HUB_UNAVAILABLE: ProblemDetails = {
  title: 'Not connected',
  detail: 'Could not reach the server. Check your connection and try again.',
  status: 0,
}

async function invokeCommand(
  connection: SignalR.HubConnection,
  method: string,
  ...args: unknown[]
): Promise<ApiResult<null>> {
  try {
    const problem = await connection.invoke<ProblemDetails | null>(method, ...args)
    return problem ? { ok: false, error: problem } : { ok: true, value: null }
  } catch (error) {
    console.error(error)
    return { ok: false, error: HUB_UNAVAILABLE }
  }
}

export async function saveAnnotation(
  connection: SignalR.HubConnection,
  audioId: string,
  annotation: AnnotationRegion,
) {
  return invokeCommand(connection, 'SaveAnnotation', toAnnotationCommand(audioId, annotation))
}

export async function updateAnnotation(
  connection: SignalR.HubConnection,
  audioId: string,
  annotation: AnnotationRegion,
) {
  return invokeCommand(connection, 'UpdateAnnotation', toAnnotationCommand(audioId, annotation))
}

export async function deleteAnnotation(
  connection: SignalR.HubConnection,
  annotationId: string,
  audioId: string,
) {
  return invokeCommand(connection, 'DeleteAnnotation', annotationId, audioId)
}
