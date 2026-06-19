<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getMe } from '../../services/authService'

defineProps<{
  pageTitle: string
}>()

const router = useRouter()

const loggedInUser = ref({
  fullName: '',
  role: '',
  specialtyName: ''
})

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

const fetchUser = async () => {
  try {
    const response = await getMe()
    loggedInUser.value = response.data
  }
  catch (error) {
    console.error('Failed to fetch user:', error)
  }
}

onMounted(() => {
  fetchUser()
})

const openNotifications = () => {
  if (loggedInUser.value.role === 'Physician') {
    router.push('/doctor/notifications')
  }
  else if (loggedInUser.value.role === 'Supervisor') {
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

        <img
          src="https://i.pravatar.cc/200?img=12"
          alt="Profile"
          class="avatar"
        />

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
  color: #1f2937;
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
  object-fit: cover;
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