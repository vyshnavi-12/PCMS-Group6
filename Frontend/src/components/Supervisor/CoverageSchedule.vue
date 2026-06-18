<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const currentWeek = ref(
    route.query.week?.toString() || 'Jun 02 - Jun 15, 2025'
)

const scheduleStatus = ref(
    route.query.status?.toString() || 'DRAFT'
)

const weeks = [
    'May 05 - May 18, 2025',
    'May 19 - Jun 01, 2025',
    'Jun 02 - Jun 15, 2025',
    'Jun 16 - Jun 29, 2025'
]

const currentWeekIndex = ref(
    Math.max(
        weeks.findIndex(
            week => week === currentWeek.value
        ),
        0
    )
)

const isEditing = ref(false)

const physicians = ref([
    'Dr. Michael Brown',
    'Dr. Sarah Davis',
    'Dr. Emily Clark',
    'Dr. James Wilson',
    'Dr. Robert Taylor',
    'Dr. John Miller',
    'Dr. Sarah White',
    'Dr. Adam Scott'
])

const activeCell = ref('')

const openCellEditor = (
    date: string,
    shift: string,
    specialty: string
) => {
    activeCell.value =
        `${date}-${shift}-${specialty}`
}

const closeCellEditor = () => {
    activeCell.value = ''
}

/* =========================
   COVERAGE SCHEDULE
========================= */

const coverageSchedule = ref([
    {
        date: 'Jun 02, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Michael Brown',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Sarah Davis',
                neurology: 'Dr. Sarah White',
                orthopedics: 'Dr. Emily Clark'
            }
        ]
    },

    {
        date: 'Jun 03, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. James Wilson',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Emily Clark',
                neurology: 'Dr. Sarah White',
                orthopedics: '-'
            }
        ]
    },

    {
        date: 'Jun 04, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Michael Brown',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Sarah Davis',
                neurology: 'Dr. Sarah White',
                orthopedics: 'Dr. Emily Clark'
            }
        ]
    },

    {
        date: 'Jun 05, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Robert Taylor',
                neurology: 'Dr. Sarah White',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Emily Clark',
                neurology: 'Dr. John Miller',
                orthopedics: '-'
            }
        ]
    },

    {
        date: 'Jun 06, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Michael Brown',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Sarah Davis',
                neurology: 'Dr. Sarah White',
                orthopedics: 'Dr. Emily Clark'
            }
        ]
    },

    {
        date: 'Jun 07, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. James Wilson',
                neurology: '-',
                orthopedics: '-'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Emily Clark',
                neurology: '-',
                orthopedics: '-'
            }
        ]
    },

    {
        date: 'Jun 08, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Michael Brown',
                neurology: '-',
                orthopedics: '-'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Sarah Davis',
                neurology: '-',
                orthopedics: '-'
            }
        ]
    },

    {
        date: 'Jun 09, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. James Wilson',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Emily Clark',
                neurology: 'Dr. Sarah White',
                orthopedics: 'Dr. Emily Clark'
            }
        ]
    },

    {
        date: 'Jun 10, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Robert Taylor',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Sarah Davis',
                neurology: 'Dr. Sarah White',
                orthopedics: '-'
            }
        ]
    },

    {
        date: 'Jun 11, 2025',

        shifts: [
            {
                type: 'DAY',
                cardiology: 'Dr. Michael Brown',
                neurology: 'Dr. Sarah White',
                orthopedics: 'Dr. Adam Scott'
            },

            {
                type: 'NIGHT',
                cardiology: 'Dr. Emily Clark',
                neurology: 'Dr. John Miller',
                orthopedics: 'Dr. Emily Clark'
            }
        ]
    }
])

/* =========================
   WEEK NAVIGATION
========================= */

const previousWeek = () => {
    if (currentWeekIndex.value > 0) {
        currentWeekIndex.value--

        currentWeek.value =
            weeks[currentWeekIndex.value]
    }
}

const nextWeek = () => {
    if (
        currentWeekIndex.value <
        weeks.length - 1
    ) {
        currentWeekIndex.value++

        currentWeek.value =
            weeks[currentWeekIndex.value]
    }
}

/* =========================
   EDIT MODE
========================= */

const editSchedule = () => {
    isEditing.value = true
}

const updateSchedule = () => {
    isEditing.value = false

    alert('Schedule Updated')
}

const cancelEdit = () => {
    isEditing.value = false
}

/* =========================
   PUBLISH
========================= */

const publishSchedule = () => {
    scheduleStatus.value = 'PUBLISHED'

    isEditing.value = false

    alert('Schedule Published')
}
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

                        <th>Cardiology</th>
                        <th>Neurology</th>
                        <th>Orthopedics</th>

                    </tr>

                </thead>

                <tbody>

                    <template v-for="day in coverageSchedule" :key="day.date">

                        <tr v-for="(shift, index) in day.shifts" :key="day.date + shift.type">

                            <td v-if="index === 0" :rowspan="2" class="date-cell">
                                {{ day.date }}
                            </td>

                            <td class="shift-cell">
                                {{ shift.type }}
                            </td>

                            <!-- Cardiology -->

                            <td>

                                <select v-if="
                                    isEditing &&
                                    activeCell === `${day.date}-${shift.type}-cardiology`
                                " v-model="shift.cardiology" class="physician-dropdown" @blur="closeCellEditor">

                                    <option v-for="doctor in physicians" :key="doctor" :value="doctor">
                                        {{ doctor }}
                                    </option>

                                </select>

                                <span v-else class="doctor-name" @click="
                                    isEditing &&
                                    openCellEditor(
                                        day.date,
                                        shift.type,
                                        'cardiology'
                                    )
                                    ">
                                    {{ shift.cardiology }}
                                </span>

                            </td>

                            <!-- Neurology -->

                            <td>

                                <select v-if="
                                    isEditing &&
                                    activeCell === `${day.date}-${shift.type}-neurology`
                                " v-model="shift.neurology" class="physician-dropdown" @blur="closeCellEditor">

                                    <option v-for="doctor in physicians" :key="doctor" :value="doctor">
                                        {{ doctor }}
                                    </option>

                                </select>

                                <span v-else class="doctor-name" @click="
                                    isEditing &&
                                    openCellEditor(
                                        day.date,
                                        shift.type,
                                        'neurology'
                                    )
                                    ">
                                    {{ shift.neurology }}
                                </span>

                            </td>

                            <!-- Orthopedics -->

                            <td>

                                <select v-if="
                                    isEditing &&
                                    activeCell === `${day.date}-${shift.type}-orthopedics`
                                " v-model="shift.orthopedics" class="physician-dropdown" @blur="closeCellEditor">

                                    <option v-for="doctor in physicians" :key="doctor" :value="doctor">
                                        {{ doctor }}
                                    </option>

                                </select>

                                <span v-else class="doctor-name" @click="
                                    isEditing &&
                                    openCellEditor(
                                        day.date,
                                        shift.type,
                                        'orthopedics'
                                    )
                                    ">
                                    {{ shift.orthopedics }}
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

.physician-dropdown {
    width: 100%;
    padding: 6px 8px;

    border: 1px solid #dbe2ea;
    border-radius: 6px;

    font-size: 13px;
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