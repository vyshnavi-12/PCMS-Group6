import * as signalR from '@microsoft/signalr'

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL

let connection: signalR.HubConnection | null = null

export const startNotificationSignalRConnection = async (
  userId: string,
  onNotificationReceived: (payload: any) => void
) => {
  if (connection) return

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${SIGNALR_BASE_URL}/notificationHub`, {
      withCredentials: true
    })
    .withAutomaticReconnect()
    .build()

  connection.on('ReceiveNotification', payload => {
    onNotificationReceived(payload)
  })

  try {
    await connection.start()
    console.log('SignalR Connected')

    await connection.invoke('JoinUserGroup', userId)
  } catch (error) {
    console.error('SignalR connection error:', error)
  }
}

export const stopSignalRConnection = async () => {
  if (connection) {
    await connection.stop()
    connection = null
  }
}