<template>
  <div class="calendar-view">
    <h1>Kalendarz</h1>

    <!-- Nawigacja miesiąc rok -->
    <div class="calendar-header">
      <button @click="previousMonth">◀</button>
      <h2>{{ monthName }} {{ currentYear }}</h2>
      <button @click="nextMonth">▶</button>
    </div>

    <!-- Siatka kalendarza -->
    <div class="calendar-grid">
      <div v-for="day in daysOfWeek" :key="day" class="day-header">
        {{ day }}
      </div>

      <!-- Puste komórki przed pierwszym dniem miesiąca -->
      <div v-for="blank in blankAtStart" :key="'blank-' + blank" class="calendar-day empty"></div>

      <!-- Dni miesiąca -->
      <div 
        v-for="day in daysInMonth" 
        :key="day" 
        class="calendar-day"
        :class="{ 'is-today': isToday(day), 'has-data': getDayData(day).hours > 0 }"
        @click="openDayModal(day)"
      >
        <div class="day-number">{{ day }}</div>

        <!-- Godziny (jeśli są) -->
        <div v-if="getDayData(day).hours" class="hours-display">
          {{ getDayData(day).hours }}h
        </div>

        <!-- Zarobki tego dnia (jeśli są) -->
        <div v-if="getDayData(day).earnings" class="earnings">
          {{ formatCurrency(getDayData(day).earnings) }}
        </div>
      </div>
    </div>

    <!-- MODAL DO EDYCJI DNIA -->
    <Transition name="modal">
      <div v-if="showModal" class="modal-overlay" @click="closeModal">
        <div class="modal-content" @click.stop>
          <div class="modal-header">
            <h3>{{ selectedDay }} {{ monthName }} {{ currentYear }}</h3>
            <button @click="closeModal" class="close-btn">✕</button>
          </div>

          <div class="modal-body">
            <!-- Aktualna stawka -->
            <div class="rate-display">
              <span class="rate-label">Aktualna stawka:</span>
              <span class="rate-value">{{ modalData.currentRate }} zł/h</span>
            </div>

            <!-- Czas rozpoczęcia -->
            <div class="form-group">
              <label for="start-time">Czas rozpoczęcia:</label>
              <input 
                id="start-time"
                v-model="modalData.startTime" 
                type="time"
              />
            </div>

            <!-- Czas zakończenia -->
            <div class="form-group">
              <label for="end-time">Czas zakończenia:</label>
              <input 
                id="end-time"
                v-model="modalData.endTime" 
                type="time"
              />
            </div>

            <!-- Wyliczone godziny -->
            <div v-if="calculatedHours > 0" class="calculated-hours">
              <span class="hours-label">Przepracowane godziny:</span>
              <span class="hours-value">{{ calculatedHours }}h</span>
            </div>
          </div>

          <div class="modal-footer">
            <button @click="closeModal" class="btn-cancel">Anuluj</button>
            <button @click="saveModalData" class="btn-save">Zapisz</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import api from '@/services/api'

const currentMonth = ref(new Date().getMonth())
const currentYear = ref(new Date().getFullYear())
const workData = ref({})
const daysOfWeek = ['Pn', 'Wt', 'Śr', 'Cz', 'Pt', 'So', 'Nd']

// Modal state
const showModal = ref(false)
const selectedDay = ref(null)
const modalData = ref({
  startTime: '',
  endTime: '',
})

// Computed
const monthName = computed(() => {
  const date = new Date(currentYear.value, currentMonth.value)
  return date.toLocaleDateString('pl-PL', { month: 'long' })
})

const daysInMonth = computed(() => {
  return new Date(currentYear.value, currentMonth.value + 1, 0).getDate()
})

const blankAtStart = computed(() => {
  const firstDay = new Date(currentYear.value, currentMonth.value, 1).getDay()
  return firstDay === 0 ? 6 : firstDay - 1
})

