import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export const useDoctorSwapRequestsStore = defineStore('doctorSwapRequests', () => {
    const myRequests = ref<any[]>([])
    const requestsToMe = ref<any[]>([])
    const pendingMyRequestsCount = ref<number>(0)
    const pendingRequestsToMeCount = ref<number>(0)

    const fetchDoctorRequests = async () => {
        try {
            const [myResponse, toMeResponse] = await Promise.all([
                axios.get('https://localhost:7119/api/Physician/SwapRequests/my', { withCredentials: true }),
                axios.get('https://localhost:7119/api/Physician/SwapRequests/to-me', { withCredentials: true })
            ])
            myRequests.value = myResponse.data.data || []
            requestsToMe.value = toMeResponse.data.data || []
        } catch (error) {
            console.error('Error fetching doctor swap requests:', error)
        }
    }

    const fetchPendingMyRequestsCount = async () => {
        const res = await axios.get('https://localhost:7119/api/Physician/SwapRequests/pending-my-count', { withCredentials: true })
        pendingMyRequestsCount.value = res.data.data || 0
    }

    const fetchPendingRequestsToMeCount = async () => {
        const res = await axios.get('https://localhost:7119/api/Physician/SwapRequests/pending-to-me-count', { withCredentials: true })
        pendingRequestsToMeCount.value = res.data.data || 0
    }

    const acceptRequest = async (requestId: number) => {
        await axios.put(`https://localhost:7119/api/Physician/SwapRequests/${requestId}/accept`, {}, { withCredentials: true })
    }

    const declineRequest = async (requestId: number) => {
        await axios.put(`https://localhost:7119/api/Physician/SwapRequests/${requestId}/decline`, {}, { withCredentials: true })
    }

    return {
        myRequests,
        requestsToMe,
        pendingMyRequestsCount,
        pendingRequestsToMeCount,
        fetchDoctorRequests,
        fetchPendingMyRequestsCount,
        fetchPendingRequestsToMeCount,
        acceptRequest,
        declineRequest
    }
})
