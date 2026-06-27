import { defineStore } from "pinia";
import { ref } from "vue";
import API from "../api/axios";

export const useSupervisorSwapRequestsStore = defineStore(
  "supervisorSwapRequests",
  () => {
    const supervisorRequests = ref<any[]>([]);

    const fetchSupervisorRequests = async () => {
      try {
        const response = await API.get("/Supervisor/SwapRequests/my");

        supervisorRequests.value = response.data.data || [];
        console.log("Supervisor Requests:", supervisorRequests.value);
      } catch (error) {
        console.error("Error fetching supervisor requests:", error);
      }
    };

    const approveRequest = async (requestId: number) => {
      try {
        await API.put(`/Supervisor/SwapRequests/${requestId}/approve`, {},);
      } catch (error) {
        console.error(error);
      }
    };

    const rejectRequest = async (requestId: number) => {
      try {
        await API.put(`/Supervisor/SwapRequests/${requestId}/reject`, {},);
      } catch (error) {
        console.error(error);
      }
    };

    return {
      supervisorRequests,
      fetchSupervisorRequests,
      approveRequest,
      rejectRequest,
    };
  },
);
