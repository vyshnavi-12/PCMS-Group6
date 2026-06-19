<script setup lang="ts">
const loggedInUser = JSON.parse(
    localStorage.getItem('loggedInUser') || '{}'
)
</script>

<template>
    <div class="profile-page">

        <div class="profile-card">

            <!-- Left Section -->
            <div class="profile-left">

                <img :src="loggedInUser.profileImage" alt="Profile" class="profile-avatar" />

                <h2>{{ loggedInUser.name }}</h2>

                <p class="department">
                    {{ loggedInUser.department }}
                </p>

            </div>

            <!-- Right Section -->
            <div class="profile-right">

                <h3>Profile Information</h3>

                <div class="info-grid">

                    <div class="info-row">
                        <span class="label">Employee ID</span>
                        <span class="value">
                            {{ loggedInUser.employeeId }}
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
                            {{ loggedInUser.phone }}
                        </span>
                    </div>

                    <div class="info-row">
                        <span class="label">Department</span>
                        <span class="value">
                            {{ loggedInUser.department }}
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
    object-fit: cover;
    margin-bottom: 16px;
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
    margin-bottom: 4px;
}

.role {
    color: #6b7280;
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