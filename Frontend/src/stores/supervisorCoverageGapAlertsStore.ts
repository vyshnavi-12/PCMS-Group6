import { defineStore } from "pinia";
import { ref } from "vue";
import API from "../api/axios";

export const useSupervisorCoverageGapAlertsStore = defineStore(
  "supervisorCoverageGapAlerts",
  () => {
    const openAlertsCount = ref<number>(0);

    const fetchOpenAlertsCount = async () => {
      try {
        const response = await API.get(
          "/Supervisor/CoverageGapAlerts/open-count",
        );
        // ✅ unwrap Result<int>
        openAlertsCount.value = response.data.data || 0;
      } catch (error) {
        console.error("Error fetching coverage gap alerts count:", error);
        openAlertsCount.value = 0;
      }
    };

    return {
      openAlertsCount,
      fetchOpenAlertsCount,
    };
  },
);
