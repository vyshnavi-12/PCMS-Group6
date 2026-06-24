<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

import SupervisorSideBar from '../components/Common/SupervisorSideBar.vue'
import AppHeader from '../components/Common/AppHeader.vue'

const route = useRoute()

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

    case '/supervisor/gap-alerts':
      return 'Unavailable Requests'

    case '/supervisor/swap-requests':
      return 'Swap Requests'

    default:
      return 'Dashboard'
  }
})
</script>

<template>
  <div class="layout">

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