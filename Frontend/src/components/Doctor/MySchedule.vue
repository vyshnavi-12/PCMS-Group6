<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useScheduleStore } from '../../stores/scheduleStore'
import API from '../../api/axios'
import { useToast } from 'primevue/usetoast'

const toast = useToast()
const router = useRouter()
const scheduleStore = useScheduleStore()

interface Schedule {
    coverageAssignmentId: number
    originalDate: string
    date: string
    shift: string
    specialty: string
    time: string
    status: string
}

const showUnavailableModal = ref(false)
const selectedScheduleDate = ref('')
const selectedAssignmentId = ref<number | null>(null)
const unavailableReason = ref('')

const months = [
    'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
]

const days = [
    'Sun', 'Mon', 'Tue', 'Wed',
    'Thu', 'Fri', 'Sat'
]

const formatDate = (dateString: string) => {
    const [year, month, day] = dateString.split('-')

    const date = new Date(
        Number(year),
        Number(month) - 1,
        Number(day)
    )

    return `${days[date.getDay()]}, ${months[Number(month) - 1]} ${day}`
}

const schedules = computed<Schedule[]>(() =>
    scheduleStore.doctorSchedules.map((schedule: any) => ({
        coverageAssignmentId: schedule.coverageAssignmentId,
        originalDate: schedule.date,
        date: formatDate(schedule.date),
        shift: schedule.shift.toUpperCase(),
        specialty: schedule.specialty,
        time: schedule.time,
        status: schedule.status
    }))
)

const fetchMySchedule = async () => {
    try {
        await scheduleStore.fetchDoctorSchedules()
    } catch (error) {
        console.error(error)
    }
}

onMounted(() => {
    fetchMySchedule()
})

const getStatusClass = (status: string) => {
    switch (status) {
        case 'ASSIGNED':
            return 'assigned'
        case 'OFF':
            return 'off'
        default:
            return ''
    }
}

const markUnavailable = (schedule: Schedule) => {
    selectedScheduleDate.value = schedule.date
    selectedAssignmentId.value = schedule.coverageAssignmentId
    unavailableReason.value = ''
    showUnavailableModal.value = true
}

const closeUnavailableModal = () => {
    showUnavailableModal.value = false
}

const submitUnavailableRequest = async () => {
    if (!unavailableReason.value.trim()) {
        toast.add({
            severity: 'warn',
            summary: 'Warning',
            detail: 'Please enter reason',
            life: 3000
        })
        return
    }

    if (!selectedAssignmentId.value) {
        toast.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Invalid assignment',
            life: 3000
        })
        return
    }

    try {
        await API.patch(
            `coverageassignments/${selectedAssignmentId.value}/unavailable`,
            {
                reason: unavailableReason.value
            }
        )

        showUnavailableModal.value = false
        unavailableReason.value = ''
        selectedAssignmentId.value = null

        toast.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Unavailable request submitted successfully',
            life: 3000
        })

        await fetchMySchedule()
    } catch (error: any) {
        console.error(error)

        toast.add({
            severity: 'error',
            summary: 'Error',
            detail:
                error?.response?.data?.message ||
                'Failed to submit unavailable request',
            life: 3000
        })
    }
}

const openSwapRequest = (schedule: Schedule) => {
    router.push({
        path: '/doctor/swap-requests',
        query: {
            new: 'true',
            assignmentId: schedule.coverageAssignmentId,
            shift: schedule.shift,
            specialty: schedule.specialty
        }
    })
}
</script>

