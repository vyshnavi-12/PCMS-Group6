import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export const useSupervisorSwapRequestsStore = defineStore('supervisorSwapRequests', () => {
    const supervisorRequests = ref<any[]>([])

    const fetchSupervisorRequests = async () => {
        try {
            const response = await axios.get(
                'https://localhost:7119/api/Supervisor/SwapRequests/my',
                { withCredentials: true }
            )

            supervisorRequests.value = response.data.data || []
            console.log('Supervisor Requests:', supervisorRequests.value)
        } catch (error) {
            console.error('Error fetching supervisor requests:', error)
        }
    }

    const approveRequest = async (requestId: number) => {
        try {
            await axios.put(
                `https://localhost:7119/api/Supervisor/SwapRequests/${requestId}/approve`,
                {},
                { withCredentials: true }
            )
        } catch (error) {
            console.error(error)
        }
    }

    const rejectRequest = async (requestId: number) => {
        try {
            await axios.put(
                `https://localhost:7119/api/Supervisor/SwapRequests/${requestId}/reject`,
                {},
                { withCredentials: true }
            )
        } catch (error) {
            console.error(error)
        }
    }

    return {
        supervisorRequests,
        fetchSupervisorRequests,
        approveRequest,
        rejectRequest
    }
})