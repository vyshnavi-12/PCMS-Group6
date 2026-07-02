import * as signalR from '@microsoft/signalr'

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL;

class SupervisorSwapSignalRService {
  private connection: signalR.HubConnection | null = null

  async startConnection(userId: string | number) {
    if (this.connection) return

    this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${SIGNALR_BASE_URL}/swapRequests`, {
            withCredentials: true
        })
        .withAutomaticReconnect()
        .build()

    this.connection.on("RefreshSupervisorSwapRequests", () => {
        console.log("RefreshSupervisorSwapRequests received")
        this.refreshCallback?.()
    })

    await this.connection.start()

    await this.connection.invoke(
        "JoinUserGroup",
        userId.toString()
    )
}

private refreshCallback?: () => void

onRefreshSupervisorRequests(callback: () => void) {
    this.refreshCallback = callback
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