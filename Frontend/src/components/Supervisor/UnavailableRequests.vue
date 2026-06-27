<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import UnavailableRequestCard from './UnavailableRequestCard.vue'
import API from '../../api/axios.ts'
import { useToast } from 'primevue/usetoast'
import unavailableRequestSignalRService from '../../services/unavailableRequestSignalRService'

const toast = useToast()
const activeTab = ref('Open')
const selectedRequest = ref<any | null>(null)
const gapAlerts = ref<any[]>([])
const fetchUnavailableRequests = async () => {
    const unavailableRequest = await API.get("coverageassignments/alerts")
    gapAlerts.value = unavailableRequest.data.data
    console.log(unavailableRequest.data)
}

const formatDate = (dateString: string) => {
    if (!dateString) return ''

    const parts = dateString.split('-')
    const year = Number(parts[0])
    const month = Number(parts[1]) - 1
    const day = Number(parts[2])

    const date = new Date(year, month, day)

    return date.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric'
    })
}

const openCount = computed(() => {
    return gapAlerts.value.filter(g => g.status === 'Open').length;
});

const resolvedCount = computed(() => {
    return gapAlerts.value.filter(g => g.status === 'Resolved').length;
});
const filteredGaps = computed(() => {
    if (activeTab.value === 'Open') {
        return gapAlerts.value.filter(
            gap => gap.status === 'Open'
        )
    }

    return gapAlerts.value.filter(
        gap => gap.status === 'Resolved'
    )
})


const getStatusClass = (status: string) => {
    switch (status) {
        case 'Open':
            return 'open'
        case 'Resolved':
            return 'resolved'
        default:
            return ''
    }
}

const formatDateTime = (dateString: string) => {
    if (!dateString) return '';

    const date = new Date(dateString);

    return date.toLocaleString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });
}
const viewRequest = (gap: any) => {
    selectedRequest.value = gap
}

const closeRequest = () => {
    selectedRequest.value = null
}

const handleRequestUpdated = async (message: string) => {
    toast.add({
        severity: 'success',
        summary: 'Success',
        detail: message,
        life: 3000
    })

    selectedRequest.value = null
    await fetchUnavailableRequests()
}

onMounted(() => {
    fetchUnavailableRequests()

    unavailableRequestSignalRService.onNewUnavailableRequest(async () => {
        console.log('New unavailable request received')

        await fetchUnavailableRequests()

        toast.add({
            severity: 'info',
            summary: 'New Request',
            detail: 'New unavailable request received',
            life: 3000
        })
    })
})
</script>

<template>
    <div class="coverage-gap-page">

        <template v-if="!selectedRequest">

            <div class="toolbar">
                <div class="tabs">

                    <button class="tab-button" :class="{ active: activeTab === 'Open' }" @click="activeTab = 'Open'">
                        Open ({{ openCount }})
                    </button>

                    <button class="tab-button" :class="{ active: activeTab === 'Resolved' }"
                        @click="activeTab = 'Resolved'">
                        Resolved ({{ resolvedCount }})
                    </button>

                </div>
            </div>

            <div class="table-card">
                <table>
                    <thead>
                        <tr>
                            <th>Coverage Date</th>
                            <th>Specialty</th>
                            <th>Shift</th>
                            <th>Requestd By</th>
                            <th>Status</th>
                            <th>Created At</th>
                            <th v-if="activeTab === 'Open'">Actions</th>
                        </tr>
                    </thead>

                    <tbody>
                        <tr v-for="gap in filteredGaps" :key="gap.id">

                            <td>{{ formatDate(gap.date) }}</td>
                            <td>{{ gap.specialty }}</td>
                            <td>{{ gap.shift }}</td>
                            <td>{{ gap.requestedBy }}</td>

                            <td>
                                <span class="status-badge" :class="getStatusClass(gap.status)">
                                    {{ gap.status }}
                                </span>
                            </td>

                            <td>{{ formatDateTime(gap.createdAt) }}</td>

                            <td v-if="activeTab === 'Open'">
                                <button class="view-btn" @click="viewRequest(gap)">
                                    View
                                </button>
                            </td>

                        </tr>

                        <tr v-if="filteredGaps.length === 0">
                            <td :colspan="activeTab === 'Open' ? 7 : 6" class="empty-state">
                                {{ activeTab === 'Open'
                                    ? 'No open unavailable requests'
                                    : 'No resolved unavailable requests'
                                }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

        </template>

        <template v-else>

            <UnavailableRequestCard 
    :request="selectedRequest" 
    @close="closeRequest"
    @updated="handleRequestUpdated"
/>
        </template>

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
    background: #fcfee2;
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

.empty-state {
    text-align: center;
    padding: 30px;
    color: #64748b;
}
</style>