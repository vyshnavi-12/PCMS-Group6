<script setup lang="ts">
import { ref, computed } from 'vue'

const activeTab = ref('All')

const notifications = ref([
    {
        message: 'New schedule published for May 19 – May 25, 2025.',
        date: 'May 18, 2025 11:00 AM',
        status: 'Unread'
    },
    {
        message: 'Swap request SWP-0012 has been approved.',
        date: 'May 18, 2025 02:30 PM',
        status: 'Unread'
    },
    {
        message: 'You are assigned to NIGHT shift on May 20, 2025.',
        date: 'May 18, 2025 09:00 AM',
        status: 'Read'
    },
    {
        message: 'Dr. Sarah Davis requested a swap for May 21, 2025 (DAY).',
        date: 'May 17, 2025 04:15 PM',
        status: 'Read'
    },
    {
        message: 'Coverage gap detected in Orthopedics (NIGHT) on May 22.',
        date: 'May 17, 2025 01:20 PM',
        status: 'Read'
    }
])

const filteredNotifications = computed(() => {
    if (activeTab.value === 'Unread') {
        return notifications.value.filter(
            notification => notification.status === 'Unread'
        )
    }

    return notifications.value
})

const markAsRead = (notification: any) => {
    notification.status = 'Read'
}

const markAllAsRead = () => {
    notifications.value.forEach(notification => {
        notification.status = 'Read'
    })
}
</script>

<template>
    <div class="notifications-page">

        <div class="notifications-toolbar">

            <div class="tabs">

                <span
                    :class="{ active: activeTab === 'All' }"
                    @click="activeTab = 'All'"
                >
                    All
                </span>

                <span
                    :class="{ active: activeTab === 'Unread' }"
                    @click="activeTab = 'Unread'"
                >
                    Unread
                </span>

            </div>

            <button
                class="mark-btn"
                @click="markAllAsRead"
            >
                Mark All as Read
            </button>

        </div>

        <div class="table-card">

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

                    <tr
                        v-for="(notification, index) in filteredNotifications"
                        :key="index"
                    >

                        <td>

                            <input
                                v-if="notification.status === 'Unread'"
                                type="checkbox"
                                @change="markAsRead(notification)"
                            />

                        </td>

                        <td>
                            {{ notification.message }}
                        </td>

                        <td>
                            {{ notification.date }}
                        </td>

                        <td>

                            <div class="status">

                                <span
                                    class="dot"
                                    :class="
                                        notification.status === 'Unread'
                                            ? 'unread'
                                            : 'read'
                                    "
                                />

                                {{ notification.status }}

                            </div>

                        </td>

                    </tr>

                    <tr v-if="filteredNotifications.length === 0">

                        <td
                            colspan="4"
                            class="empty-state"
                        >
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
  width: 100%;
}

/* Toolbar */

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

/* Button */

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

/* Card */

.table-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  overflow: hidden;

  box-shadow:
    0 4px 10px rgba(35, 47, 114, 0.04);
}

/* Table */

.notification-table {
  width: 100%;
  border-collapse: collapse;
}

.notification-table thead {
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

/* Status */

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

/* Mobile */

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