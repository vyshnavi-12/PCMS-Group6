<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useScheduleStore } from '../../stores/scheduleStore'
import OverlayPanel from 'primevue/overlaypanel'
import API from '../../api/axios'

const route = useRoute()
const toast = useToast()
const scheduleStore = useScheduleStore()

const currentWeek = ref('')
const scheduleStatus = ref('DRAFT')

const weeks = ref<string[]>([])
const selectedScheduleId = ref<number | null>(null)
const currentWeekIndex = ref(0)

const isEditing = ref(false)
const physicians = ref<string[]>([])
const specialties = ref<string[]>([])
const activeCell = ref('')

const loading = ref(false)
const errorMessage = ref('')
const coverageSchedule = ref<any[]>([])

const op = ref()
const isLoadingDoctor = ref(false)
const selectedDoctor = ref({
  name: '',
  employeeCode: '',
  phone: '',
  email: ''
})

const months = [
  'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
  'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
]

const formatDate = (dateString: string) => {
  const [year, month, day] = dateString.split('-')
  return `${months[Number(month) - 1]} ${day}, ${year}`
}

const formatWeekRange = (startDate: string, endDate: string) => {
  const [, startMonth, startDay] = startDate.split('-')
  const [endYear, endMonth, endDay] = endDate.split('-')

  return `${months[Number(startMonth) - 1]} ${startDay} - ${months[Number(endMonth) - 1]} ${endDay}, ${endYear}`
}

const openCellEditor = (date: string, shift: string, specialty: string) => {
  activeCell.value = `${date}-${shift}-${specialty}`
}

const closeCellEditor = () => {
  activeCell.value = ''
}

const fetchAllSchedules = async () => {
  try {
    await scheduleStore.fetchSchedules()

    scheduleStore.schedules.sort((a: any, b: any) =>
      new Date(a.weekStartDate).getTime() - new Date(b.weekStartDate).getTime()
    )

    weeks.value = scheduleStore.schedules.map((schedule: any) =>
      formatWeekRange(schedule.weekStartDate, schedule.weekEndDate)
    )

    if (route.query.id) {
      selectedScheduleId.value = Number(route.query.id)
    } else if (scheduleStore.schedules.length > 0) {
      selectedScheduleId.value =
        scheduleStore.schedules[0].coverageScheduleId
    }

    currentWeekIndex.value = scheduleStore.schedules.findIndex(
      (schedule: any) =>
        schedule.coverageScheduleId === selectedScheduleId.value
    )

    if (currentWeekIndex.value === -1) {
      currentWeekIndex.value = 0
    }

    await fetchScheduleDetails()
  } catch (error) {
    console.error(error)
  }
}

const fetchScheduleDetails = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    if (!selectedScheduleId.value) return

    const scheduleData = await scheduleStore.fetchScheduleById(
      selectedScheduleId.value
    )

    currentWeek.value = formatWeekRange(
      scheduleData.weekStartDate,
      scheduleData.weekEndDate
    )

    scheduleStatus.value = scheduleData.status.toUpperCase()

    const assignments = scheduleData.assignments

    assignments.sort((a: any, b: any) =>
      new Date(a.coverageDate).getTime() - new Date(b.coverageDate).getTime()
    )

    const physicianSet = new Set<string>()
    const specialtySet = new Set<string>()

    assignments.forEach((assignment: any) => {
      physicianSet.add(assignment.physicianName)
      specialtySet.add(assignment.specialtyName)
    })

    physicians.value = Array.from(physicianSet)
    specialties.value = Array.from(specialtySet)

    const groupedByDate: Record<string, any> = {}

    assignments.forEach((assignment: any) => {
      const formattedDate = formatDate(assignment.coverageDate)
      const shiftType = assignment.shiftType.toUpperCase()
      const specialty = assignment.specialtyName

      if (!groupedByDate[formattedDate]) {
        groupedByDate[formattedDate] = {
          date: formattedDate,
          shifts: {}
        }
      }

      if (!groupedByDate[formattedDate].shifts[shiftType]) {
        const emptyAssignments: Record<string, string> = {}

        specialties.value.forEach(s => {
          emptyAssignments[s] = '-'
        })

        groupedByDate[formattedDate].shifts[shiftType] = {
          type: shiftType,
          assignments: emptyAssignments
        }
      }

      groupedByDate[formattedDate]
        .shifts[shiftType]
        .assignments[specialty] = { 
      name: assignment.physicianName, 
      id: assignment.physicianId 
  };
    })

    coverageSchedule.value = Object.values(groupedByDate).map((day: any) => ({
      date: day.date,
      shifts: Object.values(day.shifts)
    }))
  } catch (error: any) {
    errorMessage.value =
      error.response?.data?.message || 'Failed to load schedule'
  } finally {
    loading.value = false
  }
}

