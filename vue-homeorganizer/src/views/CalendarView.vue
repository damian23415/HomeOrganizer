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
        <div 
        v-for="day in daysOfWeek"
        :key="day"
        class="day-header"
        >
          {{ day }}
        </div>

        <!-- Puste komórki przed pierwszym dniem miesiąca -->
        <div
        v-for="blank in blankAtStart"
        :key="'blank-' + blank"
        class="calendar-day empty"
        ></div>

        <!-- Dni miesiąca -->
        <div
        v-for="day in daysInMonth"
          :key="day"
          class="calendar-day"
          :class="{'today': isToday(day), 'has-work': getDayEarnings(day)}"
          @click="openModal(day)"
        >
          <span class="day-number">{{ day }}</span>
          <span v-if="getDayEarnings(day)" class="earnings">
            {{ getDayEarnings(day) }} zł
          </span>
        </div>
     </div>

    <!-- Modal (będzie widoczny po kliknięciu) -->
     <CalendarModal
      v-if="showModal"
      :date="selectedDate"
      @close="closeModal"
      @save="handleSaveWork"
     />
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue';
  import CalendarModal from '@/components/CalendarModal.vue';

  // Stan reaktywny
  const currentMonth = ref(new Date().getMonth());
  const currentYear = ref(new Date().getFullYear());
  const showModal = ref(false);
  const selectedDate = ref(null);

  // Dane o pracy - tutaj będą przechowywane kwoty z API
  // Klucz: 'YYYY-MM-DD', wartość: kwota zarobiona
  const workData = ref({
    // Przykładowe dane - później będą z API
    '2026-01-15': 800,
    '2026-01-16': 650,
    '2026-01-20': 920
  });

  // Nazwy dni tygodnia
  const daysOfWeek = ['Pn', 'Wt', 'Śr', 'Cz', 'Pt', 'Sb', 'Nd'];

  // Obliczenia
  const monthName = computed(() => {
    const date = new Date(currentYear.value, currentMonth.value);
    return date.toLocaleDateString('pl-PL', { month: 'long' });
  })

  const daysInMonth = computed(() => {
    return new Date(currentYear.value, currentMonth.value + 1, 0).getDate();
  })

  const blankAtStart = computed(() => {
    const firstDay = new Date(currentYear.value, currentMonth.value, 1).getDay();
    return firstDay === 0 ? 6 : firstDay - 1
  })

  // Metody
  const previousMonth = () => {
    if (currentMonth.value === 0) {
      currentMonth.value = 11;
      currentYear.value--;
    } else {
      currentMonth.value--;
    }
  }

  const nextMonth = () => {
    if (currentMonth.value === 11) {
      currentMonth.value = 0;
      currentYear.value++;
    } else {
      currentMonth.value++;
    }
  }

  const isToday = (day) => {
    const today = new Date();
    return (
      day === today.getDate() &&
      currentMonth.value === today.getMonth() &&
      currentYear.value === today.getFullYear()
    )
  }

  const getDayEarnings = (day) => {
    const dateKey = `${currentYear.value}-${String(currentMonth.value + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
    return workData.value[dateKey] || null;
  }

  const openModal = (day) => {
    selectedDate.value = new Date(currentYear.value, currentMonth.value, day);
    showModal.value = true;
  }

  const closeModal = () => {
    showModal.value = false;
    selectedDate.value = null;
  }

  const handleSaveWork = async (data) => {
    console.log('📥 Otrzymane dane z modala:', data);
    
    // TODO: Tutaj wywołanie API
    // const response = await api.saveWorkTime(data)
    
    // MOCK - symulacja odpowiedzi z API
    const mockResponse = {
      success: true,
      date: data.date,
      totalEarnings: 825 // Przykładowa kwota obliczona przez backend
    };
    
    console.log('✅ Odpowiedź z API:', mockResponse);
    
    // Zapisz kwotę w danych lokalnych
    if (mockResponse.success) {
      workData.value[mockResponse.date] = mockResponse.totalEarnings;
    }
  }
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
    gap: 5px;
  }

  .day-header {
    text-align: center;
    font-weight: bold;
    padding: 10px;
    background: #f0f0f0;
    border-radius: 4px;
  }

  .calendar-day {
    aspect-ratio: 1;
    position: relative;  /* ⬅️ ZMIENIONE */
    display: flex;
    align-items: center;
    justify-content: center;
    border: 1px solid #ddd;
    border-radius: 4px;
    cursor: pointer;
    transition: all 0.2s;
    padding: 8px;
  }

  .calendar-day:hover {
    background: #e3f2fd;
    transform: scale(1.05);
  }

  .calendar-day.empty {
    border: none;
    cursor: default;
  }

  .calendar-day.today {
    background: #2196f3;
    color: white;
    font-weight: bold;
  }

  .day-number {
    font-size: 16px;
    font-weight: 500;
  }

 .earnings {
    position: absolute;  /* ⬅️ NOWE */
    bottom: 4px;         /* ⬅️ NOWE */
    left: 4px;           /* ⬅️ NOWE */
    font-size: 14px;
    color: #2e7d32;
    font-weight: 600;
    background: rgba(255, 255, 255, 0.8);  /* ⬅️ NOWE - białe tło */
    padding: 2px 4px;    /* ⬅️ NOWE */
    border-radius: 3px;  /* ⬅️ NOWE */
  }

 .calendar-day.today .earnings {
    color: #fff;
    background: rgba(0, 0, 0, 0.2);  /* ⬅️ NOWE - ciemne tło dla today */
  }

  .calendar-day.has-work .earnings {
    background: rgba(255, 255, 255, 0.9);  /* ⬅️ NOWE */
  }
</style>