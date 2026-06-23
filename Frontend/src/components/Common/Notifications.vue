<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useNotificationStore } from '../../stores/notificationStore'

const activeTab = ref('All')
const loading = ref(false)
const errorMessage = ref('')

const store = useNotificationStore()

const loadNotifications = async () => {
  loading.value = true
  errorMessage.value = ''
  try {
    await store.fetchNotifications()
  } catch (error: any) {
    errorMessage.value = error.response?.data?.message || error.message || 'Something went wrong'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadNotifications()
})

const filteredNotifications = computed(() => {
  if (activeTab.value === 'Unread') {
    return store.notifications.filter(n => !n.isRead)
  }
  return store.notifications
})
</script>


<template>
  <div class="notifications-page">

    <div class="notifications-toolbar">
      <div class="tabs">
        <span :class="{ active: activeTab === 'All' }" @click="activeTab = 'All'">
          All
        </span>

        <span :class="{ active: activeTab === 'Unread' }" @click="activeTab = 'Unread'">
          Unread
        </span>
      </div>

      <button class="mark-btn" @click="store.markAllAsRead" :disabled="store.unreadCount === 0">
        Mark All as Read
      </button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      Loading notifications...
    </div>

    <!-- Error -->
    <div v-else-if="errorMessage" class="error-state">
      {{ errorMessage }}
    </div>

    <!-- Table -->
    <div v-else class="table-card">
      <table class="notification-table">
        <thead>
          <tr>
            <th width="50"></th>
            <th>Message</th>
            <th>Date</th>
            <th>Status</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="notification in filteredNotifications" :key="notification.notificationId">
            <td>
              <input v-if="!notification.isRead" type="checkbox" @change="store.markAsRead(notification)" />
            </td>

            <td>
              <div>
                <strong>{{ notification.notificationTitle }}</strong>
              </div>
              <div>
                {{ notification.notificationMessage }}
              </div>
            </td>

            <td>
              {{ new Date(notification.createdAt).toLocaleString() }}
            </td>

            <td>
              <div class="status">
                <span class="dot" :class="notification.isRead ? 'read' : 'unread'" />
                {{ notification.isRead ? 'Read' : 'Unread' }}
              </div>
            </td>
          </tr>

          <tr v-if="filteredNotifications.length === 0">
            <td colspan="4" class="empty-state">
              No notifications found
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.notifications-page {
  height: calc(100vh - 170px);
  display: flex;
  flex-direction: column;
}

.notifications-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.tabs {
  display: flex;
  gap: 20px;
}

.tabs span {
  font-size: 14px;
  color: #64748b;
  cursor: pointer;
  padding-bottom: 6px;
}

.tabs .active {
  color: #232f72;
  font-weight: 600;
  border-bottom: 2px solid #f69d39;
}

.mark-btn {
  background: white;
  border: 1px solid #dbe2ea;
  border-radius: 8px;
  padding: 8px 14px;
  font-size: 13px;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.2s ease;
}

.mark-btn:hover {
  border-color: #f69d39;
  color: #232f72;
}

.table-card {
  flex: 1;
  overflow-y: auto;
  overflow-x: auto;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  box-shadow:
    0 4px 10px rgba(35, 47, 114, 0.04);
}

.notification-table {
  width: 100%;
  border-collapse: collapse;
}

.notification-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: #f8fafc;
}

.notification-table th {
  text-align: left;
  padding: 14px 18px;
  font-size: 13px;
  font-weight: 600;
  color: #475569;
}

.notification-table td {
  padding: 16px 18px;
  border-top: 1px solid #f1f5f9;
  font-size: 14px;
  color: #334155;
}

.notification-table tbody tr:hover {
  background: #fafbfc;
}

.status {
  display: flex;
  align-items: center;
  gap: 8px;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.dot.unread {
  background: #ef4444;
}

.dot.read {
  background: #60a5fa;
}

.toolbar-actions {
  display: flex;
  gap: 10px;
}

input[type='checkbox'] {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.loading-state,
.error-state,
.empty-state {
  padding: 24px;
  text-align: center;
  font-size: 14px;
}

.error-state {
  color: red;
}

.mark-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

@media (max-width: 900px) {
  .notifications-toolbar {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .table-card {
    overflow-x: auto;
  }

  .notification-table {
    min-width: 700px;
  }
}
</style>