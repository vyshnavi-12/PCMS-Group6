import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export const usePhysicianUnavailableRequestsStore = defineStore('physicianUnavailableRequests', () => {
    const openUnavailableCount = ref<number>(0)

    const fetchOpenUnavailableCount = async (physicianId: number) => {
        try {
            const response = await axios.get(
                `https://localhost:7119/api/Physician/UnavailableRequests/open-count/${physicianId}`,
                { withCredentials: true }
            )
            openUnavailableCount.value = response.data.data || 0
        } catch (error) {
            console.error('Error fetching unavailable requests count:', error)
        }
    }

    return {
        openUnavailableCount,
        fetchOpenUnavailableCount
    }
})
