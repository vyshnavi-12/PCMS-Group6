import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

export const useDoctorSwapRequestsStore = defineStore('doctorSwapRequests', () => {
    const myRequests = ref<any[]>([])
    const requestsToMe = ref<any[]>([])

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

    return {
        myRequests,
        requestsToMe,
        fetchDoctorRequests,
        acceptRequest,
        declineRequest
    }
})