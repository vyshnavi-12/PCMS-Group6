<script setup lang="ts">
import { ref, computed } from 'vue'

const emit = defineEmits(['back'])

const selectedDate = ref('')
const selectedShift = ref('')
const reason = ref('')

/*
  MOCK COVERAGE SCHEDULE
  Later replace with API call
*/
const weeklySchedule = [
    {
        date: '2026-06-15',
        shift: 'DAY',
        physician: 'Dr. Michael Brown',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-15',
        shift: 'NIGHT',
        physician: 'Dr. Sarah Davis',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-16',
        shift: 'DAY',
        physician: 'Dr. James Wilson',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-16',
        shift: 'NIGHT',
        physician: 'Dr. Emily Clark',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-17',
        shift: 'DAY',
        physician: 'Dr. Robert Taylor',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-17',
        shift: 'NIGHT',
        physician: 'Dr. Sarah Davis',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-18',
        shift: 'DAY',
        physician: 'Dr. Michael Brown',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-18',
        shift: 'NIGHT',
        physician: 'Dr. Emily Clark',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-19',
        shift: 'DAY',
        physician: 'Dr. James Wilson',
        specialty: 'Cardiology'
    },
    {
        date: '2026-06-19',
        shift: 'NIGHT',
        physician: 'Dr. Robert Taylor',
        specialty: 'Cardiology'
    }
]

/*
  CURRENT WEEK RANGE
*/
const today = new Date()

const minDate = computed(() => {
    const monday = new Date(today)

    const day = monday.getDay()

    monday.setDate(
        monday.getDate() - day + (day === 0 ? -6 : 1)
    )

    return monday.toISOString().split('T')[0]
})

const maxDate = computed(() => {
    const sunday = new Date(minDate.value)

    sunday.setDate(sunday.getDate() + 6)

    return sunday.toISOString().split('T')[0]
})

/*
  FIND ASSIGNED PHYSICIAN
*/
const selectedCoverage = computed(() => {
    return weeklySchedule.find(
        schedule =>
            schedule.date === selectedDate.value &&
            schedule.shift === selectedShift.value
    )
})

const assignedPhysician = computed(() =>
    selectedCoverage.value?.physician ?? ''
)

const specialty = computed(() =>
    selectedCoverage.value?.specialty ?? ''
)

const goBack = () => {
    emit('back')
}

const submitRequest = () => {
    if (!selectedDate.value || !selectedShift.value) {
        alert('Please select date and shift')
        return
    }

    if (!selectedCoverage.value) {
        alert('No physician assigned for this shift')
        return
    }

    alert('Swap Request Submitted')

    emit('back')
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

                            <input v-model="selectedDate" type="date" :min="minDate" :max="maxDate" />
                        </div>

                        <div class="field">
                            <label>
                                Select Shift
                                <span>*</span>
                            </label>

                            <select v-model="selectedShift">

                                <option value="">
                                    Select Shift
                                </option>

                                <option value="DAY">
                                    DAY (08:00 AM - 04:00 PM)
                                </option>

                                <option value="NIGHT">
                                    NIGHT (04:00 PM - 12:00 AM)
                                </option>

                            </select>
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
                        <span class="detail-value">{{ selectedDate || '-' }}</span>
                    </div>

                    <div class="detail-item">
                        <span class="detail-label">Physician:</span>
                        <span class="detail-value">{{ assignedPhysician || '-' }}</span>
                    </div>

                    <div class="detail-item">
                        <span class="detail-label">Shift:</span>
                        <span class="detail-value">{{ selectedShift || '-' }}</span>
                    </div>

                    <div class="detail-item">
                        <span class="detail-label">Specialty:</span>
                        <span class="detail-value">{{ specialty || '-' }}</span>
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