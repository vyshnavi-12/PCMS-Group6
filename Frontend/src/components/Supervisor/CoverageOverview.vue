<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const currentSchedule = {
    week: 'Jun 02 - Jun 15, 2025',
    status: 'PUBLISHED'
}

const coverageData = [
    {
        specialty: 'Cardiology',
        filled: '100%',
        days: [
            'filled',
            'filled',
            'filled',
            'filled',
            'filled',
            'filled',
            'filled'
        ]
    },

    {
        specialty: 'Neurology',
        filled: '88%',
        days: [
            'filled',
            'filled',
            'filled',
            'gap',
            'filled',
            'gap',
            'filled'
        ]
    },

    {
        specialty: 'Orthopedics',
        filled: '82%',
        days: [
            'filled',
            'filled',
            'gap',
            'filled',
            'gap',
            'gap',
            'filled'
        ]
    },

    {
        specialty: 'Emergency',
        filled: '80%',
        days: [
            'filled',
            'gap',
            'filled',
            'filled',
            'filled',
            'filled',
            'filled'
        ]
    }
]

const openCalendar = () => {

    router.push({
        path: '/supervisor/coverage-schedule',

        query: {
            week: currentSchedule.week,
            status: currentSchedule.status
        }
    })
}

</script>

<template>

<div class="coverage-card">

    <div class="header">

        <h3>
            Coverage Overview (Weekly)
        </h3>

        <div class="header-actions">

            <div class="legend">

                <span>
                    <span class="dot filled"></span>
                    Filled
                </span>

                <span>
                    <span class="dot gap"></span>
                    Gap
                </span>

            </div>

            <button
                class="calendar-btn"
                @click="openCalendar"
            >
                View Schedule
            </button>

        </div>

    </div>

    <table>

        <thead>

            <tr>

                <th>Specialty</th>

                <th>Jun 02</th>
                <th>Jun 03</th>
                <th>Jun 04</th>
                <th>Jun 05</th>
                <th>Jun 06</th>
                <th>Jun 07</th>
                <th>Jun 08</th>

                <th>%</th>

            </tr>

        </thead>

        <tbody>

            <tr
                v-for="row in coverageData"
                :key="row.specialty"
            >

                <td class="specialty">
                    {{ row.specialty }}
                </td>

                <td
                    v-for="(status, index) in row.days"
                    :key="index"
                >

                    <span
                        class="status-dot"
                        :class="status"
                    ></span>

                </td>

                <td class="filled-value">
                    {{ row.filled }}
                </td>

            </tr>

        </tbody>

    </table>

</div>

</template>

<style scoped>

.coverage-card {
    background: white;

    border: 1px solid #e2e8f0;
    border-radius: 12px;

    padding: 18px;
}

.header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    margin-bottom: 16px;
}

.header h3 {
    margin: 0;

    font-size: 16px;
    font-weight: 600;

    color: #232f72;
}

.header-actions {
    display: flex;
    align-items: center;
    gap: 16px;
}

.legend {
    display: flex;
    gap: 14px;

    font-size: 12px;
    color: #64748b;
}

.legend span {
    display: flex;
    align-items: center;
    gap: 4px;
}

.dot {
    width: 8px;
    height: 8px;

    border-radius: 50%;
}

.filled {
    background: #16a34a;
}

.gap {
    background: #dc2626;
}

.calendar-btn {
    background: #2563eb;
    color: white;

    border: none;
    border-radius: 6px;

    padding: 8px 12px;

    font-size: 12px;
    font-weight: 600;

    cursor: pointer;
}

table {
    width: 100%;
    border-collapse: collapse;
}

th {
    background: #f8fafc;

    padding: 10px;

    font-size: 12px;
    font-weight: 600;

    color: #64748b;
}

td {
    padding: 12px;
    text-align: center;

    border-top: 1px solid #f1f5f9;
}

.specialty {
    text-align: left;
    font-weight: 600;
    color: #334155;
}

.status-dot {
    width: 10px;
    height: 10px;

    border-radius: 50%;

    display: inline-block;
}

.filled-value {
    font-weight: 700;
    color: #0f172a;
}

</style>