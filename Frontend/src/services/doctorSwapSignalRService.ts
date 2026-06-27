import * as signalR from "@microsoft/signalr";

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL;

class DoctorSwapSignalRService {
  private connection: signalR.HubConnection | null = null;

  async startConnection(userId: string | number) {
    if (this.connection) {
      console.log("Doctor Swap SignalR already connected");
      return;
    }

    console.log("Starting Doctor Swap SignalR for:", userId);

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${SIGNALR_BASE_URL}/swapRequests`, {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log("Doctor Swap SignalR connected");

      await this.connection.invoke("JoinUserGroup", userId.toString());

      console.log(`Doctor joined group: User_${userId}`);
    } catch (error) {
      console.error("Doctor Swap SignalR connection failed:", error);
    }
  }

  onRefreshDoctorRequests(callback: () => void) {
    if (!this.connection) return;

    this.connection.off("RefreshSwapRequests");

    this.connection.on("RefreshSwapRequests", () => {
      console.log("RefreshSwapRequests event received");
      callback();
    });
  }

  async stopConnection(userId: string | number) {
    if (!this.connection) return;

    try {
      await this.connection.invoke("LeaveUserGroup", userId.toString());

      await this.connection.stop();
      this.connection = null;

      console.log("Doctor Swap SignalR disconnected");
    } catch (error) {
      console.error(error);
    }
  }
}

export default new DoctorSwapSignalRService();
