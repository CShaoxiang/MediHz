import * as SignalR from '@microsoft/signalr'

const HUB_BASE = '/hubs' 

export async function hubConnection() {
  const connection = new SignalR.HubConnectionBuilder()
    .withUrl(`${HUB_BASE}/audio`, {
      skipNegotiation: true,
      transport: SignalR.HttpTransportType.WebSockets,
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
