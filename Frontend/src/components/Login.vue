<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'

import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'

import { loginUser } from '../services/authService'

const router = useRouter()
const toast = useToast()

const email = ref('')
const password = ref('')

const submitted = ref(false)
const loginError = ref('')

const emailValid = computed(() =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)
)

const isPasswordValid = computed(() =>
  password.value.trim().length >= 8
)

const formValid = computed(() =>
  emailValid.value &&
  isPasswordValid.value
)

const handleLogin = async () => {
  submitted.value = true
  loginError.value = ''

  if (!formValid.value) return

  try {
    const response = await loginUser(
      email.value.trim(),
      password.value
    )

    const user = response.data

    toast.add({
      severity: 'success',
      summary: 'Login Successful',
      detail: `Welcome ${user.fullName || ''}`,
      life: 3000
    })

    localStorage.setItem(
      "loggedInUser",
      JSON.stringify(user)
    )

    if (user.role === "Physician") {
      router.push("/doctor/dashboard")
    } else {
      router.push("/supervisor/dashboard")
    }
  } catch (error: any) {
    loginError.value =
      error?.response?.data?.message || "Login failed"

    toast.add({
      severity: 'error',
      summary: 'Login Failed',
      detail: loginError.value,
      life: 3000
    })
  }
}
</script>

<template>
  <div class="login-page">

    <!-- LEFT PANEL -->
    <div class="left-panel">
      <div class="branding-content">

        <div class="brand-logo">
          <i class="pi pi-shield"></i>
          <h1>Care On-Call</h1>
        </div>



        <div class="mission-card">
          <p>
            Ensuring continuous physician coverage across every specialty,
            every shift, every day.
          </p>
        </div>

        <div class="stats-row">
          <div class="mini-card">
            <h3>24/7</h3>
            <span>Coverage Support</span>
          </div>

          <div class="mini-card">
            <h3>100%</h3>
            <span>Schedule Visibility</span>
          </div>
        </div>

      </div>
    </div>

    <!-- RIGHT PANEL -->
    <div class="right-panel">
      <Card class="login-card">
        <template #content>
          <div class="header">
            <h1>COC Portal</h1>
            <p>Coverage Scheduling & Management</p>
          </div>

          <div class="field">
            <label>Email Address</label>
            <InputText v-model="email" placeholder="Enter your email" class="full-width" />

            <small v-if="submitted && !emailValid" class="error-text">
              Please enter a valid email address
            </small>
          </div>

          <div class="field">
            <div class="password-header">
              <label>Password</label>

              <RouterLink to="/forgot-password" class="forgot-link">
                Forgot password?
              </RouterLink>
            </div>

            <Password v-model="password" :feedback="false" toggleMask placeholder="Enter your password" fluid />

            <small v-if="submitted && password.length === 0" class="error-text">
              Password is required
            </small>

            <small v-else-if="submitted && password.length < 8" class="error-text">
              Password must be at least 8 characters
            </small>
          </div>

          <Button label="Log In" class="login-btn" @click="handleLogin" />

          <small v-if="loginError" class="error-text login-error">
            {{ loginError }}
          </small>
        </template>
      </Card>
    </div>

  </div>
</template>

<style scoped>
.login-page {
  height: 100vh;
  display: flex;
  background: #f8fafc;
}

/* LEFT PANEL */
.left-panel {
  width: 50%;
  background: linear-gradient(135deg, #232f72 0%, #16204d 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 60px;
}

.branding-content {
  max-width: 500px;
}

.brand-logo {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 28px;
}

.brand-logo i {
  width: 72px;
  height: 72px;
  border-radius: 18px;
  background: rgba(255, 255, 255, 0.12);

  display: flex;
  align-items: center;
  justify-content: center;

  font-size: 32px;
}

.brand-logo h1 {
  margin: 0;
  font-size: 52px;
  font-weight: 700;
  color: white;
}

.branding-content h2 {
  margin-top: 14px;
  font-size: 26px;
  font-weight: 600;
  line-height: 1.3;
}

.branding-content p {
  margin-top: 18px;
  font-size: 16px;
  line-height: 1.7;
  color: rgba(255, 255, 255, 0.85);
}

.mission-card {
  margin-top: 40px;
  padding: 28px;
  border-radius: 20px;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.mission-card p {
  margin: 0;
  font-size: 20px;
  line-height: 1.7;
  color: rgba(255, 255, 255, 0.95);
  font-weight: 500;
}

/* MINI CARDS */
.stats-row {
  display: flex;
  gap: 16px;
  margin-top: 24px;
}

.mini-card {
  flex: 1;
  padding: 22px;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.08);
  text-align: center;
}

.mini-card h3 {
  margin: 0;
  font-size: 28px;
  color: #f69d39;
}

.mini-card span {
  display: block;
  margin-top: 8px;
  font-size: 13px;
  color: rgba(255, 255, 255, 0.75);
}

/* RIGHT PANEL */
.right-panel {
  width: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 40px;
}

.login-card {
  width: 100%;
  max-width: 500px;
}

/* FORM HEADER */
.header {
  text-align: center;
  margin-bottom: 28px;
}

.header h1 {
  margin: 0;
  font-size: 32px;
  font-weight: 700;
  color: #232f72;
}

.header p {
  margin-top: 8px;
  color: #64748b;
  font-size: 14px;
}

/* FORM FIELDS */
.field {
  margin-bottom: 18px;
}

.field label {
  display: block;
  margin-bottom: 6px;
  font-size: 14px;
  font-weight: 600;
  color: #232f72;
}

.password-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.forgot-link {
  font-size: 13px;
  color: #f69d39;
  text-decoration: none;
  font-weight: 600;
}

.forgot-link:hover {
  color: #e68b22;
}

.full-width {
  width: 100%;
}

/* LOGIN BUTTON */
.login-btn {
  width: 100%;
}

/* ERROR */
.error-text {
  display: block;
  margin-top: 4px;
  color: #dc2626;
  font-size: 12px;
}

.login-error {
  display: block;
  text-align: center;
  margin-top: 12px;
  font-size: 13px;
}

/* PRIMEVUE CARD */
:deep(.p-card) {
  border-radius: 18px;
  border: 1px solid #e2e8f0;
  box-shadow:
    0 10px 25px rgba(35, 47, 114, 0.08),
    0 4px 10px rgba(35, 47, 114, 0.05);
}

/* INPUTS */
:deep(.p-inputtext),
:deep(.p-password-input) {
  width: 100%;
}

:deep(.p-inputtext:enabled:focus),
:deep(.p-password-input:enabled:focus) {
  border-color: #232f72;
  box-shadow: 0 0 0 2px rgba(35, 47, 114, 0.15);
}

/* CHECKBOX */
:deep(.p-checkbox.p-highlight .p-checkbox-box) {
  background: #232f72;
  border-color: #232f72;
}

/* BUTTON */
:deep(.login-btn) {
  background: #232f72;
  border: none;
}

:deep(.login-btn:hover) {
  background: #1b255c;
}

/* PASSWORD EYE */
:deep(.p-password .p-password-toggle-mask-icon) {
  color: #232f72;
}

/* RESPONSIVE */
@media (max-width: 1024px) {
  .left-panel {
    display: none;
  }

  .right-panel {
    width: 100%;
  }
}
</style>