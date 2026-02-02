import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(localStorage.getItem('auth_token') || null)
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))

  // Getters
  const isAuthenticated = computed(() => !!token.value)

  // Actions
  const setAuth = (authData) => {
    token.value = authData.token
    user.value = authData.user || null
    
    // Zapisz do localStorage
    localStorage.setItem('auth_token', authData.token)
    if (authData.user) {
      localStorage.setItem('user', JSON.stringify(authData.user))
    }
    
    console.log('✅ Zalogowano:', user.value)
  }

  const clearAuth = () => {
    token.value = null
    user.value = null
    
    localStorage.removeItem('auth_token')
    localStorage.removeItem('user')
    
    console.log('🚪 Wylogowano')
  }

  const logout = () => {
    clearAuth()
  }

  return {
    // State
    token,
    user,
    
    // Getters
    isAuthenticated,
    
    // Actions
    setAuth,
    clearAuth,
    logout
  }
})