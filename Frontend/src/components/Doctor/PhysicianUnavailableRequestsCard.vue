<script setup lang="ts">
import { onMounted } from 'vue'
import { usePhysicianUnavailableRequestsStore } from '../../stores/physicianUnavailableRequestsStore'

const unavailableStore = usePhysicianUnavailableRequestsStore()

// Replace with actual logged-in physician ID
const physicianId = 123  

onMounted(() => {
  unavailableStore.fetchOpenUnavailableCount(physicianId)
})
</script>

<template>
  <div class="stat-card">
    <div class="icon red"><i class="pi pi-exclamation-triangle"></i></div>
    <div>
      <div class="stat-title">Unavailable Requests</div>
      <div class="stat-value">{{ unavailableStore.openUnavailableCount }}</div>
      <div class="stat-subtitle">Sent by me (Open)</div>
    </div>
  </div>
</template>
