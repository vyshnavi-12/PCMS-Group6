import { defineStore } from "pinia";
import { ref } from "vue";
import API from "../api/axios";

export const useDoctorSwapRequestsStore = defineStore(
  "doctorSwapRequests",
  () => {
    const myRequests = ref<any[]>([]);
    const requestsToMe = ref<any[]>([]);

    const fetchDoctorRequests = async () => {
      try {
        const [myResponse, toMeResponse] = await Promise.all([
          API.get("/Physician/SwapRequests/my"),
          API.get("/Physician/SwapRequests/to-me"),
        ]);

        myRequests.value = myResponse.data.data || [];
        requestsToMe.value = toMeResponse.data.data || [];

        console.log("My Requests:", myRequests.value);
        console.log("Requests To Me:", requestsToMe.value);
      } catch (error) {
        console.error("Error fetching doctor swap requests:", error);
      }
    };

    const acceptRequest = async (requestId: number) => {
      try {
        await API.put(`/Physician/SwapRequests/${requestId}/accept`, {},);
      } catch (error) {
        console.error(error);
      }
    };

    const declineRequest = async (requestId: number) => {
      try {
        await API.put(`/Physician/SwapRequests/${requestId}/decline`, {},);
      } catch (error) {
        console.error(error);
      }
    };

    return {
      myRequests,
      requestsToMe,
      fetchDoctorRequests,
      acceptRequest,
      declineRequest,
    };
  },
);
