<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getMe } from '../../services/authService'


const loggedInUser = ref({
    fullName: '',
    role: '',
    phoneNumber: '',
    employeeCode: '',
    email: '',
    specialtyName: ''
})

const colors = [
    '#BFDBFE', // pastel blue
    '#DDD6FE', // pastel purple
    '#FBCFE8', // pastel pink
    '#BBF7D0', // pastel green
    '#FED7AA', // pastel orange
    '#A5F3FC', // pastel cyan
    '#FDE68A', // pastel yellow
]

const userInitials = computed(() => {
    if (!loggedInUser.value.fullName) return ''

    const names = loggedInUser.value.fullName.trim().split(' ')

    const firstInitial = names[0]?.charAt(0) || ''
    const lastInitial =
        names.length > 1
            ? names[names.length - 1]?.charAt(0)
            : ''

    return (firstInitial + lastInitial).toUpperCase()
})

const avatarColor = computed(() => {
    if (!loggedInUser.value.fullName) return '#2563eb'

    const index =
        loggedInUser.value.fullName.length % colors.length

    return colors[index]
})

const loading = ref(true)

const fetchProfile = async () => {
    try {
        const response = await getMe()
        loggedInUser.value = response.data
    } catch (error) {
        console.error("Failed to fetch profile:", error)
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    fetchProfile()
})
</script>

<template>
    <div class="profile-page">

        <div v-if="loading" class="loading-text">
            Loading profile...
        </div>

        <div v-else class="profile-card">

            <!-- Left Section -->
            <div class="profile-left">

                <div class="profile-avatar" :style="{ backgroundColor: avatarColor }">
                    {{ userInitials }}
                </div>

                <h2>{{ loggedInUser.fullName }}</h2>

                <p class="department">
                    {{ loggedInUser.specialtyName || loggedInUser.role }}
                </p>

            </div>

            <!-- Right Section -->
            <div class="profile-right">

                <h3>Profile Information</h3>

                <div class="info-grid">

                    <div class="info-row">
                        <span class="label">Employee ID</span>
                        <span class="value">
                            {{ loggedInUser.employeeCode }}
                        </span>
                    </div>

                    <div class="info-row">
                        <span class="label">Email</span>
                        <span class="value">
                            {{ loggedInUser.email }}
                        </span>
                    </div>

                    <div class="info-row">
                        <span class="label">Phone</span>
                        <span class="value">
                            {{ loggedInUser.phoneNumber }}
                        </span>
                    </div>

                    <div class="info-row">
                        <span class="label">Department</span>
                        <span class="value">
                            {{ loggedInUser.specialtyName || loggedInUser.role }}
                        </span>
                    </div>

                </div>

            </div>

        </div>

    </div>
</template>

<style scoped>
.profile-page {
    width: 100%;
}

.loading-text {
    text-align: center;
    padding: 50px;
    font-size: 18px;
}

.profile-card {
    background: white;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    padding: 24px;
    display: flex;
    gap: 32px;
}

.profile-left {
    width: 250px;
    text-align: center;
    border-right: 1px solid #e5e7eb;
    padding-right: 24px;
}

.profile-avatar {
    width: 130px;
    height: 130px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto 16px;

    color: #1e293b;
    font-size: 42px;
    font-weight: 700;
    letter-spacing: 2px;

    border: 2px solid rgba(255,255,255,0.8);
    box-shadow: 0 4px 14px rgba(0,0,0,0.08);
}

.profile-left h2 {
    font-size: 20px;
    font-weight: 600;
    color: #1f2937;
    margin-bottom: 8px;
}

.department {
    color: #2563eb;
    font-weight: 500;
}

.profile-right {
    flex: 1;
}

.profile-right h3 {
    margin-bottom: 24px;
    color: #1f2937;
    font-size: 18px;
    font-weight: 600;
}

.info-grid {
    border: 1px solid #e5e7eb;
    border-radius: 10px;
    overflow: hidden;
}

.info-row {
    display: flex;
    padding: 16px 20px;
    border-bottom: 1px solid #e5e7eb;
}

.info-row:last-child {
    border-bottom: none;
}

.label {
    width: 180px;
    color: #6b7280;
    font-weight: 500;
}

.value {
    color: #111827;
    font-weight: 500;
}

@media (max-width: 768px) {
    .profile-card {
        flex-direction: column;
    }

    .profile-left {
        width: 100%;
        border-right: none;
        border-bottom: 1px solid #e5e7eb;
        padding-right: 0;
        padding-bottom: 24px;
    }

    .info-row {
        flex-direction: column;
        gap: 6px;
    }

    .label {
        width: auto;
    }
}
</style>