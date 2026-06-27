<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { getMe } from '../../services/authService'
import { useNotificationStore } from '../../stores/notificationStore'

defineProps<{ pageTitle: string }>()

const router = useRouter()
const notificationStore = useNotificationStore()

const loggedInUser = ref({ fullName: '', role: '', specialtyName: '' })

const unreadNotifications = computed(() => notificationStore.unreadCount)

const fetchUser = async () => {
  try {
    const response = await getMe()
    loggedInUser.value = response.data
  } catch (error) {
    console.error('Failed to fetch user:', error)
  }
}

const colors = [
  '#BFDBFE',
  '#DDD6FE',
  '#FBCFE8',
  '#BBF7D0',
  '#FED7AA',
  '#A5F3FC',
  '#FDE68A'
]

const userInitials = computed(() => {
  if (!loggedInUser.value.fullName) return ''

  const names = loggedInUser.value.fullName.trim().split(' ')

  const firstInitial = names[0]?.charAt(0) || ''
  const lastInitial =
    names.length > 1
      ? names[names.length - 1]?.charAt(0)
      : ''

  return (firstInitial + lastInitial).toUpperCase()
})

const avatarColor = computed(() => {
  if (!loggedInUser.value.fullName) return '#BFDBFE'

  const index = loggedInUser.value.fullName.length % colors.length
  return colors[index]
})

onMounted(() => {
  fetchUser()
  notificationStore.fetchNotifications()
})

const openNotifications = () => {
  if (loggedInUser.value.role === 'Physician') router.push('/doctor/notifications')
  else if (loggedInUser.value.role === 'Supervisor') router.push('/supervisor/notifications')
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

        <div class="avatar" :style="{ backgroundColor: avatarColor }">
          {{ userInitials }}
        </div>

        <div class="profile-info">

          <span class="profile-name">
            {{ loggedInUser.fullName }}
          </span>

          <span class="profile-role">
            {{ loggedInUser.specialtyName || loggedInUser.role }}
          </span>

        </div>

      </div>

    </div>

  </header>
</template>

<style scoped>
.top-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  background: white;
  border-bottom: 1px solid #e5e7eb;
}

.top-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #232f72;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 24px;
}

.notification-wrapper {
  position: relative;
  cursor: pointer;
}

.notification-icon {
  font-size: 24px;
  color: #374151;
}

.notification-badge {
  position: absolute;
  top: -8px;
  right: -10px;
  background: red;
  color: white;
  font-size: 11px;
  min-width: 18px;
  height: 18px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.profile-section {
  display: flex;
  align-items: center;
  gap: 12px;
}

.avatar {
  width: 42px;
  height: 42px;
  border-radius: 50%;

  display: flex;
  align-items: center;
  justify-content: center;

  font-size: 14px;
  font-weight: 700;
  color: #1e293b;

  flex-shrink: 0;
}

.profile-info {
  display: flex;
  flex-direction: column;
}

.profile-name {
  font-size: 14px;
  font-weight: 600;
  color: #111827;
}

.profile-role {
  font-size: 12px;
  color: #6b7280;
}
</style>