<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import NewSwapRequest from './NewSwapRequest.vue'

const route = useRoute()

onMounted(() => {
  if (route.query.new === 'true') {
    showNewRequest.value = true
  }
})

const showNewRequest = ref(false)

const activeTab = ref('My Requests')

const openNewRequest = () => {
    showNewRequest.value = true
}

const goBackToList = () => {
    showNewRequest.value = false
}

/* =========================
   MY REQUESTS
========================= */

const myRequests = ref([
    {
        id: 'SWP-0012',
        date: 'May 23, 2025',
        shift: 'NIGHT',
        requestedWith: 'Dr. Michael Brown',
        status: 'PENDING',
        requestedOn: 'May 18, 2025 10:30 AM'
    },
    {
        id: 'SWP-0011',
        date: 'May 20, 2025',
        shift: 'NIGHT',
        requestedWith: 'Dr. Sarah Davis',
        status: 'APPROVED',
        requestedOn: 'May 15, 2025 09:15 AM'
    },
    {
        id: 'SWP-0010',
        date: 'May 16, 2025',
        shift: 'DAY',
        requestedWith: 'Dr. James Wilson',
        status: 'DECLINED',
        requestedOn: 'May 10, 2025 02:20 PM'
    },
    {
        id: 'SWP-0009',
        date: 'May 13, 2025',
        shift: 'NIGHT',
        requestedWith: 'Dr. Emily Clark',
        status: 'CANCELLED',
        requestedOn: 'May 09, 2025 11:45 AM'
    }
])

/* =========================
   REQUESTS TO ME
========================= */

const requestsToMe = ref([
    {
        id: 'SWP-0021',
        date: 'May 25, 2025',
        shift: 'DAY',
        requestedBy: 'Dr. James Wilson',
        reason: 'Family emergency',
        requestedOn: 'May 20, 2025 11:00 AM',
        status: 'PENDING'
    },
    {
        id: 'SWP-0022',
        date: 'May 27, 2025',
        shift: 'NIGHT',
        requestedBy: 'Dr. Emily Clark',
        reason: 'Medical appointment',
        requestedOn: 'May 22, 2025 02:30 PM',
        status: 'PENDING'
    }
])

/* =========================
   ACCEPT REQUEST
========================= */

const acceptRequest = (requestId: string) => {
    const request = requestsToMe.value.find(
        request => request.id === requestId
    )

    if (request) {
        request.status = 'APPROVED'
    }
}

/* =========================
   DECLINE REQUEST
========================= */

const declineRequest = (requestId: string) => {
    const request = requestsToMe.value.find(
        request => request.id === requestId
    )

    if (request) {
        request.status = 'DECLINED'
    }
}

/* =========================
   STATUS BADGES
========================= */

const getStatusClass = (status: string) => {
    switch (status) {
        case 'APPROVED':
            return 'approved'

        case 'DECLINED':
            return 'declined'

        case 'PENDING':
            return 'pending'

        case 'CANCELLED':
            return 'cancelled'

        default:
            return ''
    }
}
</script>

