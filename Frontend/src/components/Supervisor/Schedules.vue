<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const activeTab = ref('Published')

const schedules = ref([
  {
    id: 1,
    scheduleName: 'May 19 - Jun 01, 2025',
    weekStart: 'May 19, 2025',
    weekEnd: 'Jun 01, 2025',
    status: 'PUBLISHED',
    publishedAt: 'May 18, 2025 10:30 AM'
  },

  {
    id: 2,
    scheduleName: 'May 05 - May 18, 2025',
    weekStart: 'May 05, 2025',
    weekEnd: 'May 18, 2025',
    status: 'PUBLISHED',
    publishedAt: 'May 04, 2025 09:15 AM'
  },

  {
    id: 3,
    scheduleName: 'Jun 02 - Jun 15, 2025',
    weekStart: 'Jun 02, 2025',
    weekEnd: 'Jun 15, 2025',
    status: 'DRAFT',
    publishedAt: '-'
  },

  {
    id: 4,
    scheduleName: 'Jun 16 - Jun 29, 2025',
    weekStart: 'Jun 16, 2025',
    weekEnd: 'Jun 29, 2025',
    status: 'DRAFT',
    publishedAt: '-'
  }
])

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