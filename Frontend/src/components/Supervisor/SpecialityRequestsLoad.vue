<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Tooltip,
  Legend
} from 'chart.js'
import { Bar } from 'vue-chartjs'
import API from '../../api/axios'

ChartJS.register(CategoryScale, LinearScale, BarElement, Tooltip, Legend)

interface SpecialtyData {
  specialtyName: string
  requestCount: number
}

const specialtyData = ref<SpecialtyData[]>([])
const isLoading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    const response = await API.get(
      '/coverageassignments/alerts/unavailable-requests-per-specialty'
    )
    specialtyData.value = response.data.data
  } catch (err) {
    error.value = 'Failed to load specialty data.'
    console.error(err)
  } finally {
    isLoading.value = false
  }
})

const chartData = computed(() => ({
  labels: specialtyData.value.map(item => item.specialtyName),
  datasets: [
    {
      label: 'Requests',
      data: specialtyData.value.map(item => item.requestCount),
      backgroundColor: '#9CB080',
      hoverBackgroundColor: '#607456',
      borderRadius: 8,
      borderSkipped: false,
      barThickness: 42
    }
  ]
}))

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#1e293b',
      titleColor: '#ffffff',
      bodyColor: '#ffffff',
      callbacks: {
        label: (context: any) => `${context.raw} requests`
      }
    }
  },
  scales: {
    x: {
      grid: { display: false },
      ticks: {
        color: '#64748b',
        maxRotation: 40,
        minRotation: 40,
        font: { size: 11 }
      }
    },
    y: {
      beginAtZero: true,
      ticks: { stepSize: 2, color: '#64748b' },
      grid: { color: '#e2e8f0' }
    }
  }
}
</script>

<template>
  <div class="request-card">
    <div class="card-header">
      <h3>Unavailable requests (Weekly)</h3>
    </div>

    <div class="chart-wrapper">
      <div v-if="isLoading" class="state-message">Loading...</div>
      <div v-else-if="error" class="state-message error">{{ error }}</div>
      <div v-else-if="specialtyData.length === 0" class="state-message">No data available.</div>
      <Bar v-else :data="chartData" :options="chartOptions" />
    </div>
  </div>
</template>

<style scoped>
.request-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 12px;
  height: calc(100vh - 270px);
  overflow-y: auto;
}

.card-header {
  margin-bottom: 18px;
}

.card-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 700;
  color: #232f72;
}

.chart-wrapper {
  width: 100%;
  height: 340px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.state-message {
  color: #64748b;
  font-size: 14px;
}

.state-message.error {
  color: #ef4444;
}
</style>