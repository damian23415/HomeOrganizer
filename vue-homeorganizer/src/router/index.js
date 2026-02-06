import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import MainLayout from '@/layouts/MainLayout.vue'
import CalendarView from '../views/CalendarView.vue'
import LoginView from '../views/LoginView.vue'  // ⬅️ MUSISZ MIEĆ!
import RegisterView from '@/views/RegisterView.vue'
import HourlyRatesView from '@/views/HourlyRatesView.vue'
import EmailConfirmationSendView from '@/views/EmailConfirmationSendView.vue'
import EmailConfirmationView from '@/views/EmailConfirmationView.vue'

const routes = [
  {
    path: '/login',           // ⬅️ WAŻNE
    name: 'Login',
    component: LoginView,
    meta: { requiresAuth: false }
  },
  {
    path: '/register',
    name: 'Register',
    component: RegisterView,
    meta: { requiresAuth: false }
  },
  {
    path: '/email-confirmation',
    name: 'EmailConfirmationSend',
    component: EmailConfirmationSendView,
    meta: { requiresAuth: false }
  },
  {
    path: '/confirm-email',
    name: 'EmailConfirmation',
    component: EmailConfirmationView,
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/work-log'
      },
      {
        path: 'work-log',
        name: 'WorkLog',
        component: CalendarView,
        meta: {
          requiresAuth: true,
          section: 'PRACA',
          title: 'Work Log'
        }
      },
      {
        path: 'hourly-rates',
        name: 'HourlyRates',
        component: HourlyRatesView,
        meta: {
          requiresAuth: true,
          section: 'PRACA',
          title: 'Stawki'
        }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

// Guard - sprawdź autoryzację przed każdą trasą
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const requiresAuth = to.meta.requiresAuth

  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/')
  } else if (to.path === '/register' && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router