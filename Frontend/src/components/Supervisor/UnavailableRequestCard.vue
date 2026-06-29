<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import API from '../../api/axios'

type ReplacementDoctor = {
  physicianId: number
  physicianName: string
  isRecommended: boolean
}

type RequestData = {
  alertId: number
  date: string
  specialty: string
  shift: string
  requestedBy: string
}

const props = defineProps<{
  request: RequestData
}>()

const emit = defineEmits<{
  close: []
  updated: [message: string]
}>()

const reason = ref('')
const replacements = ref<ReplacementDoctor[]>([])
const selectedReplacement = ref<number | null>(null)

const isDropdownOpen = ref(false)
const isLoading = ref(false)
const errorMessage = ref('')

const dropdownRef = ref<HTMLElement | null>(null)

const selectedDoctor = computed(() => {
  return replacements.value.find(
    doctor => doctor.physicianId === selectedReplacement.value
  )
})

const selectedDoctorDisplay = computed(() => {
  if (!selectedDoctor.value) return 'Select a replacement doctor'
  return selectedDoctor.value.physicianName
})

const handleSelect = (doctor: ReplacementDoctor) => {
  selectedReplacement.value = doctor.physicianId
  isDropdownOpen.value = false
}

const toggleDropdown = () => {
  if (!replacements.value.length) return
  isDropdownOpen.value = !isDropdownOpen.value
}

const approveRequest = async () => {
  try {
    if (!selectedReplacement.value) return

    await API.patch(
      `coverageassignments/alerts/${props.request.alertId}/update-physician/${selectedDoctor.value?.physicianId}`
    )

    emit('updated', 'Request approved successfully')
  } catch (error) {
    console.error(error)
  }
}

const declineRequest = async () => {
  try {
    await API.patch(
      `coverageassignments/alerts/${props.request.alertId}/decline-request`
    )

    emit('updated', 'Request declined successfully')
  } catch (error) {
    console.error(error)
  }
}

const fetchAlertDetails = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''

    const res = await API.get(
      `coverageassignments/alerts/${props.request.alertId}/details`
    )

    reason.value = res.data.data.reason || ''
    replacements.value = res.data.data.replacements || []

    if (replacements.value.length > 0) {
      const recommendedDoctor = replacements.value.find(
        doctor => doctor.isRecommended
      )

      selectedReplacement.value =
        recommendedDoctor?.physicianId || replacements.value[0].physicianId
    }
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Unable to load replacement details.'
  } finally {
    isLoading.value = false
  }
}

const handleClickOutside = (event: MouseEvent) => {
  if (
    dropdownRef.value &&
    !dropdownRef.value.contains(event.target as Node)
  ) {
    isDropdownOpen.value = false
  }
}

