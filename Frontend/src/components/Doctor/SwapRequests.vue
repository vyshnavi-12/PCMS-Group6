<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRoute } from 'vue-router'
import NewSwapRequest from './NewSwapRequest.vue'

import { useDoctorSwapRequestsStore } from '../../stores/doctorSwapRequestsStore'

const route = useRoute()

const swapRequestsStore = useDoctorSwapRequestsStore()

const showNewRequest = ref(false)
const activeTab = ref('My Requests')

const { myRequests, requestsToMe } = storeToRefs(swapRequestsStore)

const openNewRequest = () => {
    showNewRequest.value = true
}

const goBackToList = () => {
    showNewRequest.value = false
}

const fetchSwapRequests = async () => {
    await swapRequestsStore.fetchDoctorRequests()
}

const acceptRequest = async (requestId: number) => {
    try {
        await swapRequestsStore.acceptRequest(requestId)
    } catch (error) {
        console.error(error)
    }
}

const declineRequest = async (requestId: number) => {
    try {
        await swapRequestsStore.declineRequest(requestId)
    } catch (error) {
        console.error(error)
    }
}

const getStatusClass = (status: string) => {
    switch (status) {
        case 'TARGET_ACCEPTED':
        case 'SUPERVISOR_APPROVED':
            return 'approved'

        case 'TARGET_DECLINED':
        case 'SUPERVISOR_DECLINED':
            return 'declined'

        case 'PENDING_TARGET':
            return 'pending'

        default:
            return ''
    }
}

const handleRequestCreated = async () => {
    showNewRequest.value = false
    activeTab.value = 'My Requests'

    await fetchSwapRequests()
}

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

const formatDateTime = (dateString: string) => {
    if (!dateString) return '-'

    const date = new Date(dateString.replace(' ', 'T'))

    return date.toLocaleString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    })
}

onMounted(async () => {
    if (route.query.new === 'true') {
        showNewRequest.value = true
    }

    if (route.query.tab === 'requests-to-me') {
        activeTab.value = 'Requests To Me'
    } else {
        activeTab.value = 'My Requests'
    }

    await fetchSwapRequests()
})
</script>

<template>

    <div v-if="!showNewRequest" class="swap-requests-page">

        <div class="toolbar">

            <div class="tabs">

                <button class="tab-button" :class="{ active: activeTab === 'My Requests' }"
                    @click="activeTab = 'My Requests'">
                    My Requests ({{ myRequests.length }})
                </button>

                <button class="tab-button" :class="{ active: activeTab === 'Requests To Me' }"
                    @click="activeTab = 'Requests To Me'">
                    Requests To Me ({{ requestsToMe.length }})
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
                        <th>My Coverage Date</th>
                        <th>Requested Swap Date</th>
                        <th>Shift</th>
                        <th>Requested With</th>
                        <th>Reason</th>
                        <th>Requested On</th>
                        <th>Status</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="request in myRequests" :key="request.swapRequestId">
                        <td>{{ formatDate(request.currentDate) }}</td>
                        <td>{{ formatDate(request.requestedDate) }}</td>
                        <td>{{ request.shift }}</td>
                        <td>{{ request.requestedWith }}</td>
                        <td>{{ request.reason }}</td>
                        <td>{{ formatDateTime(request.requestedOn) }}</td>
                        <td>
                            <span class="status-badge" :class="getStatusClass(request.status)">
                                {{ request.status }}
                            </span>
                        </td>
                    </tr>

                    <tr v-if="myRequests.length === 0">
                        <td colspan="6" class="empty-state">
                            No swap requests found
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
                        <th>Requester Coverage Date</th>
                        <th>My Coverage Date</th>
                        <th>Shift</th>
                        <th>Requested By</th>
                        <th>Reason</th>
                        <th>Requested On</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>
                    <tr v-for="request in requestsToMe" :key="request.swapRequestId">

                        <td>{{ formatDate(request.currentDate) }}</td>
                        <td>{{ formatDate(request.newDate) }}</td>
                        <td>{{ request.shift }}</td>
                        <td>{{ request.requestedBy }}</td>
                        <td>{{ request.reason }}</td>
                        <td>{{ formatDateTime(request.requestedOn) }}</td>

                        <td>
                            <span class="status-badge" :class="getStatusClass(request.status)">
                                {{ request.status }}
                            </span>
                        </td>

                        <td class="action-cell">

                            <template v-if="request.status === 'PENDING_TARGET'">
                                <button class="approve-btn" @click="acceptRequest(request.swapRequestId)">
                                    ✓ Accept
                                </button>

                                <button class="decline-btn" @click="declineRequest(request.swapRequestId)">
                                    ✕ Decline
                                </button>
                            </template>

                            <span v-else>
                                -
                            </span>

                        </td>

                    </tr>

                    <tr v-if="requestsToMe.length === 0">
                        <td colspan="7" class="empty-state">
                            No requests available
                        </td>
                    </tr>
                </tbody>

            </table>

        </div>

    </div>

    <NewSwapRequest v-else @back="goBackToList" @requestCreated="handleRequestCreated" />

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

.empty-state {
    text-align: center;
    color: #94a3b8;
    font-size: 14px;
    padding: 28px;
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