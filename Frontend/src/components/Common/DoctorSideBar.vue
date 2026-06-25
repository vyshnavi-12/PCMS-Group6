<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import Button from 'primevue/button'
import { logoutUser } from '../../services/authService'

const route = useRoute()
const router = useRouter()

const isSidebarCollapsed = ref(true)

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

      <RouterLink to="/doctor/dashboard" class="nav-item" :class="{ active: route.path === '/doctor/dashboard' }">
        <i class="pi pi-home"></i>

        <span v-if="!isSidebarCollapsed">
          Dashboard
        </span>
      </RouterLink>

      <RouterLink to="/doctor/schedule" class="nav-item" :class="{ active: route.path === '/doctor/schedule' }">
        <i class="pi pi-calendar"></i>

        <span v-if="!isSidebarCollapsed">
          My Schedule
        </span>
      </RouterLink>

      <RouterLink to="/doctor/swap-requests" class="nav-item"
        :class="{ active: route.path === '/doctor/swap-requests' }">
        <i class="pi pi-arrow-right-arrow-left"></i>

        <span v-if="!isSidebarCollapsed">
          Swap Requests
        </span>
      </RouterLink>

      <RouterLink to="/doctor/notifications" class="nav-item"
        :class="{ active: route.path === '/doctor/notifications' }">
        <i class="pi pi-bell"></i>

        <span v-if="!isSidebarCollapsed">
          Notifications
        </span>
      </RouterLink>

      <RouterLink to="/doctor/profile" class="nav-item" :class="{ active: route.path === '/doctor/profile' }">
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
  min-height: 100vh;

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

/* =========================
   HEADER
========================= */

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
  align-items: center;
  justify-content: center;

  font-size: 18px;
}

.logo-text {
  font-size: 24px;
  font-weight: 700;
  letter-spacing: 0.5px;
}

/* =========================
   TOGGLE BUTTON
========================= */

:deep(.toggle-btn.p-button) {
  width: 32px;
  height: 32px;

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
  background: transparent;
}

:deep(.toggle-btn.p-button:active) {
  background: transparent;
}

/* =========================
   NAVIGATION
========================= */

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

.nav-item i {
  font-size: 15px;
  min-width: 18px;
}

.nav-item span {
  font-size: 14px;
  font-weight: 500;
}

.nav-item:hover {
  background: rgba(246, 157, 57, 0.15);
}

.nav-item.active {
  background: #f89341;
  color: white;
}

.sidebar.collapsed .nav-item {
  justify-content: center;
  padding: 0;
}

.sidebar.collapsed .nav-item i {
  margin: 0;
}

/* =========================
   FOOTER
========================= */

.sidebar-footer {
  margin-top: auto;
  padding: 8px;
}

.logout:hover {
  background: rgba(246, 157, 57, 0.15);
}

/* =========================
   SCROLLBAR
========================= */

.sidebar::-webkit-scrollbar {
  width: 6px;
}

.sidebar::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 20px;
}

.sidebar::-webkit-scrollbar-track {
  background: transparent;
}

/* =========================
   MOBILE
========================= */

@media (max-width: 768px) {
  .sidebar {
    width: 78px;
  }

  .logo-text,
  .nav-item span {
    display: none;
  }

  .nav-item {
    justify-content: center;
    padding: 0;
  }
}
</style>