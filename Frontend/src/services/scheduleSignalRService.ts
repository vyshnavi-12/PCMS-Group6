import * as signalR from "@microsoft/signalr";

const SIGNALR_BASE_URL = import.meta.env.VITE_SIGNALR_BASE_URL;

let connection: signalR.HubConnection | null = null;

export const startScheduleSignalRConnection = async (
  userId: string,
  onSchedulePublished: (payload: any) => void,
) => {
  if (connection) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${SIGNALR_BASE_URL}/scheduleHub`, {
      withCredentials: true,
    })
    .withAutomaticReconnect()
    .build();

  connection.on("SchedulePublished", (payload) => {
    onSchedulePublished(payload);
  });

  try {
    await connection.start();
    console.log("Schedule SignalR Connected");

    await connection.invoke("JoinUserGroup", userId);
  } catch (error) {
    console.error("Schedule SignalR connection error:", error);
  }
};

export const stopScheduleSignalRConnection = async () => {
  if (connection) {
    await connection.stop();
    connection = null;
  }
};
