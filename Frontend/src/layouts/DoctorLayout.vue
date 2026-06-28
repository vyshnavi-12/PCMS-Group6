<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { RouterView, useRoute } from 'vue-router'
import { useToast } from 'primevue/usetoast'

import Sidebar from '../components/Common/DoctorSideBar.vue'
import AppHeader from '../components/Common/AppHeader.vue'

import { useNotificationStore } from '../stores/notificationStore'
import { useScheduleStore } from '../stores/scheduleStore'
import { useDoctorSwapRequestsStore } from '../stores/doctorSwapRequestsStore'

import { startNotificationSignalRConnection } from '../services/notificationSignalRService'
import { startScheduleSignalRConnection } from '../services/scheduleSignalRService'
import doctorSwapSignalRService from '../services/doctorSwapSignalRService'
import unavailableRequestSignalRService from '../services/unavailableRequestSignalRService'

const route = useRoute()
const toast = useToast()

const notificationStore = useNotificationStore()
const scheduleStore = useScheduleStore()
const swapRequestsStore = useDoctorSwapRequestsStore()

onMounted(async () => {
  const user = localStorage.getItem('loggedInUser')
  if (!user) return

  const parsedUser = JSON.parse(user)

  await startNotificationSignalRConnection(
    parsedUser.userId.toString(),
    (payload) => {
      notificationStore.addNotification({
        notificationId: payload.notificationId,
        userId: parsedUser.userId,
        notificationTitle: payload.title,
        notificationMessage: payload.message,
        isRead: false,
        createdAt: payload.createdAt,
        readAt: null
      })

      toast.add({
        severity: 'info',
        summary: payload.title,
        detail: payload.message,
        life: 3000
      })
    }
  )

  await startScheduleSignalRConnection(
    parsedUser.userId.toString(),
    async () => {
      await scheduleStore.fetchDoctorSchedules()

      toast.add({
        severity: 'success',
        summary: 'Schedule Updated',
        detail: 'Your schedule has been updated',
        life: 3000
      })
    }
  )

  await doctorSwapSignalRService.startConnection(
    parsedUser.userId.toString()
  )

  doctorSwapSignalRService.onRefreshDoctorRequests(async () => {
    console.log('Doctor swap refresh received')

    await Promise.all([
      swapRequestsStore.fetchDoctorRequests(),
      scheduleStore.fetchDoctorSchedules()
    ])
  })

  await unavailableRequestSignalRService.startConnection(
    parsedUser.userId.toString()
  )
  unavailableRequestSignalRService.onUnavailableRequestUpdated(async () => {
    console.log('Unavailable request updated')

    await scheduleStore.fetchDoctorSchedules()

  })
})

const pageTitle = computed(() => {
  switch (route.path) {
    case '/doctor/dashboard':
      return 'Dashboard'
    case '/doctor/notifications':
      return 'Notifications'
    case '/doctor/schedule':
      return 'My Schedule'
    case '/doctor/swap-requests':
      return 'Swap Requests'
    case '/doctor/profile':
      return 'Profile'
    default:
      return 'Dashboard'
  }
})
</script>

<template>
  <div class="doctor-layout">

    <Toast position="bottom-right" />

    <Sidebar />

    <main class="main-content">

      <AppHeader :pageTitle="pageTitle" />

      <section class="content-area">
        <RouterView />
      </section>

    </main>

  </div>
</template>

<style scoped>
.doctor-layout {
  display: flex;
  min-height: 100vh;
  background: #f8fafc;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.content-area {
  flex: 1;
  padding: 20px 20px 18px 20px;
  overflow: hidden;
}
</style>