<template>
  <div class="confirm-page">
    <div class="card">
      <header class="card__header">
        <h1 class="title">Potwierdzanie adresu e‑mail</h1>
        <p class="subtitle">Trwa sprawdzanie linku — proszę chwilę poczekać.</p>
      </header>

      <section class="card__body">
        <div v-if="status === 'loading'" class="state loading" aria-live="polite">
          <div class="spinner" aria-hidden="true"></div>
          <p class="muted">Proszę czekać — potwierdzanie...</p>
        </div>

        <div v-else-if="status === 'success'" class="state success" aria-live="polite">
          <div class="icon">✅</div>
          <h2>Adres potwierdzony</h2>
          <p>Twoje konto zostało aktywowane. Zaraz przekierujemy Cię do ekranu logowania.</p>
          <div class="actions">
            <button class="btn" @click="goToLogin">Przejdź do logowania</button>
          </div>
        </div>

        <div v-else-if="status === 'expired'" class="state error" aria-live="polite">
          <div class="icon">⚠️</div>
          <h2>Link wygasł</h2>
          <p>Link potwierdzający wygasł. Możesz poprosić o nowy link.</p>

          <form class="resend" @submit.prevent="resend">
            <label class="label">
              Twój e‑mail
              <input v-model="email" type="email" placeholder="adres@domena.pl" required />
            </label>
            <div class="actions">
              <button class="btn outline" :disabled="resending">
                {{ resending ? 'Wysyłanie...' : 'Wyślij ponownie' }}
              </button>
            </div>
            <p class="muted small" v-if="resendMessage">{{ resendMessage }}</p>
          </form>
        </div>

        <div v-else-if="status === 'error'" class="state error" aria-live="polite">
          <div class="icon">⚠️</div>
          <h2>Błąd</h2>
          <p>Wystąpił błąd sieci. Spróbuj ponownie.</p>
          <div class="actions">
            <button class="btn" @click="confirmToken">Spróbuj ponownie</button>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const status = ref(null) // null | 'success' | 'expired' | 'invalid' | 'error'
const email = ref('')
const resending = ref(false)
const resendMessage = ref('')

const token = route.query.token || ''
const expiryDate = route.query.expiry || ''

const goToLogin = () => router.push({ name: 'login' })

const confirmToken = async () => {
    if (!token) {
        status.value = null;
        return
    }
    status.value = 'loading'
    loading.value = true
    try {
         const response = await api.auth.confirmToken(token, expiryDate.replace(/'/g, ''));
        
         if (response) {
            status.value = 'success'
           router.push({
             name: "Login"
           })
         }
       
    } catch (e) {
        status.value = 'error'
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    if (token) confirmToken()
})
</script>

<style scoped>
:root {
  --card-bg: #ffffff;
  --page-bg: #f4f7fb;
  --muted: #6b7280;
  --primary-start: #667eea;
  --primary-end: #764ba2;
  --btn-text: #ffffff;
  --danger: #ef4444;
  --success: #10b981;
}

.confirm-page {
  min-height: 70vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px 16px;
  background: var(--page-bg);
}

/* card */
.card {
  width: 100%;
  max-width: 680px;
  background: var(--card-bg);
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(16,24,40,0.06);
  overflow: hidden;
  border: 1px solid rgba(16,24,40,0.04);
}

/* header */
.card__header {
  padding: 28px 28px 8px 28px;
  background: linear-gradient(90deg, rgba(102,126,234,0.06), rgba(118,75,162,0.03));
}
.title {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #0f172a;
}
.subtitle {
  margin: 6px 0 0;
  color: var(--muted);
  font-size: 13px;
}

/* body */
.card__body {
  padding: 24px 28px 28px 28px;
}

/* common states */
.state {
  text-align: center;
  padding: 8px 12px;
}
.state .icon {
  font-size: 34px;
  margin-bottom: 8px;
}
.state h2 {
  margin: 6px 0 8px;
  font-size: 18px;
}
.state p { color: var(--muted); margin: 0 0 12px; }

/* spinner */
.spinner {
  width: 48px;
  height: 48px;
  margin: 8px auto 12px;
  border-radius: 50%;
  border: 4px solid rgba(99,102,241,0.15);
  border-top-color: rgba(118,75,162,1);
  animation: spin 0.9s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* actions & buttons */
.actions {
  margin-top: 12px;
  display: flex;
  justify-content: center;
  gap: 12px;
}

.btn {
  padding: 10px 18px;
  border-radius: 10px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  color: var(--btn-text);
  background: linear-gradient(90deg, var(--primary-start), var(--primary-end));
  box-shadow: 0 6px 18px rgba(102,126,234,0.18);
}
.btn:disabled { opacity: 0.6; cursor: not-allowed; }
.btn.outline {
  background: transparent;
  color: #0f172a;
  border: 1px solid rgba(15,23,42,0.06);
  box-shadow: none;
}

/* inputs */
.label {
  display: block;
  text-align: left;
  font-size: 13px;
  color: #111827;
  margin: 10px 0 6px;
}
.label input {
  width: 100%;
  padding: 10px 12px;
  border-radius: 8px;
  border: 1px solid rgba(15,23,42,0.06);
  margin-top: 6px;
  box-sizing: border-box;
  font-size: 14px;
}

/* helper text */
.muted { color: var(--muted); margin-top: 10px; }
.small { font-size: 13px; }

/* responsive */
@media (max-width: 520px) {
  .card { margin: 12px; }
  .card__header { padding: 20px; }
  .card__body { padding: 18px; }
  .title { font-size: 18px; }
}
</style>