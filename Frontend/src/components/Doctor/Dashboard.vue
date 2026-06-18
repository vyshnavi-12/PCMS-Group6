<script setup lang="ts">
import { ref } from 'vue'

const currentWeek = ref('May 19 – May 25, 2025')

const schedule = [
  {
    label: 'DAY',
    time: '8:00 AM – 4:00 PM',
    assignments: [
      'Assigned',
      'Assigned',
      'Assigned',
      null,
      'Assigned',
      'Assigned',
      null
    ]
  },
  {
    label: 'NIGHT',
    time: '4:00 PM – 12:00 AM',
    assignments: [
      'Assigned',
      'Assigned',
      'Assigned',
      'Assigned',
      'Assigned',
      'Assigned',
      null
    ]
  }
]

const previousWeek = () => {
  console.log('Previous Week')
}

const nextWeek = () => {
  console.log('Next Week')
}

const openCalendar = () => {
  console.log('Open Calendar')
}
</script>

<template>

  <div class="dashboard-page">

    <!-- SUMMARY CARDS -->

    <div class="stats-grid">

      <div class="stat-card">

        <div class="icon blue">
          <i class="pi pi-calendar"></i>
        </div>

        <div>
          <div class="stat-title">
            My Assignments
          </div>

          <div class="stat-subtitle">
            This Week
          </div>

          <div class="stat-value">
            5
          </div>
        </div>

      </div>

      <div class="stat-card">

        <div class="icon orange">
          <i class="pi pi-arrow-right-arrow-left"></i>
        </div>

        <div>
          <div class="stat-title">
            Swap Requests
          </div>

          <div class="stat-subtitle">
            Pending
          </div>

          <div class="stat-value">
            1
          </div>
        </div>

      </div>

      <div class="stat-card">

        <div class="icon red">
          <i class="pi pi-bell"></i>
        </div>

        <div>
          <div class="stat-title">
            Notifications
          </div>

          <div class="stat-subtitle">
            Unread
          </div>

          <div class="stat-value">
            3
          </div>
        </div>

      </div>

    </div>

    <!-- SCHEDULE -->

    <div class="schedule-card">

      <div class="card-header">

        <h3>
          My Schedule Overview (Weekly)
        </h3>

        <button class="calendar-btn" @click="openCalendar">
          <i class="pi pi-calendar"></i>
          View Schedule
        </button>

      </div>

      <div class="week-toolbar">

        <button class="nav-btn" @click="previousWeek">
          ‹
        </button>

        <span class="week-label">
          {{ currentWeek }}
        </span>

        <button class="nav-btn" @click="nextWeek">
          ›
        </button>

      </div>

      <table class="schedule-table">

        <thead>

          <tr>

            <th></th>

            <th>Mon 19</th>
            <th>Tue 20</th>
            <th>Wed 21</th>
            <th>Thu 22</th>
            <th>Fri 23</th>
            <th>Sat 24</th>
            <th>Sun 25</th>

          </tr>

        </thead>

        <tbody>

          <tr v-for="row in schedule" :key="row.label">

            <td class="shift-column">

              <div class="shift-name">
                {{ row.label }}
              </div>

              <div class="shift-time">
                {{ row.time }}
              </div>

            </td>

            <td v-for="(assignment, index) in row.assignments" :key="`${row.label}-${index}`">

              <div v-if="assignment" :class="[
                row.label === 'DAY'
                  ? 'day-badge'
                  : row.label === 'NIGHT'
                    ? 'night-badge'
                    : 'off-badge'
              ]">
                {{ assignment }}
              </div>

              <span v-else class="empty-slot">
                —
              </span>

            </td>

          </tr>

        </tbody>

      </table>

      <div class="legend">

        <span>
          <span class="dot day"></span>
          Day Shift
        </span>

        <span>
          <span class="dot night"></span>
          Night Shift
        </span>

      </div>

    </div>

  </div>

</template>

<style scoped>
.dashboard-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* STATS */

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.stat-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 18px;

  display: flex;
  align-items: center;
  gap: 16px;
}

.icon {
  width: 52px;
  height: 52px;

  border-radius: 12px;

  display: flex;
  justify-content: center;
  align-items: center;

  font-size: 22px;
}

.blue {
  background: #dbeafe;
  color: #2563eb;
}

.orange {
  background: #fef3c7;
  color: #d97706;
}

.red {
  background: #fee2e2;
  color: #dc2626;
}

.stat-title {
  font-size: 14px;
  font-weight: 600;
  color: #1e293b;
}

.stat-subtitle {
  font-size: 13px;
  color: #64748b;
}

.stat-value {
  font-size: 30px;
  font-weight: 700;
  color: #0f172a;
}

/* SCHEDULE CARD */

.schedule-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;

  margin-bottom: 16px;
}

.card-header h3 {
  margin: 0;
  color: #232f72;
}

.calendar-btn {
  border: 1px solid #dbe2ea;
  background: white;

  padding: 8px 14px;
  border-radius: 8px;

  cursor: pointer;
  font-weight: 600;
}

.week-toolbar {
  display: flex;
  align-items: center;
  gap: 10px;

  margin-bottom: 16px;
}

.nav-btn {
  width: 34px;
  height: 34px;

  border: 1px solid #dbe2ea;
  border-radius: 8px;

  background: white;
  cursor: pointer;
}

.week-label {
  font-weight: 600;
  color: #334155;
}

.schedule-table {
  width: 100%;
  border-collapse: collapse;
}

.schedule-table th,
.schedule-table td {
  border: 1px solid #eef2f7;
  padding: 12px;
  text-align: center;
}

.schedule-table th {
  background: #f8fafc;
}

.shift-column {
  width: 120px;
}

.shift-name {
  font-weight: 700;
  color: #334155;
}

.shift-time {
  font-size: 12px;
  color: #64748b;
}

.day-badge,
.night-badge{
  border-radius: 8px;
  padding: 10px;
  font-size: 12px;
  font-weight: 600;
}

.day-badge {
  background: #eff6ff;
  color: #2563eb;
}

.night-badge {
  background: #f3e8ff;
  color: #7c3aed;
}

.empty-slot {
  color: #94a3b8;
}

.legend {
  display: flex;
  justify-content: center;
  gap: 30px;

  margin-top: 16px;

  font-size: 13px;
}

.dot {
  width: 10px;
  height: 10px;

  border-radius: 50%;

  display: inline-block;
  margin-right: 6px;
}

.dot.day {
  background: #2563eb;
}

.dot.night {
  background: #7c3aed;
}
</style>