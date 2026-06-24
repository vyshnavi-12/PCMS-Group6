<script setup lang="ts">
import PhysicianWorkload from './PhysicianWorkload.vue'
import SpecialityRequestsLoad from './SpecialityRequestsLoad.vue';
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
          <div class="stat-title">
            Swap Requests
          </div>

          <div class="stat-value">
            4
          </div>

          <div class="stat-subtitle">
            Pending approvals
          </div>
        </div>
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
            7
          </div>

          <div class="stat-subtitle">
            Open requests
          </div>
        </div>
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
            Jun 30
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

  min-height: 100px;
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