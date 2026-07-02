import * as signalR from '@microsoft/signalr'

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL;

// 1. Initialize the connection immediately so it is never null
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${SIGNALR_BASE_URL}/unavailableRequestHub`, {
    withCredentials: true
  })
  .withAutomaticReconnect()
  .build()

// Flag to prevent starting multiple times
let isStarted = false 

const unavailableRequestSignalRService = {
  async startConnection(userId: string) {
    if (isStarted) return

    try {
      await connection.start()
      isStarted = true
      console.log('Unavailable Request SignalR Connected')
      await connection.invoke('JoinUserGroup', userId)
    } catch (error) {
      console.error('Unavailable SignalR error:', error)
    }
  },

  onNewUnavailableRequest(callback: () => void) {
    // 2. Remove the null check. We can attach listeners before the connection officially "starts"
    connection.on('NewUnavailableRequest', callback)
  },

  onUnavailableRequestUpdated(callback: () => void) {
    // 2. Remove the null check.
    connection.on('UnavailableRequestUpdated', callback)
  },
  offNewUnavailableRequest(callback: () => void) {
    connection.off("NewUnavailableRequest", callback)
},

offUnavailableRequestUpdated(callback: () => void) {
    connection.off("UnavailableRequestUpdated", callback)
},
}

export default unavailableRequestSignalRService