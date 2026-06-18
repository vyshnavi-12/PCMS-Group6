<script setup lang="ts">
import { ref, computed } from 'vue'

const activeTab = ref('All')

const showFilter = ref(false)

const selectedFilter = ref('')

const requests = ref([
    {
        id: 'SWP-0011',
        coverageDate: 'May 25, 2025',
        shift: 'NIGHT',
        requestedBy: 'Dr. Davis',
        targetPhysician: 'Dr. Miller',
        status: 'PENDING',
        requestedAt: 'May 18, 2025 10:30 AM'
    },
    {
        id: 'SWP-0010',
        coverageDate: 'May 24, 2025',
        shift: 'DAY',
        requestedBy: 'Dr. Williams',
        targetPhysician: 'Dr. Brown',
        status: 'PENDING',
        requestedAt: 'May 18, 2025 09:30 AM'
    },
    {
        id: 'SWP-0009',
        coverageDate: 'May 23, 2025',
        shift: 'NIGHT',
        requestedBy: 'Dr. Smith',
        targetPhysician: 'Dr. Johnson',
        status: 'APPROVED',
        requestedAt: 'May 17, 2025 04:15 PM'
    },
    {
        id: 'SWP-0008',
        coverageDate: 'May 22, 2025',
        shift: 'DAY',
        requestedBy: 'Dr. Taylor',
        targetPhysician: 'Dr. Brown',
        status: 'DECLINED',
        requestedAt: 'May 17, 2025 01:20 PM'
    }
])

const filteredRequests = computed(() => {
    let data = requests.value

    if (activeTab.value === 'Pending') {
        return data.filter(
            request => request.status === 'PENDING'
        )
    }

    if (selectedFilter.value) {
        data = data.filter(
            request => request.status === selectedFilter.value
        )
    }

    return data
})

const approveRequest = (requestId: string) => {
    const request = requests.value.find(
        request => request.id === requestId
    )

    if (request) {
        request.status = 'APPROVED'
    }
}

const declineRequest = (requestId: string) => {
    const request = requests.value.find(
        request => request.id === requestId
    )

    if (request) {
        request.status = 'DECLINED'
    }
}

const clearFilter = () => {
    selectedFilter.value = ''
    showFilter.value = false
}

const openAllTab = () => {
    activeTab.value = 'All'
    selectedFilter.value = ''
    showFilter.value = false
}

const applyFilter = (status: string) => {
    selectedFilter.value = status
    showFilter.value = false
}

const getStatusClass = (status: string) => {
    switch (status) {
        case 'APPROVED':
            return 'approved'

        case 'DECLINED':
            return 'declined'

        case 'PENDING':
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

                <button class="tab-button" :class="{ active: activeTab === 'All' }" @click="openAllTab">
                    All
                </button>

                <button class="tab-button" :class="{ active: activeTab === 'Pending' }" @click="
                    activeTab = 'Pending';
                showFilter = false;
                ">
                    Pending
                </button>

            </div>

            <div v-if="activeTab === 'All'" class="filter-container">

                <button class="filter-btn" @click="showFilter = !showFilter">
                    <i class="pi pi-filter"></i>
                    Filter
                </button>

                <div v-if="showFilter" class="filter-popup">

                    <h4>Status</h4>

                    <label>
                        <input type="radio" :checked="selectedFilter === 'PENDING'" @change="applyFilter('PENDING')" />
                        Pending
                    </label>

                    <label>
                        <input type="radio" :checked="selectedFilter === 'APPROVED'"
                            @change="applyFilter('APPROVED')" />
                        Approved
                    </label>

                    <label>
                        <input type="radio" :checked="selectedFilter === 'DECLINED'"
                            @change="applyFilter('DECLINED')" />
                        Declined
                    </label>

                    <div class="filter-actions">

                        <button class="clear-btn" @click="clearFilter">
                            Clear
                        </button>

                    </div>

                </div>

            </div>

        </div>

        <div class="table-card">

            <table>

                <thead>
                    <tr>
                        <th>Request ID</th>
                        <th>Coverage Date</th>
                        <th>Shift</th>
                        <th>Requested By</th>
                        <th>Target Physician</th>
                        <th>Status</th>
                        <th>Requested At</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>

                    <tr v-for="request in filteredRequests" :key="request.id">

                        <td>{{ request.id }}</td>

                        <td>{{ request.coverageDate }}</td>

                        <td>{{ request.shift }}</td>

                        <td>{{ request.requestedBy }}</td>

                        <td>{{ request.targetPhysician }}</td>

                        <td>

                            <span class="status-badge" :class="getStatusClass(request.status)">
                                {{ request.status }}
                            </span>

                        </td>

                        <td>{{ request.requestedAt }}</td>

                        <td>

                            <template v-if="request.status === 'PENDING'">

                                <button class="approve-btn" @click="approveRequest(request.id)">
                                    Approve
                                </button>

                                <button class="decline-btn" @click="declineRequest(request.id)">
                                    Decline
                                </button>

                            </template>

                            <span v-else>
                                -
                            </span>

                        </td>

                    </tr>

                    <tr v-if="filteredRequests.length === 0">

                        <td colspan="8" class="empty-state">
                            No swap requests found
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

.filter-container {
    position: relative;
}

.filter-btn {
    display: flex;
    align-items: center;
    gap: 8px;

    background: white;
    border: 1px solid #dbe2ea;
    border-radius: 8px;

    padding: 8px 14px;

    font-size: 14px;
    font-weight: 500;
    color: #475569;

    cursor: pointer;
    transition: all 0.2s ease;
}

.filter-btn:hover {
    border-color: #232f72;
    color: #232f72;
}

.filter-popup {
    position: absolute;
    top: 48px;
    right: 0;

    width: 220px;

    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 10px;

    padding: 16px;

    box-shadow: 0 10px 25px rgba(15, 23, 42, 0.12);

    z-index: 100;
}

.filter-popup h4 {
    margin: 0 0 12px;
    color: #232f72;
    font-size: 14px;
    font-weight: 600;
}

.filter-popup label {
    display: flex;
    align-items: center;
    gap: 8px;

    margin-bottom: 10px;

    font-size: 14px;
    color: #334155;

    cursor: pointer;
}

.filter-popup input[type='radio'] {
    cursor: pointer;
}

.filter-actions {
    display: flex;
    justify-content: flex-end;
    margin-top: 14px;
}

.clear-btn {
    background: white;
    border: 1px solid #dbe2ea;
    border-radius: 6px;

    padding: 6px 12px;

    cursor: pointer;
    font-size: 13px;
}

.clear-btn:hover {
    background: #f8fafc;
}

.apply-btn {
    background: #232f72;
    color: white;

    border: none;
    border-radius: 6px;

    padding: 6px 12px;

    cursor: pointer;
    font-size: 13px;
}

.apply-btn:hover {
    background: #1b255c;
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