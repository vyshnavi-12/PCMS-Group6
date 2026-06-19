<script setup lang="ts">
import { ref, computed } from 'vue'

const activeTab = ref('Open')

const gapAlerts = ref([
    {
        id: 'ALT-0008',
        date: 'May 23, 2025',
        specialty: 'Neurology',
        shift: 'NIGHT',
        reason: 'Physician Unavailable',
        replacement: 'Dr. Johnson',
        status: 'OPEN',
        createdAt: 'May 18, 2025 08:45 AM'
    },
    {
        id: 'ALT-0007',
        date: 'May 23, 2025',
        specialty: 'Pediatrics',
        shift: 'DAY',
        reason: 'Insufficient Coverage',
        replacement: 'Dr. Brown',
        status: 'OPEN',
        createdAt: 'May 18, 2025 08:40 AM'
    },
    {
        id: 'ALT-0006',
        date: 'May 21, 2025',
        specialty: 'Orthopedics',
        shift: 'NIGHT',
        reason: 'Leave',
        replacement: 'Dr. Brown',
        status: 'OPEN',
        createdAt: 'May 17, 2025 04:30 PM'
    },
    {
        id: 'ALT-0005',
        date: 'May 20, 2025',
        specialty: 'Emergency',
        shift: 'DAY',
        reason: 'Insufficient Coverage',
        replacement: 'Dr. Davis',
        status: 'OPEN',
        createdAt: 'May 17, 2025 02:40 PM'
    },
    {
        id: 'ALT-0004',
        date: 'May 19, 2025',
        specialty: 'Cardiology',
        shift: 'NIGHT',
        reason: 'Physician Unavailable',
        replacement: 'Dr. Miller',
        status: 'OPEN',
        createdAt: 'May 16, 2025 01:15 PM'
    },
    {
        id: 'ALT-0003',
        date: 'May 24, 2025',
        specialty: 'Pediatrics',
        shift: 'NIGHT',
        reason: 'Insufficient Coverage',
        replacement: 'Dr. Johnson',
        status: 'UNRESOLVED'
    },
    {
        id: 'ALT-0002',
        date: 'May 25, 2025',
        specialty: 'General Surgery',
        shift: 'DAY',
        reason: 'Leave',
        replacement: 'Dr. Brown',
        status: 'UNRESOLVED'
    },
    {
        id: 'ALT-0001',
        date: 'May 26, 2025',
        specialty: 'Emergency',
        shift: 'NIGHT',
        reason: 'Physician Unavailable',
        replacement: 'Dr. Davis',
        status: 'RESOLVED'
    }
])

const filteredGaps = computed(() => {

    if (activeTab.value === 'Open') {
        return gapAlerts.value.filter(
            gap => gap.status === 'OPEN'
        )
    }

    return gapAlerts.value.filter(
        gap => gap.status === 'RESOLVED'
    )
})

const getStatusClass = (status: string) => {

    switch (status) {

        case 'OPEN':
            return 'open'

        case 'UNRESOLVED':
            return 'unresolved'

        case 'ESCALATED':
            return 'escalated'

        case 'RESOLVED':
            return 'resolved'

        default:
            return ''
    }
}
</script>

<template>

    <div class="coverage-gap-page">

        <div class="toolbar">

            <div class="tabs">

                <button class="tab-button" :class="{ active: activeTab === 'Open' }" @click="activeTab = 'Open'">
                    Open (5)
                </button>

                <button class="tab-button" :class="{ active: activeTab === 'Resolved' }"
                    @click="activeTab = 'Resolved'">
                    Resolved (1)
                </button>

            </div>

        </div>

        <div class="table-card">

            <table>

                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Specialty</th>
                        <th>Shift</th>
                        <th>Reason</th>
                        <th>Suggested Replacement</th>
                        <th>Status</th>
                        <th>Created At</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>

                    <tr v-for="gap in filteredGaps" :key="gap.id">

                        <td>{{ gap.date }}</td>

                        <td>{{ gap.specialty }}</td>

                        <td>{{ gap.shift }}</td>

                        <td>{{ gap.reason }}</td>

                        <td>{{ gap.replacement }}</td>

                        <td>

                            <span class="status-badge" :class="getStatusClass(gap.status)">
                                {{ gap.status }}
                            </span>

                        </td>

                        <td>{{ gap.createdAt }}</td>

                        <td>

                            <button class="view-btn">
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
.coverage-gap-page {
    display: flex;
    flex-direction: column;
    gap: 18px;

    height: calc(100vh - 150px);
    min-height: 0;

    overflow: hidden;
}

.toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-shrink: 0;
}

.tabs {
    display: flex;
    gap: 24px;
}

.tab-button {
    background: none;
    border: none;
    cursor: pointer;
    padding-bottom: 8px;
    color: #64748b;
    font-weight: 600;
    font-size: 14px;
}

.tab-button.active {
    color: #232f72;
    border-bottom: 2px solid #232f72;
}

.table-card {
    flex: 1;

    min-height: 0;

    background: white;

    border: 1px solid #e2e8f0;
    border-radius: 12px;

    overflow-y: auto;
    overflow-x: auto;
}

table {
    width: max-content;
    min-width: 100%;

    border-collapse: collapse;
}

thead th {
    position: sticky;
    top: 0;

    background: #f8fafc;
    z-index: 10;
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
    padding: 5px 10px;
    border-radius: 6px;
    font-size: 11px;
    font-weight: 700;
}

.open {
    background: #fee2e2;
    color: #dc2626;
}

.unresolved {
    background: #fef3c7;
    color: #b45309;
}

.escalated {
    background: #ede9fe;
    color: #7c3aed;
}

.resolved {
    background: #dcfce7;
    color: #15803d;
}

.view-btn {
    background: #eef4ff;
    color: #2563eb;
    border: none;
    border-radius: 6px;
    padding: 7px 14px;
    cursor: pointer;
    font-weight: 600;
}

.view-btn:hover {
    background: #dbeafe;
}

tbody tr:hover {
    background: #fafbfc;
}
</style>