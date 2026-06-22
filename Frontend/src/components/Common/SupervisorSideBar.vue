<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import Button from 'primevue/button'
import { logoutUser } from '../../services/authService'

const route = useRoute()
const router = useRouter()

const isSidebarCollapsed = ref(false)

const toggleSidebar = () => {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
}

const handleLogout = async () => {
  try {
    await logoutUser()
  } catch (error) {
    console.error("Logout failed:", error)
  } finally {
    localStorage.removeItem("loggedInUser")
    router.push("/login")
  }
}
</script>

<template>
  <aside class="sidebar" :class="{ collapsed: isSidebarCollapsed }">
    <div class="sidebar-header">

      <div class="logo-section">
        <div class="logo-icon">
          <i class="pi pi-shield"></i>
        </div>

        <span v-if="!isSidebarCollapsed" class="logo-text">
          PCMS
        </span>
      </div>

      <Button text rounded class="toggle-btn" @click="toggleSidebar">
        <i class="pi" :class="isSidebarCollapsed
          ? 'pi-angle-right'
          : 'pi-angle-left'
          " />
      </Button>

    </div>

    <nav class="sidebar-nav">

      <RouterLink to="/supervisor/dashboard" class="nav-item"
        :class="{ active: route.path === '/supervisor/dashboard' }">
        <i class="pi pi-home"></i>

        <span v-if="!isSidebarCollapsed">
          Dashboard
        </span>
      </RouterLink>

      <RouterLink to="/supervisor/schedules" class="nav-item"
        :class="{ active: route.path === '/supervisor/schedules' }">
        <i class="pi pi-list-check"></i>

        <span v-if="!isSidebarCollapsed">
          Schedules
        </span>
      </RouterLink>

      <RouterLink to="/supervisor/coverage-schedule" class="nav-item"
        :class="{ active: route.path === '/supervisor/coverage-schedule' }">
        <i class="pi pi-calendar"></i>

        <span v-if="!isSidebarCollapsed">
          Coverage Schedule
        </span>
      </RouterLink>

      <RouterLink to="/supervisor/coverage-gaps" class="nav-item"
        :class="{ active: route.path === '/supervisor/coverage-gaps' }">
        <i class="pi pi-exclamation-triangle"></i>

        <span v-if="!isSidebarCollapsed">
          Coverage Gap
        </span>
      </RouterLink>

      <RouterLink to="/supervisor/swap-requests" class="nav-item"
        :class="{ active: route.path === '/supervisor/swap-requests' }">
        <i class="pi pi-arrow-right-arrow-left"></i>

        <span v-if="!isSidebarCollapsed">
          Swap Requests
        </span>
      </RouterLink>

      <RouterLink to="/supervisor/notifications" class="nav-item"
        :class="{ active: route.path === '/supervisor/notifications' }">
        <i class="pi pi-bell"></i>

        <span v-if="!isSidebarCollapsed">
          Notifications
        </span>
      </RouterLink>

      <RouterLink to="/supervisor/profile" class="nav-item" :class="{ active: route.path === '/supervisor/profile' }">
        <i class="pi pi-user"></i>

        <span v-if="!isSidebarCollapsed">
          Profile
        </span>
      </RouterLink>

    </nav>

    <div class="sidebar-footer">

      <div class="nav-item logout" @click="handleLogout">
        <i class="pi pi-sign-out"></i>

        <span v-if="!isSidebarCollapsed">
          Logout
        </span>
      </div>

    </div>

  </aside>
</template>

<style scoped>
.sidebar {
  width: 240px;
  background: linear-gradient(180deg,
      #232f72 0%,
      #16204d 100%);

  color: white;
  display: flex;
  flex-direction: column;
  transition: width 0.25s ease;
}

.sidebar.collapsed {
  width: 78px;
}

.sidebar-header {
  height: 72px;
  padding: 0 16px;

  display: flex;
  align-items: center;
  justify-content: space-between;
}

.logo-section {
  display: flex;
  align-items: center;
  gap: 10px;
}

.logo-icon {
  width: 38px;
  height: 38px;

  border-radius: 10px;
  background: rgba(255, 255, 255, 0.12);

  display: flex;
  justify-content: center;
  align-items: center;

  font-size: 18px;
}

.logo-text {
  font-size: 24px;
  font-weight: 700;
}

:deep(.toggle-btn.p-button) {
  color: white;
  background: transparent;
  border: none;
  box-shadow: none;
}

:deep(.toggle-btn.p-button:hover) {
  color: #f69d39;
  background: transparent;
  border: none;
  box-shadow: none;
}

:deep(.toggle-btn.p-button:focus) {
  box-shadow: none;
}

.sidebar-nav {
  padding: 8px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.nav-item {
  height: 44px;

  display: flex;
  align-items: center;
  gap: 12px;

  padding: 0 14px;

  border-radius: 10px;

  color: white;
  text-decoration: none;

  transition: all 0.2s ease;
}

.nav-item:hover {
  background: rgba(246, 157, 57, 0.15);
}

.nav-item.active {
  background: #f89341;
}

.sidebar.collapsed .nav-item {
  justify-content: center;
  padding: 0;
}

.sidebar-footer {
  margin-top: auto;
  padding: 8px;
}

.logout:hover {
  background: rgba(246, 157, 57, 0.15);
}
</style>