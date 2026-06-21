<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()

const activeTab = ref('Published')
const loading = ref(false)
const errorMessage = ref('')

interface Schedule {
  id: number
  scheduleName: string
  weekStart: string
  weekEnd: string
  status: string
  publishedAt: string
}

const schedules = ref<Schedule[]>([])

const months = [
  'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
  'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
]

const formatDate = (dateString: string) => {
  const [year, month, day] = dateString.split('-')

  return `${months[Number(month) - 1]} ${day}, ${year}`
}

const formatScheduleName = (startDate: string, endDate: string) => {
  const [startYear, startMonth, startDay] = startDate.split('-')
  const [endYear, endMonth, endDay] = endDate.split('-')

  return `${months[Number(startMonth) - 1]} ${startDay} - ${months[Number(endMonth) - 1]} ${endDay}, ${endYear}`
}

const formatPublishedAt = (dateString: string | null) => {
  if (!dateString) return '-'

  const date = new Date(dateString)

  return date.toLocaleString('en-US', {
    month: 'short',
    day: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  })
}

const fetchSchedules = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    const response = await axios.get(
      'https://localhost:7119/api/CoverageSchedules',
      {
        withCredentials: true
      }
    )

    schedules.value = response.data.data.map((schedule: any) => ({
      id: schedule.coverageScheduleId,
      scheduleName: formatScheduleName(
        schedule.weekStartDate,
        schedule.weekEndDate
      ),
      weekStart: formatDate(schedule.weekStartDate),
      weekEnd: formatDate(schedule.weekEndDate),
      status: schedule.status.toUpperCase(),
      publishedAt: formatPublishedAt(schedule.publishedAt)
    }))
  } catch (error: any) {
    errorMessage.value =
      error.response?.data?.message || 'Failed to load schedules'
    console.error(error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchSchedules()
})

const filteredSchedules = computed(() => {
  if (activeTab.value === 'Published') {
    return schedules.value.filter(
      schedule => schedule.status === 'PUBLISHED'
    )
  }

  return schedules.value.filter(
    schedule => schedule.status === 'DRAFT'
  )
})

const createSchedule = () => {
  alert('Create Schedule Clicked')
}

const viewSchedule = (scheduleId: number) => {
  const selectedSchedule = schedules.value.find(
    schedule => schedule.id === scheduleId
  )

  if (!selectedSchedule) return

  router.push({
    path: '/supervisor/coverage-schedule',
    query: {
      id: selectedSchedule.id,
      week: selectedSchedule.scheduleName,
      status: selectedSchedule.status
    }
  })
}
</script>

<template>

  <div class="schedule-page">

    <div class="toolbar">

      <div class="tabs">

        <button
          class="tab-button"
          :class="{ active: activeTab === 'Published' }"
          @click="activeTab = 'Published'"
        >
          Published
        </button>

        <button
          class="tab-button"
          :class="{ active: activeTab === 'Drafts' }"
          @click="activeTab = 'Drafts'"
        >
          Drafts
        </button>

      </div>

      <button
        class="create-btn"
        @click="createSchedule"
      >
        Generate Schedule
      </button>

    </div>

    <div class="table-card">

      <table>

        <thead>
          <tr>
            <th>Bi-Weekly Schedule</th>
            <th>Week Start</th>
            <th>Week End</th>
            <th>Status</th>
            <th>Published At</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>

          <tr
            v-for="schedule in filteredSchedules"
            :key="schedule.id"
          >
            <td>{{ schedule.scheduleName }}</td>

            <td>{{ schedule.weekStart }}</td>

            <td>{{ schedule.weekEnd }}</td>

            <td>

              <span
                class="status-badge"
                :class="schedule.status.toLowerCase()"
              >
                {{ schedule.status }}
              </span>

            </td>

            <td>{{ schedule.publishedAt }}</td>

            <td>

              <button
                class="view-btn"
                @click="viewSchedule(schedule.id)"
              >
                View
              </button>

            </td>

          </tr>

        </tbody>

      </table>

    </div>

  </div>

</template>

<style scoped>
.schedule-page {
    display: flex;
    flex-direction: column;
    gap: 20px;
}

.toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.tabs {
    display: flex;
    gap: 24px;
}

.tab-button {
    background: none;
    border: none;
    cursor: pointer;
    color: #64748b;
    font-size: 14px;
    font-weight: 600;
    padding-bottom: 8px;
}

.tab-button.active {
    color: #232f72;
    border-bottom: 2px solid #232f72;
}

.create-btn {
    background: #232f72;
    color: white;
    border: none;
    border-radius: 6px;
    padding: 10px 16px;
    cursor: pointer;
    font-weight: 600;
}

.create-btn:hover {
    background: #1d285f;
}

.table-card {
    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    overflow: hidden;
}

table {
    width: 100%;
    border-collapse: collapse;
}

thead {
    background: #f8fafc;
}

th {
    text-align: left;
    padding: 14px 16px;
    font-size: 13px;
    color: #64748b;
    border-bottom: 1px solid #e2e8f0;
}

td {
    padding: 14px 16px;
    font-size: 14px;
    color: #334155;
    border-top: 1px solid #f1f5f9;
}

.status-badge {
    display: inline-block;
    padding: 4px 10px;
    border-radius: 6px;
    font-size: 11px;
    font-weight: 700;
}

.published {
    background: #dcfce7;
    color: #15803d;
}

.draft {
    background: #fef3c7;
    color: #b45309;
}

.view-btn {
    background: #eef4ff;
    color: #2563eb;
    border: none;
    border-radius: 6px;
    padding: 6px 14px;
    cursor: pointer;
    font-weight: 600;
}

.view-btn:hover {
    background: #dbeafe;
}

@media (max-width: 1024px) {
    .table-card {
        overflow-x: auto;
    }

    table {
        min-width: 900px;
    }
}
</style>