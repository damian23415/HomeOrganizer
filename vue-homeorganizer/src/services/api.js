import { useAuthStore } from '@/stores/auth'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

// Helper do obsługi odpowiedzi
const handleResponse = async (response) => {
  // 401 = Unauthorized - token wygasł lub nieprawidłowy
  if (response.status === 401) {
    const authStore = useAuthStore()
    authStore.clearAuth()
    window.location.href = '/login'
    throw new Error('Sesja wygasła. Zaloguj się ponownie.')
  }

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ 
      message: 'Network error',
      errors: []
    }))
    
    const errorMessage = errorData.errors?.length > 0 
      ? errorData.errors.join(', ') 
      : errorData.message || `HTTP error! status: ${response.status}`
    
    throw new Error(errorMessage)
  }
  
  return response.json()
}

// Helper do tworzenia requestów
const request = async (endpoint, options = {}) => {
  const url = `${API_BASE_URL}${endpoint}`
  const authStore = useAuthStore()
  
  const config = {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  }

  // Dodaj token do headera (jeśli istnieje)
  if (authStore.token) {
    config.headers['Authorization'] = `Bearer ${authStore.token}`
  }

  console.log(`🌐 API Request: ${config.method || 'GET'} ${url}`)
  if (config.body) {
    console.log('📤 Request Body:', JSON.parse(config.body))
  }
  
  try {
    const response = await fetch(url, config)
    const data = await handleResponse(response)
    console.log('✅ API Response:', data)
    return data
  } catch (error) {
    console.error('❌ API Error:', error)
    throw error
  }
}

// ==========================================
// AUTH API
// ==========================================

export const authApi = {
  /**
   * Zaloguj użytkownika
   * @param {Object} credentials - { email, password }
   * @returns {Promise<{token: string, user: Object}>}
   */
  login: async (credentials) => {
    return request('/users/login', {  // ⬅️ ZMIENIONE
      method: 'POST',
      body: JSON.stringify(credentials),
    })
  },

  register: async (credentials) => {
    return request('/users/register', {
      method: 'POST',
      body: JSON.stringify(credentials)
    })
  }
}

// ==========================================
// WORK TIME API
// ==========================================

export const workTimeApi = {
  /**
   * Pobierz dane pracy dla konkretnego miesiąca
   */
  getMonthData: async (year, month) => {
    return request(`/worktracking/workDays/${year}/${month}`)
  },

  /**
   * Zapisz czas pracy
   */
  save: async (data) => {
    return request('/worktracking/workDay', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },
}

export const hourlyRateApi = {
  // pobierz dane o aktualnej stawce
  getHourlyRate: async () => {
    const today = new Date().toISOString().split('T')[0]
    
    return request(`/worktracking/hourlyRate/${today}`)
  },

  save: async(data) => {
    return request('/worktracking/hourlyRate', {
      method: 'POST',
      body: JSON.stringify(data)
    })
  }
}

export default {
  auth: authApi,
  workTime: workTimeApi,
  hourlyRate: hourlyRateApi
}