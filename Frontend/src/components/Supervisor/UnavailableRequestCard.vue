<script setup lang="ts">
import { ref, computed } from 'vue'

const props = defineProps({
  request: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close'])

const replacements = computed(() => [
  props.request.from,
  'Dr. Brown',
  'Dr. Davis',
  'Dr. Miller',
  'Dr. Wilson'
])

const selectedReplacement = ref(replacements.value[0])

const approveRequest = () => {
  alert(`Approved with replacement: ${selectedReplacement.value}`)
}

const declineRequest = () => {
  alert('Request Declined')
}
</script>

<template>
  <div class="request-card">

    <div class="card-header">
      <h2>Unavailable Request</h2>

      <button class="close-btn" @click="emit('close')">
        ✕
      </button>
    </div>

    <div class="details-grid">
      <div class="detail-box">
        <label>Date</label>
        <p>{{ request.date }}</p>
      </div>

      <div class="detail-box">
        <label>Specialty</label>
        <p>{{ request.specialty }}</p>
      </div>

      <div class="detail-box">
        <label>Shift</label>
        <p>{{ request.shift }}</p>
      </div>

      <div class="detail-box">
        <label>Doctor Name</label>
        <p>{{ request.from }}</p>
      </div>
    </div>

    <div class="reason-section">
      <label>Reason</label>
      <div class="reason-box">
        Personal Emergency
      </div>
    </div>

    <div class="replacement-section">
      <label>Replacement</label>

      <select v-model="selectedReplacement" class="replacement-dropdown">
        <option
          v-for="(doctor, index) in replacements"
          :key="doctor"
          :value="doctor"
        >
          {{ index === 0 ? '⭐ ' + doctor : doctor }}
        </option>
      </select>
    </div>

    <div class="actions">
      <button class="decline-btn" @click="declineRequest">
        Decline
      </button>

      <button class="approve-btn" @click="approveRequest">
        Approve
      </button>
    </div>

  </div>
</template>

<style scoped>
.request-card {
    background: white;
    border: 1px solid #e2e8f0;
    border-radius: 14px;
    padding: 24px;
    display: flex;
    flex-direction: column;
    gap: 22px;
}

.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

h2 {
    margin: 0;
    color: #232f72;
    font-size: 22px;
}

.close-btn {
    width: 34px;
    height: 34px;
    border: none;
    border-radius: 50%;
    background: #f1f5f9;
    color: #64748b;
    cursor: pointer;
    font-size: 16px;
    font-weight: 700;

    display: flex;
    align-items: center;
    justify-content: center;

    transition: 0.2s;
}

.close-btn:hover {
    background: #e2e8f0;
    color: #334155;
}

.details-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 16px;
}

.detail-box label,
.reason-section label,
.replacement-section label {
    display: block;
    font-size: 13px;
    color: #64748b;
    margin-bottom: 8px;
    font-weight: 600;
}

.detail-box p {
    margin: 0;
    background: #f8fafc;
    padding: 12px;
    border-radius: 8px;
    color: #334155;
    font-weight: 500;
}

.reason-box {
    background: #f8fafc;
    padding: 14px;
    border-radius: 10px;
    color: #334155;
}

.replacement-dropdown {
    width: 100%;
    padding: 12px;
    border: 1px solid #dbe2ea;
    border-radius: 8px;
    font-size: 14px;
}

.actions {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
}

.decline-btn {
    background: white;
    color: #dc2626;
    border: 1px solid #fecaca;
    border-radius: 8px;
    padding: 10px 18px;
    cursor: pointer;
    font-weight: 600;
}

.decline-btn:hover {
    background: #fef2f2;
}

.approve-btn {
    background: #232f72;
    color: white;
    border: none;
    border-radius: 8px;
    padding: 10px 18px;
    cursor: pointer;
    font-weight: 600;
}

.approve-btn:hover {
    background: #1c265f;
}
</style>