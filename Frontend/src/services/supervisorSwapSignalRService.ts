import * as signalR from '@microsoft/signalr'

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL;

class SupervisorSwapSignalRService {
  private connection: signalR.HubConnection | null = null

  async startConnection(userId: string | number) {
    if (this.connection) {
      console.log('Supervisor Swap SignalR already connected')
      return
    }

    console.log('Starting Supervisor Swap SignalR for:', userId)

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${SIGNALR_BASE_URL}/swapRequests`, {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build()

    try {
      await this.connection.start()
      console.log('Supervisor Swap SignalR connected')

      await this.connection.invoke(
        'JoinUserGroup',
        userId.toString()
      )

      console.log(`Supervisor joined group: User_${userId}`)
    } catch (error) {
      console.error('Supervisor Swap SignalR connection failed:', error)
    }
  }

  onRefreshSupervisorRequests(callback: () => void) {
    if (!this.connection) return

    this.connection.off('RefreshSupervisorSwapRequests')

    this.connection.on('RefreshSupervisorSwapRequests', () => {
      console.log('RefreshSupervisorSwapRequests event received')
      callback()
    })
  }

  async stopConnection(userId: string | number) {
    if (!this.connection) return

    try {
      await this.connection.invoke(
        'LeaveUserGroup',
        userId.toString()
      )

      await this.connection.stop()
      this.connection = null

      console.log('Supervisor Swap SignalR disconnected')
    } catch (error) {
      console.error(error)
    }
  }
}

export default new SupervisorSwapSignalRService()