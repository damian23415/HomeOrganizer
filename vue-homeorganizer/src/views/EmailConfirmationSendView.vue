<template>
  <div class="confirmation-container">
    <div class="confirmation-card">
      <div class="icon-wrapper">
        <span class="icon">📧</span>
      </div>
      
      <h1>Sprawdź swoją skrzynkę!</h1>
      
      <p class="main-message">
        Wysłaliśmy link aktywacyjny na adres:
      </p>
      
      <p class="email">{{ email }}</p>
      
      <div class="info-box">
        <p>✅ Kliknij link w emailu, aby aktywować konto</p>
        <p>⏱️ Link jest ważny przez 24 godziny</p>
        <p>📬 Sprawdź folder SPAM, jeśli nie widzisz wiadomości</p>
      </div>
      
      <div class="actions">
        
        <button @click="goToLogin" class="btn-login">
          🔑 Przejdź do logowania
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const email = ref(route.query.email || 'twoj@email.com')

const goToLogin = () => {
  router.push('/login')
}
</script>

<style scoped>
.confirmation-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.confirmation-card {
  background: white;
  border-radius: 16px;
  padding: 40px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  text-align: center;
}

.icon-wrapper {
  margin-bottom: 20px;
}

.icon {
  font-size: 80px;
  display: inline-block;
  animation: bounce 2s infinite;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-20px); }
}

h1 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 28px;
}

.main-message {
  color: #666;
  font-size: 16px;
  margin-bottom: 10px;
}

.email {
  color: #667eea;
  font-weight: 600;
  font-size: 18px;
  margin-bottom: 30px;
}

.info-box {
  background: #f8f9fa;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 30px;
  text-align: left;
}

.info-box p {
  margin: 10px 0;
  color: #555;
  font-size: 14px;
  line-height: 1.6;
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.btn-resend,
.btn-login {
  padding: 14px;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-resend {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-resend:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-resend:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-login {
  background: white;
  color: #667eea;
  border: 2px solid #667eea;
}

.btn-login:hover {
  background: #f8f9fa;
  transform: translateY(-2px);
}
</style>