<script setup lang="ts">

const publishedSchedules = 2
const draftSchedules = 2

const latestPublished = 'Jun 02 – Jun 15, 2025'
const nextDraft = 'Jun 30 – Jul 13, 2025'

</script>

<template>

<div class="schedule-status-card">

    <div class="card-header">

        <h3>
            Schedule Status
        </h3>

        <i class="pi pi-calendar"></i>

    </div>

    <div class="summary-grid">

        <div class="summary-box">

            <div class="summary-value published">
                {{ publishedSchedules }}
            </div>

            <div class="summary-label">
                Published
            </div>

        </div>

        <div class="summary-box">

            <div class="summary-value draft">
                {{ draftSchedules }}
            </div>

            <div class="summary-label">
                Drafts
            </div>

        </div>

    </div>

    <div class="info-section">

        <div class="info-row">

            <span class="label">
                Latest Published
            </span>

            <span class="value">
                {{ latestPublished }}
            </span>

        </div>

        <div class="info-row">

            <span class="label">
                Next Draft
            </span>

            <span class="value">
                {{ nextDraft }}
            </span>

        </div>

    </div>

</div>

</template>

<style scoped>

.schedule-status-card {
    background: white;

    border: 1px solid #e2e8f0;
    border-radius: 12px;

    padding: 18px;

    height: 100%;

    display: flex;
    flex-direction: column;
}

.card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;

    margin-bottom: 18px;
}

.card-header h3 {
    margin: 0;

    font-size: 16px;
    font-weight: 600;

    color: #1e293b;
}

.card-header i {
    color: #64748b;
    font-size: 18px;
}

.summary-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 12px;

    margin-bottom: 18px;
}

.summary-box {
    background: #f8fafc;

    border-radius: 10px;

    padding: 14px;

    text-align: center;
}

.summary-value {
    font-size: 28px;
    font-weight: 700;
}

.published {
    color: #16a34a;
}

.draft {
    color: #f97316;
}

.summary-label {
    margin-top: 4px;

    font-size: 12px;
    color: #64748b;
}

.info-section {
    display: flex;
    flex-direction: column;
    gap: 10px;

    margin-bottom: 20px;
}

.info-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.label {
    font-size: 13px;
    color: #64748b;
}

.value {
    font-size: 13px;
    font-weight: 600;
    color: #334155;
}

.timeline {
    display: flex;
    flex-direction: column;
    gap: 14px;
}

.timeline-item {
    display: flex;
    gap: 10px;
    align-items: flex-start;
}

.timeline-dot {
    width: 10px;
    height: 10px;

    border-radius: 50%;

    margin-top: 5px;

    flex-shrink: 0;
}

.published-dot {
    background: #16a34a;
}

.draft-dot {
    background: #f97316;
}

.timeline-content {
    display: flex;
    flex-direction: column;
}

.period {
    font-size: 13px;
    font-weight: 600;
    color: #334155;
}

.status {
    font-size: 12px;
    margin-top: 2px;
}

.published-text {
    color: #16a34a;
}

.draft-text {
    color: #f97316;
}

</style>