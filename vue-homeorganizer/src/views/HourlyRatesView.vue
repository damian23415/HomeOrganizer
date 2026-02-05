<template>
  <div class="hourly-rates-view">
    <h1>💰 Zarządzanie stawkami godzinowymi</h1>
    
    <!-- AKTUALNA STAWKA -->
    <div class="current-rate-section">
      <div v-if="currentRate" class="current-rate-card">
        <div class="badge-active">✅ Aktualna stawka</div>
        <div class="rate-amount">{{ currentRate.ratePerHour }} zł/h</div>
        <div class="rate-info">
          <p>📅 Obowiązuje od: {{ currentRate.effectiveFrom }}</p>
          <p>💰 Zarobki w tym okresie: {{ currentRate.totalEarnings }} zł</p>
        </div>
      </div>
    </div>

    <!-- DODAJ NOWĄ STAWKĘ -->
    <div class="add-rate-section">
      <h3>➕ Dodaj nową stawkę</h3>
      <form @submit.prevent="handleAddRate" class="add-rate-form">
        <div class="form-row">
          <div class="form-group">
            <label for="rate">Stawka godzinowa (zł/h):</label>
            <input 
              id="rate"
              v-model="newRate.amount" 
              type="number" 
              step="0.01"
              placeholder="100.00"
              required 
            />
          </div>
          
          <div class="form-group">
            <label for="effective-date">Obowiązuje od:</label>
            <input 
              id="effective-date"
              v-model="newRate.effectiveFrom" 
              type="date"
              required 
            />
          </div>
          
          <button type="submit" class="btn-submit">Zapisz</button>
        </div>
      </form>
    </div>

    <!-- HISTORIA STAWEK -->
    <div class="history-section">
      <h3>📚 Historia stawek</h3>
      
      <div 
        v-for="(rate, index) in historicalRates"
        :key="index"
        class="rate-card">
        <div class="card-header">
          <span class="badge-archive">📦 Archiwalna</span>
          <span class="rate-value">{{ rate.ratePerHour}}zł/h</span>
        </div>
        <div class="card-body">
          <p>📅 {{ rate.effectiveFrom }} - {{ rate.effectiveTo }} ({{ rate.totalHours / 24}} dni)</p>
          <p>💰 Zarobki: {{ rate.totalEarnings }} zł</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import api from '@/services/api'

const historicalRates = ref([])
const currentRate = ref(null)
const newRate = ref({
  amount: '',
  effectiveFrom: ''
})

const months = [
    'stycznia', 'lutego', 'marca', 'kwietnia', 'maja', 'czerwca',
    'lipca', 'sierpnia', 'września', 'października', 'listopada', 'grudnia'
  ]

const formatDate = (dateString) => {
    if (!dateString) return '-'

    const date = new Date(dateString)
    const day = date.getDate()
    const month = months[date.getMonth()]
    const year = date.getFullYear()

    return `${day} ${month} ${year}`
}

const fetchRates = async () => {
    try {
        const response = await api.hourlyRate.getHourlyRates();

        currentRate.value = response.find(rate => rate.isCurrentRate) || null;

        if (currentRate.value?.effectiveFrom) {
          currentRate.value.effectiveFrom = formatDate(currentRate.value.effectiveFrom)
        }

        historicalRates.value = response
        .filter(rate => !rate.isCurrentRate)
        .map(rate => ({
          ...rate,
          effectiveFrom: formatDate(rate.effectiveFrom),
          effectiveTo: formatDate(rate.effectiveTo)
        }))

      } catch (error) {
        console.log('Błąd:', error);
      }
}

const handleAddRate = async () => {
    try {
        if (!newRate.value.amount || !newRate.value.effectiveFrom) {
            alert('⚠️ Wypełnij wszystkie pola!')
            return
        }

        const payload = {
            hourlyRate: parseFloat(newRate.value.amount),
            startDate: newRate.value.effectiveFrom
        }

        const response = await api.hourlyRate.save(payload);

        currentRate.value = {
            rate: newRate.value.amount,
            startDate: formatDate(newRate.value.effectiveFrom)
        }

        newRate.value = {
            amount: '',
            effectiveFrom: ''
        }
    } catch (error) {
        console.error('❌ Błąd:', error)
        alert('❌ Nie udało się zapisać stawki!')
    }
}

onMounted(() => {
    fetchRates();
})
</script>

<style scoped>
.hourly-rates-view {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #1e293b;
}

h3 {
  font-size: 1.25rem;
  margin-bottom: 1rem;
  color: #334155;
}

/* AKTUALNA STAWKA */
.current-rate-section {
  margin-bottom: 2rem;
}

.current-rate-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(102, 126, 234, 0.3);
}

.badge-active {
  display: inline-block;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.rate-amount {
  font-size: 3rem;
  font-weight: bold;
  margin: 1rem 0;
}

.rate-info p {
  margin: 0.5rem 0;
  font-size: 1rem;
  opacity: 0.9;
}

/* DODAJ NOWĄ STAWKĘ */
.add-rate-section {
  background: white;
  padding: 2rem;
  border-radius: 16px;
  border: 2px dashed #cbd5e1;
  margin-bottom: 2rem;
}

.add-rate-form {
  margin-top: 1rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr auto;
  gap: 1rem;
  align-items: end;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #475569;
  font-size: 0.875rem;
}

.form-group input {
  padding: 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.2s;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.btn-submit {
  background: #667eea;
  color: white;
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 1rem;
}

.btn-submit:hover {
  background: #5568d3;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

/* HISTORIA */
.history-section {
  margin-top: 2rem;
}

.rate-card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  margin-bottom: 1rem;
  transition: all 0.2s;
}

.rate-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  transform: translateY(-2px);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.badge-archive {
  background: #94a3b8;
  color: white;
  padding: 0.375rem 0.875rem;
  border-radius: 12px;
  font-size: 0.875rem;
  font-weight: 600;
}

.rate-value {
  font-size: 1.5rem;
  font-weight: bold;
  color: #1e293b;
}

.card-body p {
  margin: 0.5rem 0;
  color: #64748b;
  font-size: 0.9375rem;
}

/* RESPONSIVE */
@media (max-width: 768px) {
  .hourly-rates-view {
    padding: 1rem;
  }

  .form-row {
    grid-template-columns: 1fr;
  }

  .rate-amount {
    font-size: 2rem;
  }

  .btn-submit {
    width: 100%;
  }
}
</style>