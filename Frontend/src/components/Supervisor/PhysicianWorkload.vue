<script setup lang="ts">
import { ref, onMounted } from 'vue'
import API from '../../api/axios'

const workloadData = ref<any[]>([])
const weekDays = ['M', 'Tu', 'W', 'Th', 'F', 'Sa', 'Su']

// Helper → get initials from full name
const getInitials = (name: string) => {
  return name
    .split(' ')
    .map(n => n[0])
    .join('')
    .toUpperCase()
}

// Helper → build 7-day shift array based on the passed weekStart date
const buildShifts = (assignments: any[], weekStart: Date) => {
  const shifts: (string | null)[] = new Array(7).fill(null)

  assignments.forEach(a => {
    const date = new Date(a.date + 'T00:00:00')
    const diff = Math.floor((date.getTime() - weekStart.getTime()) / (1000 * 60 * 60 * 24))

    if (diff >= 0 && diff < 7) {
      shifts[diff] = a.shiftType === 'Day' ? 'D' : 'N'
    }
  })

  return shifts
}

const fetchData = async () => {
  try {
    const res = await API.get(
      '/supervisor/dashboard/top-per-specialty'
    )

    const apiData = res.data.data

    // 1. Find the earliest date in the dataset to anchor our weekly grid
    let weekStart = new Date()
    const allAssignments = apiData.flatMap((d: any) => d.assignments)

    if (allAssignments.length > 0) {
      // Find the absolute minimum date in the payload
      const dates = allAssignments.map((a: any) => new Date(a.date + 'T00:00:00').getTime())
      const minDate = new Date(Math.min(...dates))

      // Calculate the Monday of that week
      const day = minDate.getDay()
      const normalizedDay = day === 0 ? 7 : day // convert Sunday (0) to 7
      weekStart = new Date(minDate)
      weekStart.setDate(minDate.getDate() - normalizedDay + 1)
      weekStart.setHours(0, 0, 0, 0)
    } else {
      // Fallback to system current week if API returns no assignments
      const today = new Date()
      const day = today.getDay()
      const normalizedDay = day === 0 ? 7 : day
      weekStart = new Date(today)
      weekStart.setDate(today.getDate() - normalizedDay + 1)
      weekStart.setHours(0, 0, 0, 0)
    }

    // 2. Transform API → UI structure
    const grouped: any = {}

    apiData.forEach((item: any) => {
      if (!grouped[item.specialtyName]) {
        grouped[item.specialtyName] = {
          specialty: item.specialtyName.toUpperCase(),
          physicians: []
        }
      }

      grouped[item.specialtyName].physicians.push({
        initials: getInitials(item.physicianName),
        name: item.physicianName,
        shifts: buildShifts(item.assignments, weekStart) // Pass the calculated week start
      })
    })

    workloadData.value = Object.values(grouped)
  } catch (error) {
    console.error('Error fetching workload data:', error)
  }
}

onMounted(fetchData)
</script>

<template>
  <div class="workload-card">
    <div class="card-header">
      <h3>Top Physician Workload (Weekly)</h3>
      
      <!-- Legend for D and N Shifts -->
      <div class="legend">
        <div class="legend-item">
          <div class="shift-box day">D</div>
          <span>Day</span>
        </div>
        <div class="legend-item">
          <div class="shift-box night">N</div>
          <span>Night</span>
        </div>
      </div>
    </div>

    <div v-for="group in workloadData" :key="group.specialty" class="specialty-section">
      <div class="specialty-title">
        {{ group.specialty }}
      </div>

      <div class="days-row">
        <div class="doctor-info-placeholder"></div>

        <div class="shift-grid">
          <div v-for="day in weekDays" :key="day" class="day-label">
            {{ day }}
          </div>
        </div>
      </div>

      <div v-for="doctor in group.physicians" :key="doctor.name" class="doctor-row">
        <div class="doctor-info">
          <div class="doctor-avatar">
            {{ doctor.initials }}
          </div>

          <div class="doctor-name">
            {{ doctor.name }}
          </div>
        </div>

        <div class="shift-grid">
          <div v-for="(shift, index) in doctor.shifts" :key="index" :class="[
            'shift-box',
            shift === 'D' ? 'day' : '',
            shift === 'N' ? 'night' : ''
          ]">
            {{ shift }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.workload-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  height: calc(100vh - 270px);
  overflow-y: auto;
  padding: 0 12px 0 12px;
}

.card-header {
  position: sticky;
  top: 0;
  z-index: 100;
  background: white;
  padding: 18px;
  border-bottom: 1px solid #e2e8f0;
  
  /* Flexbox layout to separate title and legend */
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header h3 {
  margin: 0;
  font-size: 20px;
  color: #232f72;
  white-space: nowrap
}

/* Legend Styles */
.legend {
  display: flex;
  gap: 16px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 2px;
  font-size: 9px;
  font-weight: 600;
  color: #64748b;
}

.specialty-section {
  margin-top: 18px;
}

.specialty-title {
  font-size: 12px;
  font-weight: 700;
  color: #64748b;
  margin-bottom: 10px;
}

.doctor-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;
}

.doctor-info {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 140px;
}

.doctor-avatar {
  width: 30px;
  height: 30px;
  border-radius: 50%;

  background: #dbeafe;
  color: #2563eb;

  font-size: 11px;
  font-weight: 700;

  display: flex;
  align-items: center;
  justify-content: center;
}

.doctor-name {
  font-size: 13px;
  color: #1e293b;
  font-weight: 500;
}

.shift-grid {
  display: flex;
  gap: 5px;
}

.shift-box {
  width: 22px;
  height: 22px;
  border-radius: 5px;

  background: #e5e7eb;
  color: #6b7280;

  font-size: 10px;
  font-weight: 600;

  display: flex;
  align-items: center;
  justify-content: center;
}

/* DAY SHIFT = BLUE */
.day {
  background: #eff6ff;
  color: #2563eb;
}

/* NIGHT SHIFT = PURPLE/ORANGE */
.night {
  background: #f3e8ff;
  color: #7c3aed;
}

/* Scrollbar */
.workload-card::-webkit-scrollbar {
  width: 6px;
}

.workload-card::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 10px;
}

.workload-card::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 10px;
}

.days-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.doctor-info-placeholder {
  width: 140px;
}

.day-label {
  width: 22px;
  text-align: center;
  font-size: 11px;
  font-weight: 700;
  color: #64748b;
}
</style>