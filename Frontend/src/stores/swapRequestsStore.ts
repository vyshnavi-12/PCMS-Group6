import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export const useSwapRequestsStore = defineStore('swapRequests', () => {
    const myRequests = ref<any[]>([])
    const requestsToMe = ref<any[]>([])
    const supervisorRequests = ref<any[]>([])

    const fetchDoctorRequests = async () => {
        try {
            const [myResponse, toMeResponse] = await Promise.all([
                axios.get(
                    'https://localhost:7119/api/Physician/SwapRequests/my',
                    { withCredentials: true }
                ),
                axios.get(
                    'https://localhost:7119/api/Physician/SwapRequests/to-me',
                    { withCredentials: true }
                )
            ])

            myRequests.value = myResponse.data.data || []
            requestsToMe.value = toMeResponse.data.data || []

            console.log('My Requests:', myRequests.value)
            console.log('Requests To Me:', requestsToMe.value)
        } catch (error) {
            console.error('Error fetching doctor swap requests:', error)
        }
    }

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

    const acceptRequest = async (requestId: number) => {
        try {
            await axios.put(
                `https://localhost:7119/api/Physician/SwapRequests/${requestId}/accept`,
                {},
                { withCredentials: true }
            )
        } catch (error) {
            console.error(error)
        }
    }

    const declineRequest = async (requestId: number) => {
        try {
            await axios.put(
                `https://localhost:7119/api/Physician/SwapRequests/${requestId}/decline`,
                {},
                { withCredentials: true }
            )
        } catch (error) {
            console.error(error)
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
        myRequests,
        requestsToMe,
        supervisorRequests,

        fetchDoctorRequests,
        fetchSupervisorRequests,

        acceptRequest,
        declineRequest,
        approveRequest,
        rejectRequest
    }
})