const previousWeek = async () => {
  if (currentWeekIndex.value > 0) {
    currentWeekIndex.value--

    selectedScheduleId.value =
      scheduleStore.schedules[currentWeekIndex.value].coverageScheduleId

    await fetchScheduleDetails()
  }
}

const nextWeek = async () => {
  if (currentWeekIndex.value < scheduleStore.schedules.length - 1) {
    currentWeekIndex.value++

    selectedScheduleId.value =
      scheduleStore.schedules[currentWeekIndex.value].coverageScheduleId

    await fetchScheduleDetails()
  }
}

const editSchedule = () => {
  isEditing.value = true
}

const updateSchedule = () => {
  isEditing.value = false
}

const cancelEdit = () => {
  isEditing.value = false
}

const publishSchedule = async () => {
  try {
    if (!selectedScheduleId.value) return

    await scheduleStore.publishSchedule(selectedScheduleId.value)

    scheduleStatus.value = 'PUBLISHED'
    isEditing.value = false

    await fetchAllSchedules()

    toast.add({
      severity: 'success',
      summary: 'Success',
      detail: 'Schedule Published Successfully',
      life: 3000
    })
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Failed to publish schedule',
      life: 3000
    })
  }
}

const showDoctorCard = async (event: Event, doctor: { name: string, id: number } | null) => {
  if (!doctor || !doctor.id) return;

  // 1. CLEAR OLD DATA and set the new name immediately
  selectedDoctor.value = {
    name: doctor.name,
    employeeCode: '',
    phone: '',
    email: ''
  };

  // 2. TOGGLE OVERLAY SYNCHRONOUSLY so PrimeVue grabs the correct DOM coordinates
  // Use .show(event) instead of toggle to force it to attach to the new click target
  op.value.show(event);
  
  // 3. Set loading state
  isLoadingDoctor.value = true;

  try {
    // 4. Fetch the real details
    const res = await API.get(`physician/${doctor.id}/details`);
    selectedDoctor.value = {
        name: doctor.name,
        employeeCode: res.data.data.employeeCode || 'N/A',
        phone: res.data.data.phoneNumber || 'N/A',
        email: res.data.data.email || 'N/A'
    };
  } catch (err) {
    console.error("Failed to fetch doctor details", err);
    toast.add({ severity: 'error', summary: 'Error', detail: 'Could not load details', life: 3000 });
  } finally {
    isLoadingDoctor.value = false;
  }
}

onMounted(() => {
  fetchAllSchedules()
})
</script>

