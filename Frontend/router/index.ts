import { createRouter, createWebHistory } from "vue-router";

import Login from "../src/components/Login.vue";

import DoctorLayout from "../src/layouts/DoctorLayout.vue";
import DoctorDashboard from "../src/components/Doctor/Dashboard.vue";
import DoctorNotifications from "../src/components/Common/Notifications.vue";
import DoctorProfile from "../src/components/Common/Profile.vue";
import DoctorSwapRequests from "../src/components/Doctor/SwapRequests.vue";
import MySchedule from "../src/components/Doctor/MySchedule.vue";

import SupervisorLayout from "../src/layouts/SupervisorLayout.vue";
import SupervisorDashboard from "../src/components/Supervisor/Dashboard.vue";
import SupervisorNotifications from "../src/components/Supervisor/Notifications.vue";
import SupervisorProfile from "../src/components/Supervisor/Profile.vue";
import SupervisorSwapRequests from "../src/components/Supervisor/SwapRequests.vue";
import SupervisorSchedules from "../src/components/Supervisor/Schedules.vue";
import CoverageSchedule from "../src/components/Supervisor/CoverageSchedule.vue";
import CoverageGap from "../src/components/Supervisor/CoverageGap.vue";

const router = createRouter({
  history: createWebHistory(),

  routes: [
    {
      path: "/",
      redirect: "/login",
    },

    {
      path: "/login",
      component: Login,
    },

    /* =========================
       DOCTOR ROUTES
    ========================= */

    {
      path: "/doctor",
      component: DoctorLayout,

      children: [
        {
          path: "",
          redirect: "/doctor/dashboard",
        },

        {
          path: "dashboard",
          component: DoctorDashboard,
        },

        {
          path: "notifications",
          component: DoctorNotifications,
        },

        {
          path: "profile",
          component: DoctorProfile,
        },

        {
          path: "swap-requests",
          component: DoctorSwapRequests,
        },

        {
          path: "schedule",
          component: MySchedule,
        },
      ],
    },

    /* =========================
       SUPERVISOR ROUTES
    ========================= */

    {
      path: "/supervisor",
      component: SupervisorLayout,

      children: [
        {
          path: "",
          redirect: "/supervisor/dashboard",
        },

        {
          path: "dashboard",
          component: SupervisorDashboard,
        },

        {
          path: "notifications",
          component: SupervisorNotifications,
        },

        {
          path: "profile",
          component: SupervisorProfile,
        },

        {
          path: "swap-requests",
          component: SupervisorSwapRequests,
        },

        {
          path: "schedules",
          component: SupervisorSchedules,
        },

        {
          path: "coverage-schedule",
          component: CoverageSchedule,
        },

        {
          path: "coverage-gaps",
          component: CoverageGap
        },
      ],
    },

    /* =========================
       FALLBACK
    ========================= */

    {
      path: "/:pathMatch(.*)*",
      redirect: "/login",
    },
  ],
});

export default router;
