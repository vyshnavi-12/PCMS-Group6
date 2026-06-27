import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export const useSupervisorSwapRequestsStore = defineStore('supervisorSwapRequests', () => {
    const supervisorRequests = ref<any[]>([])
    const targetAcceptedCount = ref<number>(0)

    const fetchSupervisorRequests = async () => {
        try {
            const response = await axios.get(
                'https://localhost:7119/api/Supervisor/SwapRequests/my',
                { withCredentials: true }
            )
            supervisorRequests.value = response.data.data || []
        } catch (error) {
            console.error('Error fetching supervisor requests:', error)
        }
    }

    const fetchTargetAcceptedCount = async () => {
        try {
            const response = await axios.get(
                'https://localhost:7119/api/Supervisor/SwapRequests/target-accepted-count',
                { withCredentials: true }
            )
            targetAcceptedCount.value = response.data.data || 0
        } catch (error) {
            console.error('Error fetching target accepted count:', error)
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
        targetAcceptedCount,
        fetchSupervisorRequests,
        fetchTargetAcceptedCount,
        approveRequest,
        rejectRequest
    }
})