<template>

    <div v-if="!showNewRequest" class="swap-requests-page">

        <div class="toolbar">

            <div class="tabs">

                <button class="tab-button" :class="{ active: activeTab === 'My Requests' }"
                    @click="activeTab = 'My Requests'">
                    My Requests
                </button>

                <button class="tab-button" :class="{ active: activeTab === 'Requests To Me' }"
                    @click="activeTab = 'Requests To Me'">
                    Requests To Me
                </button>

            </div>

            <div class="actions">

                <button class="new-request-btn" @click="openNewRequest">
                    + New Swap Request
                </button>

            </div>

        </div>

        <div class="table-card">

            <!-- =========================
                 MY REQUESTS
            ========================== -->

            <table v-if="activeTab === 'My Requests'">

                <thead>
                    <tr>
                        <th>Request ID</th>
                        <th>Date</th>
                        <th>Shift</th>
                        <th>Requested With</th>
                        <th>Status</th>
                        <th>Requested On</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>

                    <tr v-for="request in myRequests" :key="request.id">
                        <td>{{ request.id }}</td>

                        <td>{{ request.date }}</td>

                        <td>{{ request.shift }}</td>

                        <td>{{ request.requestedWith }}</td>

                        <td>
                            <span class="status-badge" :class="getStatusClass(request.status)">
                                {{ request.status }}
                            </span>
                        </td>

                        <td>{{ request.requestedOn }}</td>

                        <td>
                            <button class="view-btn">
                                View
                            </button>
                        </td>

                    </tr>

                </tbody>

            </table>

            <!-- =========================
                 REQUESTS TO ME
            ========================== -->

            <table v-else>

                <thead>
                    <tr>
                        <th>Request ID</th>
                        <th>Date</th>
                        <th>Shift</th>
                        <th>Requested By</th>
                        <th>Reason</th>
                        <th>Requested On</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>

                    <tr v-for="request in requestsToMe" :key="request.id">
                        <td>{{ request.id }}</td>

                        <td>{{ request.date }}</td>

                        <td>{{ request.shift }}</td>

                        <td>{{ request.requestedBy }}</td>

                        <td>{{ request.reason }}</td>

                        <td>{{ request.requestedOn }}</td>


                        <td class="action-cell">

                            <button class="approve-btn" @click="acceptRequest(request.id)">
                                ✓ Accept
                            </button>

                            <button class="decline-btn" @click="declineRequest(request.id)">
                                ✕ Decline
                            </button>

                        </td>

                    </tr>

                </tbody>

            </table>

        </div>

    </div>

    <NewSwapRequest v-else @back="goBackToList" />

</template>

<style scoped>
.swap-requests-page {
    display: flex;
    flex-direction: column;
    gap: 20px;
}

.toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
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
}

.tab-button.active {
    color: #232f72;
    border-bottom: 2px solid #232f72;
}

.actions {
    display: flex;
    gap: 12px;
}

.new-request-btn {
    background: #232f72;
    color: white;
    border: none;
    border-radius: 6px;
    padding: 10px 16px;
    cursor: pointer;
    font-weight: 600;
}

.table-card {
    background: white;
    border-radius: 10px;
    border: 1px solid #e2e8f0;
    overflow: hidden;
}

table {
    width: 100%;
    border-collapse: collapse;
}

thead {
    background: #f8fafc;
}

th {
    text-align: left;
    padding: 14px 16px;
    font-size: 13px;
    color: #64748b;
}

td {
    padding: 16px;
    border-top: 1px solid #e2e8f0;
    font-size: 14px;
    color: #334155;
}

.status-badge {
    padding: 4px 10px;
    border-radius: 4px;
    font-size: 11px;
    font-weight: 700;
}

.pending {
    background: #fef3c7;
    color: #b45309;
}

.approved {
    background: #dcfce7;
    color: #15803d;
}

.declined {
    background: #fee2e2;
    color: #dc2626;
}

.cancelled {
    background: #e5e7eb;
    color: #6b7280;
}

.view-btn {
    background: #eef4ff;
    color: #2563eb;
    border: none;
    border-radius: 6px;
    padding: 6px 14px;
    cursor: pointer;
    font-weight: 600;
}

.approve-btn {
    background: #ecfdf3;
    color: #15803d;
    border: 1px solid #bbf7d0;
    border-radius: 6px;
    padding: 6px 14px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    margin-right: 6px;
    transition: all 0.2s ease;
}

.approve-btn:hover {
    background: #dcfce7;
}

.decline-btn {
    background: #fef2f2;
    color: #dc2626;
    border: 1px solid #fecaca;
    border-radius: 6px;
    padding: 6px 14px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
}

.decline-btn:hover {
    background: #fee2e2;
}

.action-cell {
    white-space: nowrap;
}

@media (max-width: 1024px) {
    .table-card {
        overflow-x: auto;
    }

    table {
        min-width: 900px;
    }
}
</style>