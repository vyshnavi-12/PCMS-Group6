import { createRouter, createWebHistory } from "vue-router";

import Login from "../src/components/Login.vue";
import Profile from "../src/components/Common/Profile.vue";
import Notifications from "../src/components/Common/Notifications.vue";

import DoctorLayout from "../src/layouts/DoctorLayout.vue";
import DoctorDashboard from "../src/components/Doctor/Dashboard.vue";
import DoctorSwapRequests from "../src/components/Doctor/SwapRequests.vue";
import MySchedule from "../src/components/Doctor/MySchedule.vue";

import SupervisorLayout from "../src/layouts/SupervisorLayout.vue";
import SupervisorDashboard from "../src/components/Supervisor/Dashboard.vue";
import SupervisorSwapRequests from "../src/components/Supervisor/SwapRequests.vue";
import SupervisorSchedules from "../src/components/Supervisor/Schedules.vue";
import CoverageSchedule from "../src/components/Supervisor/CoverageSchedule.vue";
import CoverageGap from "../src/components/Supervisor/UnavailableRequests.vue";

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
      meta: {
        requiresAuth: true,
        role: "Physician",
      },

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
          component: Notifications,
        },
        {
          path: "profile",
          component: Profile,
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
      meta: {
        requiresAuth: true,
        role: "Supervisor",
      },

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
          component: Notifications,
        },
        {
          path: "profile",
          component: Profile,
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
          component: CoverageGap,
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

/* =========================
   ROUTE GUARD
========================= */
router.beforeEach((to, from, next) => {
  const user = localStorage.getItem("loggedInUser");

  if (to.meta.requiresAuth && !user) {
    next("/login");
    return;
  }

  if (user) {
    const parsedUser = JSON.parse(user);

    // Prevent logged-in users from opening login page again
    if (to.path === "/login") {
      if (parsedUser.role === "Physician") {
        next("/doctor/dashboard");
        return;
      }

      if (parsedUser.role === "Supervisor") {
        next("/supervisor/dashboard");
        return;
      }
    }

    // Role protection
    if (to.meta.role && parsedUser.role !== to.meta.role) {
      next("/login");
      return;
    }
  }

  next();
});

export default router;