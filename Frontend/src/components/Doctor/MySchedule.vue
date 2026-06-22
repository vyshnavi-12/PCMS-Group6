<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()

interface Schedule {
    coverageAssignmentId: number
    originalDate: string
    date: string
    shift: string
    specialty: string
    time: string
    status: string
}

const schedules = ref<Schedule[]>([])

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

const fetchMySchedule = async () => {
    try {
        const response = await axios.get(
            'https://localhost:7119/api/MySchedule',
            {
                withCredentials: true
            }
        )

        schedules.value = response.data.data.map(
            (schedule: any) => ({
                coverageAssignmentId: schedule.coverageAssignmentId,
                originalDate: schedule.date,
                date: formatDate(schedule.date),
                shift: schedule.shift.toUpperCase(),
                specialty: schedule.specialty,
                time: schedule.time,
                status: schedule.status
            })
        )
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

const markUnavailable = (scheduleDate: string) => {
    alert(`Unavailable request submitted for ${scheduleDate}`)
}

const openSwapRequest = (schedule: Schedule) => {
    router.push({
        path: '/doctor/swap-requests',
        query: {
            new: 'true',
            assignmentId: schedule.coverageAssignmentId,
            date: schedule.originalDate,
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

                                <button class="unavailable-btn" @click="markUnavailable(schedule.date)">
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

                </tbody>

            </table>

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

@media (max-width: 1024px) {
    table {
        min-width: 950px;
    }
}
</style>