// Computed hours from time difference
const calculatedHours = computed(() => {
  if (!modalData.value.startTime || !modalData.value.endTime) {
    return 0
  }

  const [startHour, startMin] = modalData.value.startTime.split(':').map(Number)
  const [endHour, endMin] = modalData.value.endTime.split(':').map(Number)

  const startMinutes = startHour * 60 + startMin
  const endMinutes = endHour * 60 + endMin

  const diffMinutes = endMinutes - startMinutes
  
  if (diffMinutes <= 0) {
    return 0
  }

  return (diffMinutes / 60).toFixed(2)
})

// Methods
const getDateKey = (day) => {
  const month = String(currentMonth.value + 1).padStart(2, '0')
  const dayStr = String(day).padStart(2, '0')
  return `${currentYear.value}-${month}-${dayStr}`
}

const getDayData = (day) => {
  const dateKey = getDateKey(day)
  return workData.value[dateKey] || { hours: 0, description: '', earnings: 0 }
}

const isToday = (day) => {
  const today = new Date()
  return (
    day === today.getDate() &&
    currentMonth.value === today.getMonth() &&
    currentYear.value === today.getFullYear()
  )
}

const formatCurrency = (amount) => {
  return `${Number(amount).toFixed(2)} zł`
}

const previousMonth = () => {
  if (currentMonth.value === 0) {
    currentMonth.value = 11
    currentYear.value--
  } else {
    currentMonth.value--
  }
}

const nextMonth = () => {
  if (currentMonth.value === 11) {
    currentMonth.value = 0
    currentYear.value++
  } else {
    currentMonth.value++
  }
}

// Fetch current hourly rate
const fetchCurrentRate = async () => {
  try {
      const data = await api.hourlyRate.getHourlyRate();
      modalData.value.currentRate = data.rate;
    } catch (error) {
    console.error('Błąd pobierania stawki:', error)
  }
}

// Open modal for day
const openDayModal = async (day) => {
  selectedDay.value = day
  const dayData = getDayData(day)
  
  // Fetch current rate
  await fetchCurrentRate()

  // If has existing data, convert hours to times (approximate)
  if (dayData.hours > 0) {
    // Default: 9:00 start
    modalData.value.startTime = '09:00'
    const endHour = 9 + Math.floor(dayData.hours)
    const endMin = (dayData.hours % 1) * 60
    modalData.value.endTime = `${String(endHour).padStart(2, '0')}:${String(Math.round(endMin)).padStart(2, '0')}`
  } else {
    modalData.value.startTime = ''
    modalData.value.endTime = ''
  }

  modalData.value.description = dayData.description || ''
  
  showModal.value = true
}

// Close modal
const closeModal = () => {
  showModal.value = false
  selectedDay.value = null
  modalData.value = { 
    startTime: '', 
    endTime: '', 
    description: '',
    currentRate: 0
  }
}

const combineDateAndTime = (day, timeString) => {
  const [hours, minutes] = timeString.split(':')
  const dateTime = new Date(currentYear.value, currentMonth.value, day)
  dateTime.setHours(parseInt(hours), parseInt(minutes), 0, 0)
  return dateTime.toISOString()
}


// Save modal data
const saveModalData = async () => {
  if (!selectedDay.value) return

  const hours = parseFloat(calculatedHours.value)
  
  if (hours <= 0 && !modalData.value.description) {
    alert('Wprowadź czas pracy!')
    return
  }

  const dateKey = getDateKey(selectedDay.value)

  try {
    const payload = {
      date: dateKey,
      startTime: combineDateAndTime(selectedDay.value, modalData.value.startTime),
      endTime: combineDateAndTime(selectedDay.value, modalData.value.endTime)
    }

    const saved = await api.workTime.save(payload)

    workData.value[dateKey] = {
      earnings: saved.totalEarnings || 0
    }

    closeModal()
  } catch (error) {
    console.error('❌ Błąd zapisu:', error)
  }
}

