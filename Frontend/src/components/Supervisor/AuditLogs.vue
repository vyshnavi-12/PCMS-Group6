<script setup lang="ts">
import { ref, onMounted } from 'vue'
import API from '../../api/axios'

interface AuditLog {
    auditLogId: number
    actionType: string
    entityName: string
    entityRecordId: number
    performedByUserId: number
    createdAt: string
}

const auditLogs = ref<AuditLog[]>([])
const loading = ref(false)
const errorMessage = ref('')

const fetchAuditLogs = async () => {
    loading.value = true
    errorMessage.value = ''

    try {
        const response = await API.get('/AuditLogs')
        auditLogs.value = response.data.data || []
    } catch (error) {
        console.error('Error fetching audit logs:', error)
        errorMessage.value = 'Failed to load audit logs'
    } finally {
        loading.value = false
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

onMounted(() => {
    fetchAuditLogs()
})
</script>

<template>
    <div class="audit-logs-page">
        <div class="table-container">
            <div v-if="loading" class="state-message">
                Loading audit logs...
            </div>

            <div v-else-if="errorMessage" class="error-message">
                {{ errorMessage }}
            </div>

            <div v-else-if="auditLogs.length === 0" class="state-message">
                No audit logs available
            </div>

            <div v-else class="table-wrapper">
                <table class="audit-table">
                    <thead>
                        <tr>
                            <th>Log ID</th>
                            <th>Activity</th>
                            <th>Module</th>
                            <th>Record ID</th>
                            <th>Performed By</th>
                            <th>Date & Time</th>
                        </tr>
                    </thead>

                    <tbody>
                        <tr v-for="log in auditLogs" :key="log.auditLogId">
                            <td>{{ log.auditLogId }}</td>
                            <td>
                                <span class="action-badge">
                                    {{ log.actionType }}
                                </span>
                            </td>
                            <td>{{ log.entityName }}</td>
                            <td>{{ log.entityRecordId }}</td>
                            <td>{{ log.performedByUserId }}</td>
                            <td>{{ formatDateTime(log.createdAt) }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<style scoped>
.audit-logs-page {
    display: flex;
  flex-direction: column;
  gap: 18px;

  height: calc(100vh - 150px);
  min-height: 0;

  overflow: hidden;
}

.table-container {
    flex: 1;
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.04);
    overflow: hidden;
    display: flex;
    flex-direction: column;
}

.table-wrapper {
    height: 100%;
    overflow-y: auto;
    overflow-x: auto;
}

.audit-table {
    width: 100%;
    border-collapse: collapse;
    table-layout: fixed;
}

.audit-table thead {
    position: sticky;
    top: 0;
    z-index: 10;
}

.audit-table th {
    background: #232f72;
    color: white;
    text-align: left;
    padding: 12px 14px;
    font-size: 14px;
    font-weight: 600;
}

.audit-table td {
    padding: 10px 14px;
    border-bottom: 1px solid #e2e8f0;
    font-size: 14px;
    color: #334155;
}

.audit-table tbody tr:hover {
    background: #f8fafc;
}

.action-badge {
    display: inline-block;
    padding: 6px 12px;
    background: #e0f2fe;
    color: #0369a1;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 600;
}

.state-message,
.error-message {
    margin: auto;
    font-size: 16px;
    color: #64748b;
}

.error-message {
    color: #dc2626;
}
</style>