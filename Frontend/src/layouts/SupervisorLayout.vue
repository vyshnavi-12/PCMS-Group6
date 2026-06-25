<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useToast } from 'primevue/usetoast'

import SupervisorSideBar from '../components/Common/SupervisorSideBar.vue'
import AppHeader from '../components/Common/AppHeader.vue'

import { useNotificationStore } from '../stores/notificationStore'
import { startNotificationSignalRConnection } from '../services/notificationSignalRService.ts'

const route = useRoute()
const toast = useToast()
const store = useNotificationStore()

onMounted(async () => {
  const user = localStorage.getItem('loggedInUser')
  if (!user) return

  const parsedUser = JSON.parse(user)

  await startNotificationSignalRConnection(
    parsedUser.userId.toString(),
    (payload) => {
      store.addNotification({
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
})

const pageTitle = computed(() => {
  switch (route.path) {
    case '/supervisor/dashboard':
      return 'Dashboard'
    case '/supervisor/schedules':
      return 'Schedules'
    case '/supervisor/notifications':
      return 'Notifications'
    case '/supervisor/profile':
      return 'Profile'
    case '/supervisor/coverage-schedule':
      return 'Coverage Schedule'
    case '/supervisor/swap-requests':
      return 'Swap Requests'
    default:
      return 'Dashboard'
  }
})
</script>

<template>
  <div class="layout">

    <Toast position="bottom-right" />

    <SupervisorSideBar />

    <main class="main-content">

      <AppHeader :pageTitle="pageTitle" />

      <section class="content-area">
        <RouterView />
      </section>

    </main>

  </div>
</template>

<style scoped>
.layout {
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
    padding: 24px;
    overflow: hidden;
}
</style>