<template>
  <div class="modal-overlay" @click="closeModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h3>Rejestracja czasu pracy</h3>
        <button @click="closeModal" class="close-btn">✕</button>
      </div>
      
      <div class="modal-body">
        <!-- Wybrana data -->
        <div class="info-section">
          <div class="info-row">
            <span class="label">Data:</span>
            <span class="value">{{ formattedDate }}</span>
          </div>
          <div class="info-row">
            <span class="label">Stawka:</span>
            <span class="value">{{ hourlyRate }} zł/h</span>
          </div>
        </div>

        <!-- Formularz czasu pracy -->
        <form @submit.prevent="handleSave" class="work-time-form">
          <div class="form-group">
            <label for="start-time">Godzina rozpoczęcia:</label>
            <input 
              id="start-time"
              v-model="startTime"
              type="time"
              class="time-input"
              required
            />
          </div>

          <div class="form-group">
            <label for="end-time">Godzina zakończenia:</label>
            <input 
              id="end-time"
              v-model="endTime"
              type="time"
              class="time-input"
              required
            />
          </div>

          <!-- Przycisk zapisu -->
          <button type="submit" class="save-btn">
            💾 Zapisz
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  date: {
    type: Date,
    required: true
  }
})

const emit = defineEmits(['close', 'save'])

const startTime = ref('')
const endTime = ref('')
const hourlyRate = ref(100)

const formattedDate = computed(() => {
  return props.date?.toLocaleDateString('pl-PL', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
})

// ⬇️ SPRAWDŹ CZY TA FUNKCJA ISTNIEJE!
const combineDateAndTime = (date, timeString) => {
  const [hours, minutes] = timeString.split(':')
  const dateTime = new Date(date)
  dateTime.setHours(parseInt(hours), parseInt(minutes), 0, 0)
  
  const isoString = dateTime.toISOString()
  console.log(`🕐 combineDateAndTime: ${timeString} → ${isoString}`) // DEBUG
  
  return isoString
}

const closeModal = () => {
  emit('close')
}

const handleSave = () => {
  if (!startTime.value || !endTime.value) {
    alert('Proszę wypełnić wszystkie pola')
    return
  }
  
  const [startHour, startMinute] = startTime.value.split(':').map(Number)
  const [endHour, endMinute] = endTime.value.split(':').map(Number)
  const startInMinutes = startHour * 60 + startMinute
  const endInMinutes = endHour * 60 + endMinute
  
  if (endInMinutes <= startInMinutes) {
    alert('Godzina zakończenia musi być późniejsza niż rozpoczęcia')
    return
  }
  
  // ⬇️ SPRAWDŹ CZY TO SIĘ WYKONUJE!
  console.log('📝 props.date:', props.date)
  console.log('📝 startTime.value:', startTime.value)
  console.log('📝 endTime.value:', endTime.value)
  
  const workData = {
    Date: props.date.toISOString(),
    StartTime: combineDateAndTime(props.date, startTime.value),
    EndTime: combineDateAndTime(props.date, endTime.value)
  }
  
  console.log('📤 Wysyłam do API (po konwersji):', workData)
  
  emit('save', workData)
  closeModal()
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 450px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from { 
    transform: translateY(30px);
    opacity: 0;
  }
  to { 
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 2px solid #f0f0f0;
}

.modal-header h3 {
  margin: 0;
  font-size: 20px;
  color: #333;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #999;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 6px;
  transition: all 0.2s;
}

.close-btn:hover {
  background: #f0f0f0;
  color: #333;
}

.modal-body {
  padding: 24px;
}

.info-section {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 24px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.info-row:last-child {
  margin-bottom: 0;
}

.info-row .label {
  color: #666;
  font-size: 14px;
}

.info-row .value {
  font-weight: 600;
  color: #333;
  font-size: 15px;
  text-transform: capitalize;
}

.work-time-form {
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

.time-input {
  padding: 12px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 16px;
  transition: all 0.2s;
}

.time-input:focus {
  outline: none;
  border-color: #2196f3;
  box-shadow: 0 0 0 3px rgba(33, 150, 243, 0.1);
}

.save-btn {
  padding: 14px 24px;
  background: #2196f3;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.2s;
  margin-top: 8px;
}

.save-btn:hover {
  background: #1976d2;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(33, 150, 243, 0.3);
}

.save-btn:active {
  transform: translateY(0);
}
</style>