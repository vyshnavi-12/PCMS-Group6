<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Checkbox from 'primevue/checkbox'
import Button from 'primevue/button'

const users = [
  {
    email: 'doctor@pcms.com',
    password: 'Doctor123',
    role: 'Doctor',
    name: 'Dr. John Smith',
    department: 'Cardiology',
    employeeId: 'PHY-0012',
    phone: '(555) 123-4567',
    profileImage: 'https://i.pravatar.cc/200?img=12'
  },
  {
    email: 'supervisor@pcms.com',
    password: 'Supervisor123',
    role: 'Supervisor',
    name: 'John Williams',
    department: 'Supervisor',
    employeeId: 'SUP-0001',
    phone: '(555) 987-6543',
    profileImage: 'https://i.pravatar.cc/200?img=15'
  }
]

const router = useRouter()

const email = ref('')
const password = ref('')
const rememberMe = ref(false)

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

const handleLogin = () => {
  submitted.value = true
  loginError.value = ''

  if (!formValid.value) {
    return
  }

  const user = users.find(
    u =>
      u.email.toLowerCase() === email.value.toLowerCase().trim() &&
      u.password === password.value
  )

  if (!user) {
    loginError.value = 'Invalid Email or Password'
    return
  }

  // Store logged in user
  localStorage.setItem(
    'loggedInUser',
    JSON.stringify(user)
  )

  // Store remember me preference
  localStorage.setItem(
    'rememberMe',
    rememberMe.value.toString()
  )

  // Redirect based on role
  if (user.role === 'Doctor') {
    router.push('/doctor/dashboard')
  }
  else {
    router.push('/supervisor/dashboard')
  }
}
</script>

<template>
  <div class="login-page">
    <Card class="login-card">
      <template #content>
        <div class="header">
          <h1>PCMS Portal</h1>
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

        <div class="remember-row">
          <Checkbox v-model="rememberMe" :binary="true" />

          <label>Remember me</label>
        </div>

        <Button label="Log In" class="login-btn" @click="handleLogin" />

        <small v-if="loginError" class="error-text login-error">
          {{ loginError }}
        </small>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.login-page {
  height: 100vh;
  background: #f8fafc;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 0;
}

.login-card {
  width: 100%;
  max-width: 500px;
}

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

.remember-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 22px;
}

.remember-row label {
  margin: 0;
  font-size: 14px;
  color: #475569;
}

.login-btn {
  width: 100%;
}

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

/* Card */
:deep(.p-card) {
  border-radius: 18px;
  border: 1px solid #e2e8f0;
  box-shadow:
    0 10px 25px rgba(35, 47, 114, 0.08),
    0 4px 10px rgba(35, 47, 114, 0.05);
}

/* Inputs */
:deep(.p-inputtext),
:deep(.p-password-input) {
  width: 100%;
}

:deep(.p-inputtext:enabled:focus),
:deep(.p-password-input:enabled:focus) {
  border-color: #232f72;
  box-shadow: 0 0 0 2px rgba(35, 47, 114, 0.15);
}

/* Checkbox */
:deep(.p-checkbox.p-highlight .p-checkbox-box) {
  background: #232f72;
  border-color: #232f72;
}

/* Button */
:deep(.login-btn) {
  background: #232f72;
  border: none;
}

:deep(.login-btn:hover) {
  background: #1b255c;
}

/* Password Eye */
:deep(.p-password .p-password-toggle-mask-icon) {
  color: #232f72;
}

@media (max-width: 768px) {
  .login-card {
    max-width: 100%;
  }

  .header h1 {
    font-size: 28px;
  }
}
</style>