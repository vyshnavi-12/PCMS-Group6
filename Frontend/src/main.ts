import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura'
import { createPinia } from 'pinia'
import VueApexCharts from "vue3-apexcharts"
import ToastService from 'primevue/toastservice'
import Toast from 'primevue/toast'
import OverlayPanel from 'primevue/overlaypanel'

import App from './App.vue'
import router from '../router'
import './style.css'

import 'primeicons/primeicons.css'
import 'primeflex/primeflex.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(VueApexCharts)
app.use(ToastService)
app.component('Toast', Toast)
app.component('OverlayPanel', OverlayPanel)

app.use(PrimeVue, {
  theme: {
    preset: Aura
  }
})

app.mount('#app')