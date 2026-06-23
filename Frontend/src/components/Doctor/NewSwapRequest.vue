<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import DatePicker from 'primevue/datepicker'

const route = useRoute()
const emit = defineEmits(['back', 'requestCreated'])

const selectedDate = ref<Date | null>(null)
const selectedShift = ref('')
const reason = ref('')
const lockedShift = ref('')

const weeklySchedule = ref<any[]>([])

const parseDate = (dateString: string) => {
    const [year, month, day] = dateString.split('-').map(Number)
    return new Date(year, month - 1, day)
}

const formatDateToString = (date: Date | null) => {
    if (!date) return ''

    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')

    return `${year}-${month}-${day}`
}

const fetchSchedule = async () => {
    try {
        const assignmentId = route.query.assignmentId

        console.log('route query:', route.query)
        console.log('assignmentId:', assignmentId)

        if (route.query.shift) {
            lockedShift.value = route.query.shift.toString()
            selectedShift.value = route.query.shift.toString()
        }

        if (!assignmentId) {
            console.error('Assignment Id missing')
            return
        }

        const response = await axios.get(
            `https://localhost:7119/api/SwapRequests/available-targets/${assignmentId}`,
            {
                withCredentials: true
            }
        )

        weeklySchedule.value = response.data.data.map((schedule: any) => ({
            ...schedule,
            shift: schedule.shift.toUpperCase()
        }))

        if (route.query.date) {
            selectedDate.value = parseDate(route.query.date.toString())
        }

        if (route.query.shift) {
            selectedShift.value = route.query.shift.toString()
        }

    } catch (error: any) {
        console.log(error.response)
        console.log(error.response?.data)
        console.error(error)
    }
}

onMounted(() => {
    fetchSchedule()
})

const availableDates = computed(() => {
    return [...new Set(
        weeklySchedule.value.map(item => item.date)
    )]
})

const minDate = computed<Date | undefined>(() => {
    if (!availableDates.value.length) return undefined
    return parseDate(availableDates.value[0])
})

const maxDate = computed<Date | undefined>(() => {
    if (!availableDates.value.length) return undefined
    return parseDate(
        availableDates.value[availableDates.value.length - 1]
    )
})

const disabledDates = computed(() => {
    if (!minDate.value || !maxDate.value) return []

    const disabled: Date[] = []
    const current = new Date(minDate.value)

    while (current <= maxDate.value) {
        const formatted = formatDateToString(current)

        if (!availableDates.value.includes(formatted)) {
            disabled.push(new Date(current))
        }

        current.setDate(current.getDate() + 1)
    }

    return disabled
})

const selectedCoverage = computed(() => {
    const formattedDate = formatDateToString(selectedDate.value)

    return weeklySchedule.value.find(
        schedule => schedule.date === formattedDate
    )
})

const assignedPhysician = computed(() => {
    return selectedCoverage.value?.physicianName || '-'
})

const specialty = computed(() => {
    return selectedCoverage.value?.specialty || '-'
})

const goBack = () => {
    emit('back')
}

const submitRequest = async () => {
    if (!selectedDate.value || !selectedShift.value) {
        alert('Please select date and shift')
        return
    }

    if (!selectedCoverage.value) {
        alert('No physician found for selected date')
        return
    }

    try {
        const payload = {
            coverageAssignmentId: selectedCoverage.value.coverageAssignmentId,
            targetPhysicianId: selectedCoverage.value.physicianId,
            requestComments: reason.value
        }

        await axios.post(
            'https://localhost:7119/api/SwapRequests',
            payload,
            {
                withCredentials: true
            }
        )

        alert('Swap Request Submitted')
        emit('requestCreated')

    } catch (error) {
        console.error(error)
        alert('Failed to submit request')
    }
}
</script>

