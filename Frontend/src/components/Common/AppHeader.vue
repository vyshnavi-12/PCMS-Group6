<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

defineProps<{
  pageTitle: string
}>()

const router = useRouter()

const loggedInUser = JSON.parse(
  localStorage.getItem('loggedInUser') || '{}'
)

/*
  TEMPORARY MOCK COUNT
  Replace with API/Store later
*/
const notifications = [
  {
    message: 'New schedule published',
    status: 'Unread'
  },
  {
    message: 'Swap request approved',
    status: 'Unread'
  },
  {
    message: 'Coverage gap alert',
    status: 'Read'
  }
]

const unreadNotifications = computed(() =>
  notifications.filter(
    notification => notification.status === 'Unread'
  ).length
)

const openNotifications = () => {
  if (loggedInUser.role === 'Doctor') {
    router.push('/doctor/notifications')
  }
  else if (loggedInUser.role === 'Supervisor') {
    router.push('/supervisor/notifications')
  }
}
</script>

<template>
  <header class="top-header">

    <div>
      <h1>{{ pageTitle }}</h1>
    </div>

    <div class="header-actions">

      <div class="notification-wrapper" @click="openNotifications">
        <i class="pi pi-bell notification-icon"></i>

        <span v-if="unreadNotifications > 0" class="notification-badge">
          {{ unreadNotifications }}
        </span>
      </div>

      <div class="profile-section">

        <img :src="loggedInUser.profileImage" alt="Profile" class="avatar" />

        <div class="profile-info">

          <span class="profile-name">
            {{ loggedInUser.name }}
          </span>

          <span class="profile-role">
            {{ loggedInUser.department }}
          </span>

        </div>

      </div>

    </div>

  </header>
</template>

<style scoped>
.top-header {
  height: 72px;
  background: white;
  border-bottom: 1px solid #e2e8f0;

  padding: 0 24px;

  display: flex;
  justify-content: space-between;
  align-items: center;
}

.top-header h1 {
  margin: 0;
  font-size: 28px;
  font-weight: 700;
  color: #232f72;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 24px;
}

/* Notification */

.notification-wrapper {
  position: relative;
  cursor: pointer;
}

.notification-icon {
  font-size: 22px;
  color: #232f72;
}

.notification-badge {
  position: absolute;
  top: -7px;
  right: -7px;

  width: 18px;
  height: 18px;

  border-radius: 50%;

  background: #ef4444;
  color: white;

  display: flex;
  align-items: center;
  justify-content: center;

  font-size: 10px;
  font-weight: 600;
}

/* Profile */

.profile-section {
  display: flex;
  align-items: center;
  gap: 10px;
}

.avatar {
  width: 42px;
  height: 42px;
  border-radius: 50%;
}

.profile-info {
  display: flex;
  flex-direction: column;
}

.profile-name {
  font-size: 14px;
  font-weight: 600;
  color: #0f172a;
}

.profile-role {
  font-size: 12px;
  color: #64748b;
}
</style>