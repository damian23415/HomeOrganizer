<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Logowanie</h1>
      <p class="subtitle">Zaloguj się aby zarządzać czasem pracy</p>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="email">Email:</label>
          <input
            id="email"
            v-model="credentials.email"
            type="email"
            placeholder="twoj@email.com"
            required
            autocomplete="email"
          />
        </div>

        <div class="form-group">
          <label for="password">Hasło:</label>
          <input
            id="password"
            v-model="credentials.password"
            type="password"
            placeholder="Wprowadź hasło"
            required
            autocomplete="current-password"
          />
        </div>

        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <button type="submit" class="login-btn" :disabled="loading">
          <span v-if="loading">Logowanie...</span>
          <span v-else>🔐 Zaloguj się</span>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { authApi } from '@/services/api'

const router = useRouter()
const authStore = useAuthStore()

const credentials = ref({
  email: '',      // ⬅️ ZMIENIONE z username
  password: ''
})

const loading = ref(false)
const errorMessage = ref('')

const handleLogin = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    // Wywołaj API logowania
    const response = await authApi.login(credentials.value)
    
    console.log('✅ Odpowiedź logowania:', response)
    
    // Backend zwraca: { token: string, user: UserDto }
    // Sprawdź czy backend używa PascalCase czy camelCase
    const token = response.token || response.Token
    const user = response.user || response.User
    
    if (!token) {
      throw new Error('Brak tokena w odpowiedzi')
    }
    
    // Zapisz token i dane użytkownika
    authStore.setAuth({
      token: token,
      user: user
    })
    
    // Przekieruj do kalendarza
    router.push('/')
    
  } catch (error) {
    console.error('❌ Błąd logowania:', error)
    errorMessage.value = error.message || 'Nieprawidłowy email lub hasło'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  background: white;
  border-radius: 16px;
  padding: 40px;
  width: 100%;
  max-width: 420px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

h1 {
  margin: 0 0 10px 0;
  color: #333;
  text-align: center;
}

.subtitle {
  color: #666;
  text-align: center;
  margin: 0 0 30px 0;
  font-size: 14px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 500;
  color: #444;
}

.form-group input {
  padding: 12px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 16px;
  transition: all 0.2s;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.error-message {
  background: #ffebee;
  color: #c62828;
  padding: 12px;
  border-radius: 8px;
  font-size: 14px;
  text-align: center;
}

.login-btn {
  padding: 14px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.login-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.login-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>