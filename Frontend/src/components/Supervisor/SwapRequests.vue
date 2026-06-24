<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const activeTab = ref('All')
const requests = ref<any[]>([])

const fetchSupervisorRequests = async () => {
    try {
        const response = await axios.get(
            'https://localhost:7119/api/Supervisor/SwapRequests/my',
            { withCredentials: true }
        )

        requests.value = response.data.data
    } catch (error) {
        console.error(error)
    }
}

onMounted(() => {
    fetchSupervisorRequests()
})

const filteredRequests = computed(() => {
    if (activeTab.value === 'Pending') {
        return requests.value.filter(
            request => request.status === 'TARGET_ACCEPTED'
        )
    }

    return requests.value
})

const approveRequest = async (requestId: number) => {
    try {
        await axios.put(
            `https://localhost:7119/api/Supervisor/SwapRequests/${requestId}/approve`,
            {},
            { withCredentials: true }
        )

        fetchSupervisorRequests()
    } catch (error) {
        console.error(error)
    }
}

const declineRequest = async (requestId: number) => {
    try {
        await axios.put(
            `https://localhost:7119/api/Supervisor/SwapRequests/${requestId}/reject`,
            {},
            { withCredentials: true }
        )

        fetchSupervisorRequests()
    } catch (error) {
        console.error(error)
    }
}

const getStatusClass = (status: string) => {
    switch (status) {
        case 'SUPERVISOR_APPROVED':
            return 'approved'

        case 'TARGET_DECLINED':
        case 'SUPERVISOR_DECLINED':
            return 'declined'

        case 'PENDING_TARGET':
        case 'TARGET_ACCEPTED':
            return 'pending'

        default:
            return ''
    }
}
</script>

<template>
    <div class="swap-requests-page">

        <div class="toolbar">

            <div class="tabs">

                <button class="tab-button" :class="{ active: activeTab === 'All' }" @click="activeTab = 'All'">
                    All
                </button>

                <button class="tab-button" :class="{ active: activeTab === 'Pending' }" @click="activeTab = 'Pending'">
                    Pending
                </button>

            </div>

        </div>

        <div class="table-card">

            <table>

                <thead>
                    <tr>
                        <th>Coverage Date</th>
                        <th>Shift</th>
                        <th>Requested By</th>
                        <th>Requested With</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>

                    <tr v-for="request in filteredRequests" :key="request.swapRequestId">
                        <td>{{ request.date }}</td>
                        <td>{{ request.shift }}</td>
                        <td>{{ request.requestedBy }}</td>
                        <td>{{ request.targetPhysician }}</td>

                        <td>
                            <span class="status-badge" :class="getStatusClass(request.status)">
                                {{ request.status }}
                            </span>
                        </td>

                        <td>

                            <template v-if="request.status === 'TARGET_ACCEPTED'">
                                <button class="approve-btn" @click="approveRequest(request.swapRequestId)">
                                    Approve
                                </button>

                                <button class="decline-btn" @click="declineRequest(request.swapRequestId)">
                                    Decline
                                </button>
                            </template>

                            <span v-else>
                                -
                            </span>

                        </td>
                    </tr>

                    <tr v-if="filteredRequests.length === 0">
                        <td colspan="6" class="empty-state">
                            {{ activeTab === 'Pending'
                                ? 'No pending requests'
                                : 'No swap requests found'
                            }}
                        </td>
                    </tr>

                </tbody>

            </table>

        </div>

    </div>
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
    color: #64748b;
    font-size: 14px;
    font-weight: 600;
    padding-bottom: 8px;
}

.tab-button.active {
    color: #232f72;
    border-bottom: 2px solid #232f72;
}

.table-card {
    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
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
    border-bottom: 1px solid #e2e8f0;
}

td {
    padding: 16px;
    font-size: 14px;
    color: #334155;
    border-top: 1px solid #f1f5f9;
}

.status-badge {
    display: inline-block;
    padding: 5px 10px;
    border-radius: 6px;
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

.approve-btn {
    background: #ecfdf3;
    color: #15803d;
    border: 1px solid #bbf7d0;
    border-radius: 6px;
    padding: 7px 14px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    margin-right: 6px;
}

.approve-btn:hover {
    background: #dcfce7;
}

.decline-btn {
    background: #fef2f2;
    color: #dc2626;
    border: 1px solid #fecaca;
    border-radius: 6px;
    padding: 7px 14px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
}

.decline-btn:hover {
    background: #fee2e2;
}

.empty-state {
    text-align: center;
    color: #94a3b8;
    padding: 30px;
}

td:last-child {
    white-space: nowrap;
}

@media (max-width: 1024px) {
    .table-card {
        overflow-x: auto;
    }

    table {
        min-width: 1000px;
    }
}
</style>