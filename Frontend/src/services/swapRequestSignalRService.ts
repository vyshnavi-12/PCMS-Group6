import * as signalR from '@microsoft/signalr'

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL;

class SwapRequestSignalRService {
  private connection: signalR.HubConnection | null = null

  async startConnection(userId: string | number) {
    if (this.connection) {
      console.log('SwapRequest SignalR already connected')
      return
    }

    console.log('Starting Swap SignalR for:', userId)

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${SIGNALR_BASE_URL}/swapRequests`, {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build()

    try {
      await this.connection.start()
      console.log('SwapRequest SignalR connected')

      await this.connection.invoke(
        'JoinUserGroup',
        userId.toString()
      )

      console.log(`Joined group: User_${userId}`)
    } catch (error) {
      console.error('SwapRequest SignalR connection failed:', error)
    }
  }

  onRefreshDoctorRequests(callback: () => void) {
    if (!this.connection) {
      console.warn('SwapRequest SignalR not connected')
      return
    }

    this.connection.off('RefreshSwapRequests')

    this.connection.on('RefreshSwapRequests', () => {
      console.log('RefreshSwapRequests event received')
      callback()
    })
  }

  onRefreshSupervisorRequests(callback: () => void) {
    if (!this.connection) {
      console.warn('SwapRequest SignalR not connected')
      return
    }

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

      console.log('SwapRequest SignalR disconnected')
    } catch (error) {
      console.error(error)
    }
  }

  getConnection() {
    return this.connection
  }
}

export default new SwapRequestSignalRService()