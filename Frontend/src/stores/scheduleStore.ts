import { defineStore } from "pinia";
import { ref } from "vue";
import API from "../api/axios";

export const useScheduleStore = defineStore("schedule", () => {
  const schedules = ref<any[]>([]);
  const doctorSchedules = ref<any[]>([]);

  const loading = ref(false);
  const errorMessage = ref("");

  // ------------------------
  // Supervisor Schedules
  // ------------------------
  const fetchSchedules = async () => {
    loading.value = true;
    errorMessage.value = "";

    try {
      const response = await API.get("/CoverageSchedules");
      schedules.value = response.data.data || [];
    } catch (error: any) {
      errorMessage.value =
        error.response?.data?.message || "Failed to load schedules";
      console.error(error);
    } finally {
      loading.value = false;
    }
  };

  const generateSchedule = async () => {
    try {
      await API.post("/CoverageSchedules/generate");
      await fetchSchedules();
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  const publishSchedule = async (scheduleId: number) => {
    try {
      await API.post(`/CoverageSchedules/${scheduleId}/publish`);
      await fetchSchedules();
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  const updateAssignments = async (
  scheduleId: number,
 assignments: {
    coverageAssignmentId: number
    physicianId: number
  }[]
) => {
  try {
    await API.patch(
      `/CoverageSchedules/${scheduleId}/assignments`,
      {
        assignments
      }
    )
  } catch (error) {
    console.error(error)
    throw error
  }
}

  const fetchScheduleById = async (scheduleId: number) => {
    try {
      const response = await API.get(`/CoverageSchedules/${scheduleId}`);
      return response.data.data;
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  // ------------------------
  // Doctor Schedule (Used by both Dashboard + MySchedule)
  // ------------------------
  const fetchDoctorSchedules = async () => {
    loading.value = true;
    errorMessage.value = "";

    try {
      const response = await API.get("/MySchedule");
      doctorSchedules.value = response.data.data || [];
    } catch (error: any) {
      errorMessage.value =
        error.response?.data?.message || "Failed to load doctor schedules";
      console.error(error);
    } finally {
      loading.value = false;
    }
  };

  return {
    schedules,
    doctorSchedules,
    loading,
    errorMessage,

    fetchSchedules,
    generateSchedule,
    publishSchedule,
    updateAssignments,
    fetchScheduleById,

    fetchDoctorSchedules,
  };
});
