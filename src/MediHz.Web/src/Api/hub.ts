import * as SignalR from '@microsoft/signalr'
import type { RegionData } from '../Interfaces/RegionData'
import { getToken } from '../Auth/tokenStorage'

const HUB_BASE = '/hubs'

export async function hubConnection() {
  const connection = new SignalR.HubConnectionBuilder()
    .withUrl(`${HUB_BASE}/audio`, {
      skipNegotiation: true,
      transport: SignalR.HttpTransportType.WebSockets,
      accessTokenFactory: () => getToken() || '',
    })
    .withAutomaticReconnect()
    .build()

  connection.on('ReceiveMessage', (message: string) => {
    if (message) {
      console.log('Received message from server:', message)
    }
  })

  connection.on('ReceiveRegionData', (audioId: string, regionId: string) => {
    console.log('Recieved region from server: ', audioId, regionId)
  })

  await connection
    .start()
    .then(() => {
      console.log('SignalR connection established.')
    })
    .catch((error) => {
      console.error('Error establishing SignalR connection:', error)
    })

  return connection
}

export function callHub(connection: SignalR.HubConnection) {
  connection.invoke('SendMessage', 'hello').catch((error) => console.log(error))
}

export function sendData(connection: SignalR.HubConnection, audioId: string, regionId: string) {
  connection.invoke('SendRegionData', audioId, regionId).catch((error) => console.log(error))
}

export function joinAudioGroup(connection: SignalR.HubConnection, audioId: string) {
  connection.invoke('JoinAudioGroup', audioId).catch(console.error)
}

export function leaveAudioGroup(connection: SignalR.HubConnection, audioId: string) {
  connection.invoke('LeaveAudioGroup', audioId).catch(console.error)
}

export function saveAnnotation(
  connection: SignalR.HubConnection,
  audioId: string,
  regionId: string,
  regionData: RegionData,
) {
  const request = {
    regionId: regionId,
    audioFileId: audioId,
    labelNames: regionData.tags ? Array.from(regionData.tags) : [],
    startTimeSeconds: regionData.startTime ?? 0,
    endTimeSeconds: regionData.endTime ?? 0,
  }
  return connection.invoke('SaveAnnotation', request).catch((error) => console.log(error))
}

export function updateAnnotation(
  connection: SignalR.HubConnection,
  annotationId: string,
  audioId: string,
  regionData: RegionData,
) {
  const request = {
    annotationId: annotationId,
    audioFileId: audioId,
    labelNames: regionData.tags ? Array.from(regionData.tags) : [],
    startTimeSeconds: regionData.startTime ?? 0,
    endTimeSeconds: regionData.endTime ?? 0,
  }
  return connection.invoke('UpdateAnnotation', request).catch((error) => console.log(error))
}

export function deleteAnnotation(
  connection: SignalR.HubConnection,
  annotationId: string,
  audioId: string,
) {
  return connection
    .invoke('DeleteAnnotation', annotationId, audioId)
    .catch((error) => console.log(error))
}