onMounted(() => {
  fetchAlertDetails()
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<template>
  <div class="request-card">
    <div class="card-header">
      <div>
        <h2>Unavailable Request</h2>
        <p class="card-subtitle">Review the request and assign a replacement doctor</p>
      </div>

      <button class="close-btn" @click="emit('close')" aria-label="Close">
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
        <p>{{ request.requestedBy }}</p>
      </div>
    </div>

    <div class="reason-section">
      <label>Reason</label>

      <div class="reason-box">
        <span v-if="isLoading">Loading reason...</span>
        <span v-else>{{ reason || 'No reason provided.' }}</span>
      </div>
    </div>

    <div class="replacement-section">
      <label>Replacement</label>

      <div v-if="errorMessage" class="error-box">
        {{ errorMessage }}
      </div>

      <div v-else ref="dropdownRef" class="custom-dropdown">
        <button
          type="button"
          class="dropdown-selected"
          :class="{ open: isDropdownOpen, disabled: !replacements.length || isLoading }"
          @click.stop="toggleDropdown"
        >
          <span class="selected-content">
            <span v-if="isLoading" class="placeholder-text">
              Loading doctors...
            </span>

            <template v-else-if="selectedDoctor">
              <span class="doctor-avatar">
                {{ selectedDoctor.physicianName.charAt(0).toUpperCase() }}
              </span>

              <span class="selected-text-wrap">
                <span class="selected-name">
                  {{ selectedDoctorDisplay }}
                </span>

                <span
                  v-if="selectedDoctor.isRecommended"
                  class="recommended-mini"
                >
                  Recommended
                </span>
              </span>
            </template>

            <span v-else class="placeholder-text">
              No replacement doctors available
            </span>
          </span>

          <span class="dropdown-arrow" :class="{ rotate: isDropdownOpen }">
            ▼
          </span>
        </button>

        <transition name="dropdown-fade">
          <ul v-if="isDropdownOpen" class="dropdown-options">
            <li
              v-for="doctor in replacements"
              :key="doctor.physicianId"
              class="dropdown-option"
              :class="{
                active: selectedReplacement === doctor.physicianId,
                recommended: doctor.isRecommended
              }"
              @click="handleSelect(doctor)"
            >
              <div class="option-left">
                <span class="doctor-avatar option-avatar">
                  {{ doctor.physicianName.charAt(0).toUpperCase() }}
                </span>

                <div class="doctor-info">
                  <span class="doctor-name">
                    {{ doctor.physicianName }}
                  </span>

                  <span
                    v-if="doctor.isRecommended"
                    class="recommended-badge"
                  >
                    ⭐ Best match
                  </span>
                </div>
              </div>

              <span
                v-if="selectedReplacement === doctor.physicianId"
                class="check-icon"
              >
                ✓
              </span>
            </li>
          </ul>
        </transition>
      </div>
    </div>

    <div class="actions">
      <button class="decline-btn" @click="declineRequest">
        Decline
      </button>

      <button
        class="approve-btn"
        :disabled="!selectedReplacement || isLoading"
        @click="approveRequest"
      >
        Approve
      </button>
    </div>
  </div>
</template>

<style scoped>
.request-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 18px;
  padding: 18px 22px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-shadow: 0 12px 30px rgba(15, 23, 42, 0.06);
  width: 100%;

  /* Dynamic height wrt viewport */
  height: calc(100vh - 140px);
  max-height: calc(100vh - 140px);
  min-height: 400px;

  overflow-y: auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

h2 {
  margin: 0;
  color: #232f72;
  font-size: 20px;
  font-weight: 800;
}

.card-subtitle {
  margin: 4px 0 0;
  color: #64748b;
  font-size: 12px;
}

.close-btn {
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 50%;
  background: #f1f5f9;
  cursor: pointer;
  font-size: 16px;
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 14px;
}

.detail-box label,
.reason-section label,
.replacement-section label {
  display: block;
  font-size: 13px;
  color: #64748b;
  margin-bottom: 6px;
  font-weight: 700;
}

.detail-box p {
  margin: 0;
  background: #f8fafc;
  padding: 12px;
  border-radius: 10px;
  border: 1px solid #eef2f7;
  font-weight: 600;
}

.reason-box {
  background: #f8fafc;
  padding: 14px;
  border-radius: 12px;
  border: 1px solid #eef2f7;
  min-height: 52px;
}

.custom-dropdown {
  position: relative;
  width: 100%;
}

.dropdown-selected {
  width: 100%;
  min-height: 56px;
  padding: 12px 16px;
  border: 1px solid #dbe2ea;
  border-radius: 12px;
  background: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
}

.selected-content {
  display: flex;
  align-items: center;
  gap: 10px;
}

.doctor-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #232f72, #4457c5);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
}

.selected-text-wrap {
  display: flex;
  flex-direction: column;
}

.selected-name {
  font-weight: 700;
}

.recommended-mini {
  font-size: 12px;
  color: green;
}

.dropdown-arrow {
  font-size: 12px;
}

.dropdown-arrow.rotate {
  transform: rotate(180deg);
}

.dropdown-options {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  right: 0;
  z-index: 999;
  margin: 0;
  padding: 8px;
  list-style: none;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.18);
  max-height: 140px;
  overflow-y: auto;
}

.dropdown-option {
  padding: 10px;
  border-radius: 10px;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.dropdown-option:hover {
  background: #f8fafc;
}

.option-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.doctor-info {
  display: flex;
  flex-direction: column;
}

.recommended-badge {
  font-size: 11px;
  color: green;
}

.check-icon {
  color: #232f72;
  font-weight: bold;
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 30px;
}

.decline-btn,
.approve-btn {
  border-radius: 10px;
  padding: 12px 22px;
  cursor: pointer;
  font-weight: 700;
}

.decline-btn {
  background: white;
  color: #dc2626;
  border: 1px solid #fecaca;
}

.approve-btn {
  background: #232f72;
  color: white;
  border: none;
}

@media (max-width: 1100px) {
  .details-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .details-grid {
    grid-template-columns: 1fr;
  }

  .actions {
    flex-direction: column-reverse;
  }

  .approve-btn,
  .decline-btn {
    width: 100%;
  }
}
</style>