<template>
    <div class="new-swap-page">

        <div class="page-header">
            <button class="back-btn" @click="goBack">
                ←
            </button>

            <h2>New Swap Request</h2>
        </div>

        <div class="form-card">

            <div class="request-layout">

                <div class="left-section">

                    <div class="top-row">

                        <div class="field">
                            <label>
                                Select Date
                                <span>*</span>
                            </label>

                            <DatePicker v-model="selectedDate" :manualInput="false" :disabledDates="disabledDates"
                                :minDate="minDate" :maxDate="maxDate" dateFormat="yy-mm-dd" showIcon />
                        </div>

                        <div class="field">
                            <label>
                                Shift
                                <span>*</span>
                            </label>

                            <input type="text" :value="selectedShift" disabled />
                        </div>

                    </div>

                    <div class="field reason-field">

                        <label>
                            Reason
                        </label>

                        <textarea v-model="reason" rows="8" placeholder="Enter reason for swap request"></textarea>

                    </div>

                </div>

                <div class="shift-details">

                    <h3>Selected Shift Details</h3>

                    <div class="detail-item">
                        <span class="detail-label">Date:</span>
                        <span class="detail-value">
                            {{ selectedDate ? formatDateToString(selectedDate) : '-' }}
                        </span>
                    </div>

                    <div class="detail-item">
                        <span class="detail-label">Physician:</span>
                        <span class="detail-value">
                            {{ assignedPhysician }}
                        </span>
                    </div>

                    <div class="detail-item">
                        <span class="detail-label">Shift:</span>
                        <span class="detail-value">
                            {{ selectedShift || '-' }}
                        </span>
                    </div>

                    <div class="detail-item">
                        <span class="detail-label">Specialty:</span>
                        <span class="detail-value">
                            {{ specialty }}
                        </span>
                    </div>

                </div>

            </div>

            <div class="actions">

                <button class="cancel-btn" @click="goBack">
                    Cancel
                </button>

                <button class="submit-btn" @click="submitRequest">
                    Send Request
                </button>

            </div>

        </div>

    </div>
</template>

<style scoped>
.new-swap-page {
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.page-header {
    display: flex;
    align-items: center;
    gap: 12px;
}

.page-header h2 {
    margin: 0;
    font-size: 22px;
    font-weight: 600;
    color: #232f72;
}

.back-btn {
    width: 38px;
    height: 38px;
    border: 1px solid #dbe2ea;
    background: white;
    border-radius: 8px;
    cursor: pointer;
}

.form-card {
    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    padding: 18px;
}

.request-layout {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 18px;
}

.left-section {
    display: flex;
    flex-direction: column;
    gap: 14px;
}

.top-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 14px;
}

.field {
    display: flex;
    flex-direction: column;
}

.field label {
    margin-bottom: 6px;
    font-size: 14px;
    font-weight: 600;
    color: #334155;
}

.field label span {
    color: red;
}

.field input,
.field select,
.field textarea {
    width: 100%;
    padding: 11px;
    border: 1px solid #dbe2ea;
    border-radius: 8px;
    font-size: 14px;
    box-sizing: border-box;
}

.field textarea {
    resize: none;
}

.reason-field textarea {
    height: 160px;
}

.shift-details {
    background: #f8fbff;
    border: 1px solid #c7d8ff;
    border-radius: 10px;
    padding: 12px;
    height: 280px;
}

.shift-details h3 {
    margin: 0 0 12px;
    color: #232f72;
}

.detail-item {
    display: flex;
    align-items: center;
    margin-bottom: 12px;
}

.detail-label {
    width: 90px;
    font-weight: 600;
    color: #334155;
    flex-shrink: 0;
}

.detail-value {
    color: #64748b;
}

.actions {
    display: flex;
    justify-content: center;
    gap: 12px;
    margin-top: 18px;
}

.cancel-btn {
    padding: 10px 24px;
    border: 1px solid #dbe2ea;
    background: white;
    border-radius: 8px;
    cursor: pointer;
}

.submit-btn {
    padding: 10px 24px;
    border: none;
    background: #232f72;
    color: white;
    border-radius: 8px;
    cursor: pointer;
}

@media (max-width: 900px) {
    .request-layout {
        grid-template-columns: 1fr;
    }

    .top-row {
        grid-template-columns: 1fr;
    }
}
</style>