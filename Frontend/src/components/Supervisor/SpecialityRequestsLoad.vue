<script setup lang="ts">
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Tooltip,
  Legend
} from 'chart.js'
import { Bar } from 'vue-chartjs'

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Tooltip,
  Legend
)

const chartData = {
  labels: [
    'Emergency',
    'Cardiology',
    'Orthopedics',
    'Neurology',
    'Radiology',
    'Anesthesiology',
    'Surgery'
  ],
  datasets: [
    {
      label: 'Requests',
      data: [12, 8, 7, 6, 4, 3, 2],
      backgroundColor: '#f89341',
      hoverBackgroundColor: '#ea580c',
      borderRadius: 8,
      borderSkipped: false,
      barThickness: 42
    }
  ]
}

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,

  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      backgroundColor: '#1e293b',
      titleColor: '#ffffff',
      bodyColor: '#ffffff',
      callbacks: {
        label: function (context: any) {
          return `${context.raw} requests`
        }
      }
    }
  },

  scales: {
    x: {
      grid: {
        display: false
      },
      ticks: {
        color: '#64748b',
        maxRotation: 40,
        minRotation: 40,
        font: {
          size: 11
        }
      }
    },

    y: {
      beginAtZero: true,
      ticks: {
        stepSize: 2,
        color: '#64748b'
      },
      grid: {
        color: '#e2e8f0'
      }
    }
  }
}
</script>

<template>
  <div class="request-card">
    <div class="card-header">
      <h3>Specialty Request Load (Weekly)</h3>
    </div>

    <div class="chart-wrapper">
      <Bar
        :data="chartData"
        :options="chartOptions"
      />
    </div>
  </div>
</template>

<style scoped>
.request-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 12px;
  height: 100%;
}

.card-header {
  margin-bottom: 18px;
}

.card-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
}

.chart-wrapper {
  width: 100%;
  height: 340px;
}
</style>