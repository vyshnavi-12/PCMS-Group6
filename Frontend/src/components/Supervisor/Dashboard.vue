<script setup lang="ts">
import CoverageOverview from './CoverageOverview.vue'
import SpecialityCoverageStatus from './ScheduleStatus.vue';
</script>

<template>

  <div class="dashboard-page">

    <div class="stats-grid">

      <!-- Active Schedules -->

      <div class="stat-card">

        <div class="stat-header">

          <div class="icon blue">
            <i class="pi pi-calendar"></i>
          </div>

          <div>

            <div class="stat-title">
              Active Schedules
            </div>

            <div class="stat-value">
              2
            </div>

          </div>

        </div>

        <div class="stat-footer">
          May 19 – May 25, 2025
        </div>

      </div>

      <!-- Coverage Filled -->

      <div class="stat-card">

        <div class="stat-header">

          <div class="icon green">
            <i class="pi pi-check-circle"></i>
          </div>

          <div>

            <div class="stat-title">
              Coverage Filled
            </div>

            <div class="stat-value">
              86%
            </div>

          </div>

        </div>

        <div class="coverage-text">
          142 / 165 Assignments
        </div>

        <div class="progress-bar">
          <div class="progress-fill" style="width: 86%;"></div>
        </div>

      </div>

      <!-- Coverage Gaps -->

      <div class="stat-card">

        <div class="stat-header">

          <div class="icon orange">
            <i class="pi pi-exclamation-triangle"></i>
          </div>

          <div>

            <div class="stat-title">
              Coverage Gaps
            </div>

            <div class="stat-value orange-text">
              3
            </div>

          </div>

        </div>

        <div class="stat-footer">
          Open Gaps
        </div>

        <button class="link-btn">
          View Details
          <i class="pi pi-arrow-right"></i>
        </button>

      </div>

      <!-- Swap Requests -->

      <div class="stat-card">

        <div class="stat-header">

          <div class="icon purple">
            <i class="pi pi-arrow-right-arrow-left"></i>
          </div>

          <div>

            <div class="stat-title">
              Swap Requests
            </div>

            <div class="stat-value purple-text">
              4
            </div>

          </div>

        </div>

        <div class="stat-footer">
          Pending
        </div>

        <button class="link-btn">
          View Requests
          <i class="pi pi-arrow-right"></i>
        </button>

      </div>

    </div>

    <div class="dashboard-content">

      <div class="left-panel">
        <CoverageOverview />
      </div>

      <div class="right-panel">
        <SpecialityCoverageStatus />
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
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.dashboard-content {
  margin-top: 20px;

  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
}

.left-panel,
.right-panel {
  min-width: 0;
}

.placeholder-card {
  background: white;

  border: 1px solid #e2e8f0;
  border-radius: 12px;

  padding: 20px;

  min-height: 420px;
}

.stat-card {
  background: white;

  border: 1px solid #e2e8f0;
  border-radius: 12px;

  padding: 20px;

  min-height: 150px;

  display: flex;
  flex-direction: column;
  justify-content: space-between;

  box-shadow:
    0 2px 8px rgba(15, 23, 42, 0.04);
}

.stat-header {
  display: flex;
  align-items: flex-start;
  gap: 14px;
}

.icon {
  width: 52px;
  height: 52px;

  border-radius: 14px;

  display: flex;
  align-items: center;
  justify-content: center;

  font-size: 22px;
}

.blue {
  background: #eef4ff;
  color: #2563eb;
}

.green {
  background: #eaf8ee;
  color: #16a34a;
}

.orange {
  background: #fff3e7;
  color: #f97316;
}

.purple {
  background: #f5ecff;
  color: #9333ea;
}

.stat-title {
  font-size: 13px;
  font-weight: 600;
  color: #475569;

  margin-bottom: 4px;
}

.stat-value {
  font-size: 36px;
  font-weight: 700;
  color: #0f172a;
  line-height: 1;
}

.orange-text {
  color: #f97316;
}

.purple-text {
  color: #9333ea;
}

.stat-footer {
  font-size: 13px;
  color: #64748b;
}

.coverage-text {
  font-size: 13px;
  color: #475569;
}

.progress-bar {
  width: 100%;
  height: 6px;

  background: #e2e8f0;
  border-radius: 999px;

  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: #2563eb;
  border-radius: 999px;
}

.link-btn {
  display: flex;
  align-items: center;
  gap: 6px;

  padding: 0;

  border: none;
  background: transparent;

  color: #2563eb;

  font-size: 13px;
  font-weight: 600;

  cursor: pointer;

  width: fit-content;
}

.link-btn:hover {
  color: #1d4ed8;
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