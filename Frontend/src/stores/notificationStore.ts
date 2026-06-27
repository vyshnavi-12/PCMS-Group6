// stores/notificationStore.ts
import { defineStore } from "pinia";
import { ref, computed } from "vue";
import API from "../api/axios";

interface Notification {
  notificationId: number;
  userId: number;
  notificationTitle: string;
  notificationMessage: string;
  isRead: boolean;
  createdAt: string;
  readAt: string | null;
}

export const useNotificationStore = defineStore("notifications", () => {
  const notifications = ref<Notification[]>([])
  const loading = ref(false)
  const errorMessage = ref("")

  const unreadCount = computed(
    () => notifications.value.filter((n) => !n.isRead).length
  )

  const addNotification = (notification: Notification) => {
    notifications.value.unshift(notification)
  }

  const fetchNotifications = async () => {
    try {
      const response = await API.get("/notifications")
      notifications.value = response.data.data || []
    } catch (error) {
      throw error
    }
  }

  const markAsRead = async (notification: Notification) => {
    if (notification.isRead) return
    try {
      await API.post(`/notifications/${notification.notificationId}/mark-read`)

      const found = notifications.value.find(
        (n) => n.notificationId === notification.notificationId
      )

      if (found) {
        found.isRead = true
        found.readAt = new Date().toISOString()
      }
    } catch (error) {
      console.error("Mark As Read Error:", error)
    }
  }

  const markAllAsRead = async () => {
    const unread = notifications.value.filter((n) => !n.isRead)
    if (!unread.length) return

    try {
      await Promise.all(unread.map((n) => markAsRead(n)))
    } catch (error) {
      console.error("Mark All As Read Error:", error)
    }
  }

  return {
    notifications,
    loading,
    errorMessage,
    unreadCount,
    addNotification,
    fetchNotifications,
    markAsRead,
    markAllAsRead
  }
})