<template>
  <div class="coverage-page">
    <div class="toolbar">
      <div class="week-navigation">
        <button class="nav-btn" @click="previousWeek" :disabled="currentWeekIndex === 0">
          ‹
        </button>

        <div class="week-label">
          {{ currentWeek }}
        </div>

        <button class="nav-btn" @click="nextWeek" :disabled="currentWeekIndex === weeks.length - 1">
          ›
        </button>
      </div>

      <div v-if="scheduleStatus !== 'PUBLISHED'" class="toolbar-actions">
        <button class="edit-btn" @click="editSchedule">
          <i class="pi pi-pencil"></i>
          Edit
        </button>

        <button class="publish-btn" @click="publishSchedule">
          Publish Schedule
        </button>
      </div>
    </div>

    <div class="table-wrapper">
      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Shift</th>

            <th v-for="specialty in specialties" :key="specialty">
              {{ specialty }}
            </th>
          </tr>
        </thead>

        <tbody>
          <template v-for="day in coverageSchedule" :key="day.date">
            <tr v-for="(shift, index) in day.shifts" :key="day.date + shift.type">
              <td v-if="index === 0" :rowspan="day.shifts.length" class="date-cell">
                {{ day.date }}
              </td>

              <td class="shift-cell">
                {{ shift.type }}
              </td>

              <td v-for="specialty in specialties" :key="specialty">
                <select v-if="isEditing && activeCell === `${day.date}-${shift.type}-${specialty}`"
                  v-model="shift.assignments[specialty]" class="physician-dropdown" @blur="closeCellEditor">
                  <option v-for="doctor in physicians" :key="doctor" :value="doctor">
                    {{ doctor }}
                  </option>
                </select>

                <span v-else class="doctor-name" :class="{ editable: isEditing, hoverable: !isEditing }" @click="
                  isEditing
                    ? openCellEditor(day.date, shift.type, specialty)
                    : showDoctorCard($event, shift.assignments[specialty])
                  ">
                  {{ shift.assignments[specialty]?.name || '-' }}

                  <span v-if="isEditing" class="edit-icon">
                    <i class="pi pi-pencil"></i>
                  </span>

                  <span v-if="!isEditing && shift.assignments[specialty]?.name" class="contact-icon">
                    <i class="pi pi-id-card"></i>
                  </span>
                </span>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <div v-if="isEditing" class="update-section">
      <button class="cancel-btn" @click="cancelEdit">
        Cancel
      </button>

      <button class="update-btn" @click="updateSchedule">
        Update
      </button>
    </div>
  </div>

  <OverlayPanel ref="op">
    <div class="doctor-card">
      <h4>{{ selectedDoctor.name || 'Physician Details' }}</h4>

      <div v-if="isLoadingDoctor" class="loading-state">
        <i class="pi pi-spin pi-spinner"></i> Fetching details...
      </div>

      <div v-else>
        <div class="doctor-detail">
          <label>Employee Code</label>
          <p>{{ selectedDoctor.employeeCode }}</p>
        </div>

        <div class="doctor-detail">
          <label>Phone Number</label>
          <p>{{ selectedDoctor.phone }}</p>
        </div>

        <div class="doctor-detail">
          <label>Email</label>
          <p>{{ selectedDoctor.email }}</p>
        </div>
      </div>
    </div>
  </OverlayPanel>
</template>

<style scoped>
.coverage-page {
  display: flex;
  flex-direction: column;
  gap: 18px;

  height: calc(100vh - 150px);
  min-height: 0;

  overflow: hidden;
}

/* =========================
   TOOLBAR
========================= */

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
}

.week-navigation {
  display: flex;
  align-items: center;
  gap: 10px;
}

.nav-btn {
  width: 36px;
  height: 36px;

  border: 1px solid #dbe2ea;
  background: white;

  border-radius: 8px;
  cursor: pointer;

  font-size: 18px;
  transition: 0.2s;
}

.nav-btn:hover:not(:disabled) {
  background: #f8fafc;
}

.nav-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.week-label {
  min-width: 240px;

  padding: 10px 18px;

  background: white;

  border: 1px solid #dbe2ea;
  border-radius: 8px;

  text-align: center;
  font-weight: 600;
  color: #334155;
}

.toolbar-actions {
  display: flex;
  gap: 10px;
}

.edit-btn {
  display: flex;
  align-items: center;
  gap: 6px;

  background: white;
  color: #334155;

  border: 1px solid #dbe2ea;
  border-radius: 8px;

  padding: 10px 16px;

  cursor: pointer;
  font-weight: 600;

  transition: 0.2s;
}

.edit-btn:hover {
  background: #f8fafc;
}

.publish-btn {
  background: #232f72;
  color: white;

  border: none;
  border-radius: 8px;

  padding: 10px 18px;

  cursor: pointer;
  font-weight: 600;

  transition: 0.2s;
}

.publish-btn:hover {
  background: #1c265f;
}

/* =========================
   TABLE WRAPPER
========================= */

.table-wrapper {
  flex: 1;

  min-height: 0;

  background: white;

  border: 1px solid #e2e8f0;
  border-radius: 12px;

  overflow-y: auto;
  overflow-x: auto;
}

/* =========================
   TABLE
========================= */

table {
  width: 100%;
  border-collapse: collapse;
  min-width: 900px;
}

