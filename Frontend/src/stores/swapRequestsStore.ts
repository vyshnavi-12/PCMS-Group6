// src/stores/swapRequestsStore.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7119',
  withCredentials: true,
})

interface SwapRequest {
  swapRequestId: number
  date: string
  shift: string
  requestedBy?: string
  requestedWith?: string
  reason: string
  requestedOn: string
  status: string
}

export const useSwapRequestsStore = defineStore('swapRequests', () => {
  // 🔹 State
  const myRequests = ref<SwapRequest[]>([])
  const requestsToMe = ref<SwapRequest[]>([])
  const loading = ref(false)
  const errorMessage = ref('')

  // 🔹 Actions
  const fetchSwapRequests = async () => {
    loading.value = true
    errorMessage.value = ''
    try {
      const myResponse = await api.get('/api/Physician/SwapRequests/my')
      myRequests.value = myResponse.data.data || []

      const toMeResponse = await api.get('/api/Physician/SwapRequests/to-me')
      requestsToMe.value = toMeResponse.data.data || []
    } catch (error: any) {
      errorMessage.value =
        error.response?.data?.message ||
        error.message ||
        'Failed to load swap requests'
    } finally {
      loading.value = false
    }
  }

  const addRequest = (request: SwapRequest) => {
    // Called when SignalR pushes a new request
    requestsToMe.value.unshift(request)
  }

  const acceptRequest = async (requestId: number) => {
    await api.put(`/api/Physician/SwapRequests/${requestId}/accept`)
    await fetchSwapRequests()
  }

  const declineRequest = async (requestId: number) => {
    await api.put(`/api/Physician/SwapRequests/${requestId}/decline`)
    await fetchSwapRequests()
  }

  return {
    myRequests,
    requestsToMe,
    loading,
    errorMessage,
    fetchSwapRequests,
    addRequest,
    acceptRequest,
    declineRequest,
  }
})