// Initialize empty month data
const initializeMonthData = () => {
  const tempData = {}
  for (let day = 1; day <= daysInMonth.value; day++) {
    const dateKey = getDateKey(day)
    tempData[dateKey] = {
      id: null,
      hours: 0,
      description: '',
      earnings: 0
    }
  }
  workData.value = tempData
}

// Fetch month data
const fetchMonthData = async () => {
  initializeMonthData()

  const month = currentMonth.value + 1
  const year = currentYear.value

  try {
      const data = await api.workTime.getMonthData(year, month)

      data.forEach((day) => {
        const dateKey = day.date.substring(0, 10)
        if (workData.value[dateKey]) {
          workData.value[dateKey] = {
            earnings: day.totalEarnings || 0
          }
        }
      })
    } catch (error) {
    console.error('Błąd pobierania danych:', error)
  }
}

// Watch month/year changes
watch([currentMonth, currentYear], () => {
  fetchMonthData()
})

onMounted(() => {
  fetchMonthData()
})
</script>

<style scoped>
.calendar-view {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

.calendar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.calendar-header button {
  padding: 8px 16px;
  font-size: 18px;
  cursor: pointer;
  border: 1px solid #ddd;
  background: #f5f5f5;
  border-radius: 4px;
}

.calendar-header button:hover {
  background: #e0e0e0;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
}

.day-header {
  font-weight: bold;
  text-align: center;
  padding: 10px;
  background: #f0f0f0;
}

.calendar-day {
  min-height: 80px;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
  background: white;
  display: flex;
  flex-direction: column;
  cursor: pointer;
  transition: all 0.2s;
}

.calendar-day:hover {
  background: #f9f9f9;
  border-color: #999;
  transform: translateY(-2px);
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.calendar-day.empty {
  background: #f9f9f9;
  border: none;
  cursor: default;
}

.calendar-day.empty:hover {
  transform: none;
  box-shadow: none;
}

.calendar-day.is-today {
  border: 2px solid #4caf50;
  background: #e8f5e9;
}

.calendar-day.has-data {
  background: #f0f8ff;
}

.day-number {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 4px;
}

.hours-display {
  font-size: 14px;
  color: #666;
  margin-top: 4px;
}

.earnings {
  margin-top: auto;
  padding: 4px;
  background: #e3f2fd;
  border-radius: 3px;
  text-align: center;
  font-size: 12px;
  font-weight: bold;
  color: #1976d2;
}

/* MODAL */
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
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.modal-header h3 {
  margin: 0;
  font-size: 20px;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #666;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
}

.close-btn:hover {
  background: #f0f0f0;
}

.modal-body {
  padding: 30px;
}

/* Wyświetlanie stawki */
.rate-display {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #e8f5e9;
  border-radius: 6px;
  margin-bottom: 20px;
  border: 1px solid #c8e6c9;
}

.rate-label {
  font-weight: 600;
  color: #2e7d32;
}

.rate-value {
  font-size: 18px;
  font-weight: bold;
  color: #1b5e20;
}

/* Wyliczone godziny */
.calculated-hours {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #e3f2fd;
  border-radius: 6px;
  margin-top: 20px;
  border: 1px solid #bbdefb;
}

.hours-label {
  font-weight: 600;
  color: #1565c0;
}

.hours-value {
  font-size: 18px;
  font-weight: bold;
  color: #0d47a1;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #333;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  font-family: inherit;
  box-sizing: border-box;
}

.form-group input[type="time"] {
  font-size: 16px;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #4caf50;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid #e0e0e0;
}

.btn-cancel,
.btn-save {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-cancel {
  background: #f5f5f5;
  color: #666;
}

.btn-cancel:hover {
  background: #e0e0e0;
}

.btn-save {
  background: #4caf50;
  color: white;
}

.btn-save:hover {
  background: #45a049;
}

/* Modal animations */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .modal-content,
.modal-leave-active .modal-content {
  transition: transform 0.3s;
}

.modal-enter-from .modal-content,
.modal-leave-to .modal-content {
  transform: scale(0.9);
}
</style>