<template>
    <div class="schedule-page">

        <div class="table-wrapper">
            <table>
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Shift</th>
                        <th>Specialty</th>
                        <th>Time</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="schedule in schedules" :key="`${schedule.originalDate}-${schedule.shift}`">
                        <td>{{ schedule.date }}</td>
                        <td>{{ schedule.shift }}</td>
                        <td>{{ schedule.specialty }}</td>
                        <td>{{ schedule.time }}</td>

                        <td>
                            <span class="status-badge" :class="getStatusClass(schedule.status)">
                                {{ schedule.status }}
                            </span>
                        </td>

                        <td class="action-cell">
                            <template v-if="schedule.status === 'ASSIGNED'">

                                <button class="unavailable-btn" @click="markUnavailable(schedule)">
                                    Unavailable
                                </button>

                                <button class="swap-btn" @click="openSwapRequest(schedule)">
                                    Request Swap
                                </button>

                            </template>

                            <span v-else>
                                -
                            </span>
                        </td>
                    </tr>

                    <tr v-if="schedules.length === 0">
                        <td colspan="6" class="empty-state">
                            No schedules available
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div v-if="showUnavailableModal" class="modal-overlay">
            <div class="unavailable-modal">

                <div class="modal-header">
                    <h3>Reason</h3>

                    <button class="close-btn" @click="closeUnavailableModal">
                        ✕
                    </button>
                </div>

                <div class="modal-body">

                    <textarea v-model="unavailableReason" placeholder="Enter reason..." rows="5"></textarea>
                </div>

                <div class="modal-actions">
                    <button class="send-btn" @click="submitUnavailableRequest">
                        Send
                    </button>
                </div>

            </div>
        </div>

    </div>
</template>

<style scoped>
.schedule-page {
    height: calc(100vh - 170px);
    display: flex;
    flex-direction: column;
}

.table-wrapper {
    flex: 1;
    overflow-y: auto;
    overflow-x: auto;
    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 10px;
}

table {
    width: 100%;
    border-collapse: collapse;
}

thead {
    position: sticky;
    top: 0;
    z-index: 10;
    background: #f8fafc;
}

th {
    padding: 14px 16px;
    text-align: left;
    font-size: 13px;
    font-weight: 600;
    color: #64748b;
    border-bottom: 1px solid #e2e8f0;
}

td {
    padding: 14px 16px;
    font-size: 14px;
    color: #334155;
    border-top: 1px solid #e2e8f0;
    vertical-align: middle;
}

tbody tr:hover {
    background: #fafcff;
}

.status-badge {
    display: inline-block;
    padding: 4px 10px;
    border-radius: 4px;
    font-size: 11px;
    font-weight: 700;
}

.assigned {
    background: #dcfce7;
    color: #15803d;
}

.off {
    background: #e5e7eb;
    color: #6b7280;
}

.action-cell {
    white-space: nowrap;
}

.unavailable-btn {
    background: #fff7ed;
    color: #c2410c;
    border: 1px solid #fed7aa;
    border-radius: 6px;
    padding: 6px 12px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    margin-right: 6px;
    transition: 0.2s ease;
}

.unavailable-btn:hover {
    background: #ffedd5;
}

.swap-btn {
    background: #eef4ff;
    color: #2563eb;
    border: 1px solid #bfdbfe;
    border-radius: 6px;
    padding: 6px 12px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    transition: 0.2s ease;
}

.swap-btn:hover {
    background: #dbeafe;
}

.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(15, 23, 42, 0.35);
    backdrop-filter: blur(4px);

    display: flex;
    justify-content: center;
    align-items: center;

    z-index: 999;
}

.unavailable-modal {
    width: 500px;
    max-width: 90%;
    background: white;
    border-radius: 14px;
    padding: 24px;
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
}

.modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}

.modal-header h3 {
    margin: 0;
    color: #232f72;
}

.close-btn {
    width: 34px;
    height: 34px;
    border: none;
    border-radius: 50%;
    background: #f1f5f9;
    cursor: pointer;
    font-size: 16px;
}

.close-btn:hover {
    background: #e2e8f0;
}

.modal-body label {
    display: block;
    margin-bottom: 8px;
    color: #64748b;
    font-size: 14px;
    font-weight: 600;
}

.modal-body textarea {
    width: 100%;
    resize: none;
    padding: 12px;
    border: 1px solid #dbe2ea;
    border-radius: 8px;
    font-size: 14px;
    outline: none;
    box-sizing: border-box;
}

.modal-body textarea:focus {
    border-color: #232f72;
}

.modal-actions {
    margin-top: 20px;
    display: flex;
    justify-content: flex-end;
}

.send-btn {
    background: #232f72;
    color: white;
    border: none;
    border-radius: 8px;
    padding: 10px 18px;
    cursor: pointer;
    font-weight: 600;
}

.send-btn:hover {
    background: #1c265f;
}

.empty-state {
    text-align: center;
    color: #94a3b8;
    font-size: 14px;
    padding: 28px;
}

@media (max-width: 1024px) {
    table {
        min-width: 950px;
    }
}
</style>