import * as signalR from '@microsoft/signalr'

let connection: signalR.HubConnection | null = null

const unavailableRequestSignalRService = {
  async startConnection(userId: string) {
    if (connection) return

    connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7119/unavailableRequestHub', {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build()

    try {
      await connection.start()
      console.log('Unavailable Request SignalR Connected')

      await connection.invoke('JoinUserGroup', userId)
    } catch (error) {
      console.error('Unavailable SignalR error:', error)
    }
  },

  onNewUnavailableRequest(callback: () => void) {
    if (!connection) return
    connection.on('NewUnavailableRequest', callback)
  },

  onUnavailableRequestUpdated(callback: () => void) {
    if (!connection) return
    connection.on('UnavailableRequestUpdated', callback)
  }
}

export default unavailableRequestSignalRService