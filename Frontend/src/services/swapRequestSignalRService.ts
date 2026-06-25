// src/services/swapRequestsSignalRService.ts
import * as signalR from '@microsoft/signalr'

let connection: signalR.HubConnection | null = null

export const startSwapRequestsSignalRConnection = async (
  userId: string,
  onSwapRequestReceived: (payload: any) => void
) => {
  if (connection) return

  connection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7119/swapRequests', {
      withCredentials: true
    })
    .withAutomaticReconnect()
    .build()

  // Listen for backend event
  connection.on('ReceiveSwapRequest', payload => {
    onSwapRequestReceived(payload)
  })

  try {
    await connection.start()
    console.log('SwapRequests SignalR Connected')

    // Join user group so only relevant requests are pushed
    await connection.invoke('JoinUserGroup', userId)
  } catch (error) {
    console.error('SwapRequests SignalR connection error:', error)
  }
}

export const stopSwapRequestsSignalRConnection = async () => {
  if (connection) {
    await connection.stop()
    connection = null
  }
}
