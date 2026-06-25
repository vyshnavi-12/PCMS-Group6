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

const approveRequest = async() => {
  if (!selectedReplacement.value) return
  await API.patch(`coverageassignments/alerts/${props.request.alertId}/update-physician/${selectedDoctor.value?.physicianId}`)
  alert('Request Approved')
}

const declineRequest = async() => {
  await API.patch(`coverageassignments/alerts/${props.request.alertId}/decline-request`)
  alert('Request Declined')
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
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 22px;
  box-shadow: 0 12px 30px rgba(15, 23, 42, 0.06);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
}

h2 {
  margin: 0;
  color: #232f72;
  font-size: 22px;
  font-weight: 800;
}

.card-subtitle {
  margin: 6px 0 0;
  color: #64748b;
  font-size: 13px;
}

.close-btn {
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 50%;
  background: #f1f5f9;
  color: #64748b;
  cursor: pointer;
  font-size: 16px;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: 0.2s ease;
  flex-shrink: 0;
}

.close-btn:hover {
  background: #e2e8f0;
  color: #334155;
  transform: rotate(90deg);
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.detail-box label,
.reason-section label,
.replacement-section label {
  display: block;
  font-size: 13px;
  color: #64748b;
  margin-bottom: 8px;
  font-weight: 700;
}

.detail-box p {
  margin: 0;
  background: #f8fafc;
  padding: 13px 14px;
  border-radius: 10px;
  color: #334155;
  font-weight: 600;
  border: 1px solid #eef2f7;
}

.reason-box {
  background: #f8fafc;
  padding: 14px;
  border-radius: 12px;
  color: #334155;
  border: 1px solid #eef2f7;
  line-height: 1.5;
  min-height: 48px;
}

.error-box {
  padding: 12px 14px;
  border-radius: 10px;
  background: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
  font-size: 14px;
  font-weight: 600;
}

.custom-dropdown {
  position: relative;
  width: 100%;
  user-select: none;
}

.dropdown-selected {
  width: 100%;
  min-height: 52px;
  padding: 10px 14px;
  border: 1px solid #dbe2ea;
  border-radius: 12px;
  font-size: 14px;
  background: #ffffff;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: #334155;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: left;
}

.dropdown-selected:hover {
  border-color: #94a3b8;
  box-shadow: 0 6px 14px rgba(15, 23, 42, 0.06);
}

.dropdown-selected.open {
  border-color: #232f72;
  box-shadow: 0 0 0 4px rgba(35, 47, 114, 0.12);
}

.dropdown-selected.disabled {
  cursor: not-allowed;
  background: #f8fafc;
  color: #94a3b8;
}

.selected-content {
  display: flex;
  align-items: center;
  gap: 10px;
  min-width: 0;
}

.doctor-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #232f72, #4457c5);
  color: #ffffff;
  font-size: 13px;
  font-weight: 800;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.selected-text-wrap {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
}

.selected-name {
  font-weight: 700;
  color: #334155;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.placeholder-text {
  color: #94a3b8;
  font-weight: 600;
}

.recommended-mini {
  color: #16a34a;
  font-size: 12px;
  font-weight: 700;
}

.dropdown-arrow {
  font-size: 11px;
  color: #64748b;
  transition: transform 0.2s ease;
  margin-left: 10px;
}

.dropdown-arrow.rotate {
  transform: rotate(180deg);
}

.dropdown-options {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  right: 0;
  z-index: 50;
  margin: 0;
  padding: 8px;
  list-style: none;
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.18);
  max-height: 260px;
  overflow-y: auto;
}

.dropdown-option {
  padding: 10px;
  border-radius: 11px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  transition: all 0.18s ease;
}

.dropdown-option:hover {
  background: #f8fafc;
}

.dropdown-option.active {
  background: #eef2ff;
}

.dropdown-option.recommended {
  border: 1px solid #dcfce7;
  background: #f0fdf4;
}

.dropdown-option.recommended:hover {
  background: #dcfce7;
}

.option-left {
  display: flex;
  align-items: center;
  gap: 10px;
  min-width: 0;
}

.option-avatar {
  width: 34px;
  height: 34px;
}

.doctor-info {
  display: flex;
  flex-direction: column;
  gap: 3px;
  min-width: 0;
}

.doctor-name {
  font-size: 14px;
  font-weight: 700;
  color: #334155;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.recommended-badge {
  width: fit-content;
  padding: 3px 8px;
  border-radius: 999px;
  background: #dcfce7;
  color: #15803d;
  font-size: 11px;
  font-weight: 800;
}

.check-icon {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: #232f72;
  color: #ffffff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 900;
  flex-shrink: 0;
}

.dropdown-options::-webkit-scrollbar {
  width: 7px;
}

.dropdown-options::-webkit-scrollbar-track {
  background: transparent;
}

.dropdown-options::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 999px;
}

.dropdown-options::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

.dropdown-fade-enter-active,
.dropdown-fade-leave-active {
  transition: all 0.16s ease;
}

.dropdown-fade-enter-from,
.dropdown-fade-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.decline-btn,
.approve-btn {
  border-radius: 10px;
  padding: 11px 20px;
  cursor: pointer;
  font-weight: 800;
  transition: all 0.2s ease;
}

.decline-btn {
  background: #ffffff;
  color: #dc2626;
  border: 1px solid #fecaca;
}

.decline-btn:hover {
  background: #fef2f2;
}

.approve-btn {
  background: #232f72;
  color: #ffffff;
  border: none;
}

.approve-btn:hover:not(:disabled) {
  background: #1c265f;
  transform: translateY(-1px);
  box-shadow: 0 8px 18px rgba(35, 47, 114, 0.25);
}

.approve-btn:disabled {
  background: #94a3b8;
  cursor: not-allowed;
  box-shadow: none;
}

@media (max-width: 640px) {
  .request-card {
    padding: 18px;
  }

  .details-grid {
    grid-template-columns: 1fr;
  }

  .actions {
    flex-direction: column-reverse;
  }

  .decline-btn,
  .approve-btn {
    width: 100%;
  }
}
</style>