// stores/supervisorSwapRequestsStore.ts
import { defineStore } from "pinia";
import { ref } from "vue";
import API from "../api/axios";

export const useSupervisorSwapRequestsStore = defineStore("supervisorSwapRequests", () => {
  const supervisorRequests = ref<any[]>([]);
  const targetAcceptedCount = ref<number>(0);

  const fetchSupervisorRequests = async () => {
    try {
      const res = await API.get("/Supervisor/SwapRequests/my", { withCredentials: true });
      supervisorRequests.value = res.data.data || [];
    } catch (error) {
      console.error("Error fetching supervisor swap requests:", error);
    }
  };

  const fetchTargetAcceptedCount = async () => {
    try {
      const res = await API.get("/Supervisor/SwapRequests/target-accepted-count", { withCredentials: true });
      targetAcceptedCount.value = res.data.data || 0;
    } catch (error) {
      console.error("Error fetching target accepted count:", error);
    }
  };

  const approveRequest = async (requestId: number) => {
    await API.put(`/Supervisor/SwapRequests/${requestId}/approve`, {}, { withCredentials: true });
    await fetchSupervisorRequests();
    await fetchTargetAcceptedCount();
  };

  const rejectRequest = async (requestId: number) => {
    await API.put(`/Supervisor/SwapRequests/${requestId}/reject`, {}, { withCredentials: true });
    await fetchSupervisorRequests();
    await fetchTargetAcceptedCount();
  };

  return {
    supervisorRequests,
    targetAcceptedCount,
    fetchSupervisorRequests,
    fetchTargetAcceptedCount,
    approveRequest,
    rejectRequest,
  };
});