thead th {
  position: sticky;
  top: 0;
  z-index: 20;

  background: #f8fafc;

  padding: 14px 12px;

  border-bottom: 1px solid #e2e8f0;

  font-size: 13px;
  font-weight: 600;
  color: #64748b;

  text-align: center;
}

tbody td {
  padding: 12px;

  border-top: 1px solid #f1f5f9;

  font-size: 13px;
  color: #334155;

  text-align: center;
}

tbody tr:hover {
  background: #fafbfc;
}

/* =========================
   DATE COLUMN
========================= */

.date-cell {
  min-width: 140px;

  text-align: left;

  font-weight: 600;
  color: #232f72;

  background: white;

  position: sticky;
  left: 0;
  z-index: 10;
}

tbody tr:hover .date-cell {
  background: #fafbfc;
}

.doctor-name {
  cursor: pointer;
  display: block;
  padding: 6px;
  border-radius: 4px;
}

.doctor-name:hover {
  background: #f8fafc;
}

.doctor-name {
  position: relative;
}

.doctor-name.editable:hover {
  background: #f1f5f9;
}

.edit-icon {
  position: absolute;
  top: 4px;
  right: 4px;

  width: 14px;
  height: 14px;

  display: flex;
  align-items: center;
  justify-content: center;

  border-radius: 50%;
  background: #e2e8f0;
  color: #64748b;

  opacity: 0;
  transition: 0.2s;
}

.edit-icon i {
  font-size: 7px !important;
}

.doctor-name.editable:hover .edit-icon {
  opacity: 1;
}

.physician-dropdown {
  width: 100%;
  padding: 6px 8px;

  border: 1px solid #dbe2ea;
  border-radius: 6px;

  font-size: 13px;
}

.doctor-name {
  position: relative;
  display: block;
  padding: 14px 28px 14px 12px;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.hoverable:hover {
  background: #f4f7ff;
  cursor: pointer;
}

.contact-icon {
  position: absolute;
  top: 6px;
  right: 6px;
  opacity: 0;
  font-size: 12px;
  color: #4f46e5;
  transition: opacity 0.2s ease;
}

.hoverable:hover .contact-icon {
  opacity: 1;
}

.doctor-card {
  min-width: 220px;
  padding: 4px;
}

.doctor-card h4 {
  margin: 0 0 10px 0;
  font-size: 16px;
  color: #1e3a8a;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 8px;
}

.loading-state {
  padding: 12px 0;
  color: #6b7280;
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.doctor-detail {
  margin-bottom: 10px;
}

.doctor-detail:last-child {
  margin-bottom: 0;
}

.doctor-detail label {
  display: block;
  font-size: 11px;
  color: #6b7280;
  margin-bottom: 2px;
}

.doctor-detail p {
  margin: 0;
  font-size: 13px;
  font-weight: 500;
  line-height: 1.3;
}

:deep(.p-overlaypanel .p-overlaypanel-content) {
  padding: 10px 12px !important;
}

:deep(.p-overlaypanel) {
  max-width: 250px;
}

/* =========================
   SHIFT COLUMN
========================= */

.shift-cell {
  width: 90px;

  font-weight: 600;
  color: #475569;
}

/* =========================
   EDIT INPUTS
========================= */

.assignment-input {
  width: 100%;

  padding: 6px 8px;

  border: 1px solid #dbe2ea;
  border-radius: 6px;

  font-size: 12px;

  box-sizing: border-box;
}

.assignment-input:focus {
  outline: none;
  border-color: #232f72;
}

/* =========================
   UPDATE BUTTON
========================= */

.update-section {
  display: flex;
  justify-content: flex-end;
  gap: 10px;

  margin-top: 12px;
}

.cancel-btn {
  background: white;
  color: #475569;

  border: 1px solid #dbe2ea;
  border-radius: 8px;

  padding: 10px 18px;

  cursor: pointer;
  font-weight: 600;

  transition: 0.2s;
}

.cancel-btn:hover {
  background: #f8fafc;
}

.update-btn {
  background: #232f72;
  color: white;

  border: none;
  border-radius: 8px;

  padding: 10px 18px;

  cursor: pointer;
  font-weight: 600;
}

.update-btn:hover {
  background: #1c265f;
}

/* =========================
   RESPONSIVE
========================= */

@media (max-width: 1200px) {
  table {
    min-width: 1000px;
  }
}
</style>