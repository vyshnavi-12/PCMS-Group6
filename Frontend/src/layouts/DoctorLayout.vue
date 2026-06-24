<script setup lang="ts">
import { computed } from 'vue'
import { RouterView, useRoute } from 'vue-router'



import Sidebar from '../components/Common/DoctorSideBar.vue'
import AppHeader from '../components/Common/AppHeader.vue'

const route = useRoute()

const pageTitle = computed(() => {
    switch (route.path) {
        case '/doctor/dashboard':
            return 'Dashboard'

        case '/doctor/notifications':
            return 'Notifications'

        case '/doctor/schedule':
            return 'My Schedule'

        case '/doctor/swap-requests':
            return 'Swap Requests'

        case '/doctor/profile':
            return 'Profile'

        default:
            return 'Dashboard'
    }
})
</script>

<template>
    <div class="doctor-layout">

        <Toast position="bottom-right" />

        <Sidebar />

        <main class="main-content">

            <AppHeader :pageTitle="pageTitle" />

            <section class="content-area">
                <RouterView />
            </section>

        </main>

    </div>
</template>

<style scoped>
.doctor-layout {
    display: flex;
    min-height: 100vh;
    background: #f8fafc;
}

.main-content {
    flex: 1;
    display: flex;
    flex-direction: column;
}

.content-area {
    flex: 1;
    padding: 20px 20px 18px 20px;
    overflow: hidden;
}
</style>