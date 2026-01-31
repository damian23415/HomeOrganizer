import { createRouter, createWebHistory } from 'vue-router'
import CalendarView from '../views/CalendarView.vue'

const routes = [
  {
    path: '/',
    name: 'Calendar',
    component: CalendarView
  }
  // Tutaj możesz dodać więcej tras w przyszłości:
  // {
  //   path: '/settings',
  //   name: 'Settings',
  //   component: () => import('../views/SettingsView.vue')
  // }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

export default router