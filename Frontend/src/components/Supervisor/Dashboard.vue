<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import API from '../../api/axios'
import PhysicianWorkload from './PhysicianWorkload.vue'
import SpecialityRequestsLoad from './SpecialityRequestsLoad.vue'
import { useSupervisorSwapRequestsStore } from '../../stores/supervisorSwapRequestsStore'
import unavailableRequestSignalRService from '../../services/unavailableRequestSignalRService'
import supervisorSwapSignalRService from '../../services/supervisorSwapSignalRService'

const router = useRouter()
const swapStore = useSupervisorSwapRequestsStore()

const dashboardDetails = ref({
  swapRequestCount: 0,
  unavailableRequestsCount: 0,
  nextScheduleDate: ''
})

const fetchDashboardDetails = async () => {
  try {
    const response = await API.get('/supervisor/dashboard/details')
    dashboardDetails.value = response.data.data
  } catch (error) {
    console.error('Failed to fetch dashboard details', error)
  }
}

const formatDate = (date: string) => {
  if (!date) return '-'

  return new Date(date).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

onMounted(async () => {
  await fetchDashboardDetails()

  swapStore.fetchTargetAcceptedCount()
  swapStore.fetchSupervisorRequests()

  unavailableRequestSignalRService.onNewUnavailableRequest(async () => {
    console.log('Unavailable dashboard refresh triggered')
    await fetchDashboardDetails()
  })

  supervisorSwapSignalRService.onRefreshSupervisorRequests(async () => {
    console.log('Swap dashboard refresh triggered')
    await fetchDashboardDetails()
  })
})

const goToSwapRequests = () => {
  router.push('/supervisor/swap-requests')
}

const goToUnavailableRequests = () => {
  router.push('/supervisor/unavailable-requests')
}
</script>

<template>
  <div class="dashboard-page">
    <div class="stats-grid">

      <!-- Swap Requests -->
      <div class="stat-card">
        <div class="icon orange">
          <i class="pi pi-arrow-right-arrow-left"></i>
        </div>
        <div>
          <div class="stat-title">Swap Requests</div>
          <div class="stat-value">
            {{ dashboardDetails.swapRequestCount }}
          </div>
          <div class="stat-subtitle">Pending approvals</div>
        </div>
        <button class="view-details-btn" @click="goToSwapRequests">View Details</button>
      </div>

      <!-- Unavailable Requests -->
      <div class="stat-card">
        <div class="icon purple">
          <i class="pi pi-exclamation-circle"></i>
        </div>
        <div>
          <div class="stat-title">
            Unavailable Requests
          </div>

          <div class="stat-value">
            {{ dashboardDetails.unavailableRequestsCount }}
          </div>

          <div class="stat-subtitle">
            Open requests
          </div>
        </div>
        <button class="view-details-btn" @click="goToUnavailableRequests">View Details</button>
      </div>

      <!-- Days Left -->
      <div class="stat-card">
        <div class="icon blue">
          <i class="pi pi-calendar"></i>
        </div>
        <div>
          <div class="stat-title">
            Days Left
          </div>

          <div class="stat-value">
            {{ formatDate(dashboardDetails.nextScheduleDate) }}
          </div>

          <div class="stat-subtitle">
            Next schedule due
          </div>
        </div>
      </div>

    </div>

    <div class="dashboard-content">
      <div class="left-panel">
        <PhysicianWorkload />
      </div>
      <div class="right-panel">
        <SpecialityRequestsLoad />
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-page {
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.dashboard-content {
  margin-top: 20px;
  display: grid;
  grid-template-columns: 0.8fr 1fr;
  gap: 20px;
}

.left-panel,
.right-panel {
  min-width: 0;
  height: 100%;
}

.stat-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 18px;

  display: flex;
  align-items: center;
  gap: 16px;
  position: relative;
  min-height: 140px;
}

.view-details-btn {
  position: absolute;
  bottom: 16px;
  right: 16px;

  background: none;
  border: none;
  color: #2563eb;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
}

.icon {
  width: 54px;
  height: 54px;
  border-radius: 14px;

  display: flex;
  align-items: center;
  justify-content: center;

  font-size: 24px;
  flex-shrink: 0;
}


/* Days Left */
.blue {
  background: #dbeafe;
  color: #2563eb;
}

/* Swap Requests */
.orange {
  background: #fef3c7;
  color: #d97706;
}

/* Unavailable Requests */
.purple {
  background: #ede9fe;
  color: #7c3aed;
}

.stat-title {
  font-size: 14px;
  font-weight: 600;
  color: #475569;
}

.stat-value {
  font-size: 30px;
  font-weight: 700;
  color: #16204d;
  margin: 4px 0;
}

.stat-subtitle {
  font-size: 13px;
  color: #64748b;
}

@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .dashboard-content {
    grid-template-columns: 1fr;
  }
}
</style>