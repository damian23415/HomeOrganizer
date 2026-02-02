<template>
  <div class="calendar-view">
    <h1>Kalendarz</h1>
    <div class="user-menu">
      <span class="username">{{ authStore.user?.username }}</span>
      <button @click="handleLogout" class="logout-btn">Wyloguj</button>
    </div>

    <!-- Loading spinner -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
      <p>Ładowanie danych...</p>
    </div>

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
      <div v-for="day in daysInMonth" :key="day" class="calendar-day"
        :class="{ 'today': isToday(day), 'has-work': getDayEarnings(day) }" @click="openModal(day)">
        <span class="day-number">{{ day }}</span>
        <span v-if="getDayEarnings(day)" class="earnings">
          {{ getDayEarnings(day) }} zł
        </span>
      </div>
    </div>

    <!-- Modal (będzie widoczny po kliknięciu) -->
    <CalendarModal v-if="showModal" :date="selectedDate" @close="closeModal" @save="handleSaveWork" />
  </div>
</template>

<script setup>
  import { ref, computed, watch, onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';
  import CalendarModal from '@/components/CalendarModal.vue';
  import { workTimeApi } from '@/services/api';

  const router = useRouter();
  const authStore = useAuthStore();

  // Stan reaktywny
  const currentMonth = ref(new Date().getMonth());
  const currentYear = ref(new Date().getFullYear());
  const showModal = ref(false);
  const selectedDate = ref(null);
  const workData = ref({});
  const loading = ref(false);

  const daysOfWeek = ['Pn', 'Wt', 'Śr', 'Cz', 'Pt', 'Sb', 'Nd'];

  // ============================================
  // HELPER: Parsowanie daty bez strefy czasowej
  // ============================================
  const parseDateToKey = (dateString) => {
    if (!dateString) return null
    
    // Jeśli data zawiera 'T', weź tylko część przed T (YYYY-MM-DD)
    if (dateString.includes('T')) {
      return dateString.split('T')[0]
    }
    
    // Jeśli to już format YYYY-MM-DD
    const match = dateString.match(/^\d{4}-\d{2}-\d{2}/)
    return match ? match[0] : null
  }

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

  // Formatowanie waluty
  const formatCurrency = (amount) => {
    return `${Number(amount).toFixed(2)} zł`
  }

  // ============================================
  // Pobierz dane z API dla miesiąca
  // ============================================
  const fetchMonthData = async () => {
    loading.value = true
    
    console.log(`📅 Pobieram dane dla: ${currentMonth.value + 1}/${currentYear.value}`)
    
    try {
      const response = await workTimeApi.getMonthData(
        currentYear.value, 
        currentMonth.value + 1
      )
      
      console.log('✅ Otrzymane dane z API:', response)
      
      // Backend zwraca tablicę: [{ date, totalEarnings }, ...]
      // Konwertuj na obiekt: { "2026-01-15": 800, ... }
      const dataMap = {}
      
      // Obsługa różnych formatów (PascalCase i camelCase)
      const items = Array.isArray(response) ? response : (response.data || [])
      
      items.forEach(item => {
        // Backend może zwracać Date lub date, TotalEarnings lub totalEarnings
        const dateString = item.date || item.Date
        const earnings = item.totalEarnings || item.TotalEarnings
        
        if (dateString && earnings !== undefined) {
          // Użyj helpera zamiast new Date()
          const dateKey = parseDateToKey(dateString)
          
          if (dateKey) {
            dataMap[dateKey] = earnings
            console.log(`📊 ${dateString} → ${dateKey} = ${earnings} zł`)
          }
        }
      })
      
      workData.value = dataMap
      console.log('📊 Przetworzone dane:', dataMap)
      
    } catch (error) {
      console.error('❌ Błąd pobierania danych:', error)
      
      if (!error.message.includes('Sesja wygasła')) {
        alert(`Nie udało się pobrać danych: ${error.message}`)
      }
      
      workData.value = {}
    } finally {
      loading.value = false
    }
  }

  // Obserwuj zmiany miesiąca/roku
  watch([currentMonth, currentYear], () => {
    fetchMonthData()
  })

  // Pobierz dane przy pierwszym załadowaniu
  onMounted(() => {
    fetchMonthData()
  })

  // ============================================
  // Metody
  // ============================================
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
    // ⬇️ ZMIEŃ: Ustaw godzinę 12:00 zamiast 00:00
    const date = new Date(currentYear.value, currentMonth.value, day, 12, 0, 0)
    selectedDate.value = date
    showModal.value = true
    
    console.log('📅 Otwieramy modal dla:', date.toISOString())
  }

  const closeModal = () => {
    showModal.value = false;
  }

  const handleSaveWork = async (data) => {
    console.log('📥 Zapisuję dane:', data);
    
    try {
      const response = await workTimeApi.save(data)
      
      console.log('✅ Zapisano:', response);
      
      // Backend może zwracać Date lub date
      const responseDate = response.date || response.Date
      const responseEarnings = response.totalEarnings || response.TotalEarnings
      
      // Użyj helpera
      const dateKey = parseDateToKey(responseDate)
      
      if (dateKey && responseEarnings !== undefined) {
        // Aktualizuj dane lokalne
        workData.value[dateKey] = responseEarnings
      }
      
    } catch (error) {
      console.error('❌ Błąd zapisu:', error)
      alert(`Nie udało się zapisać: ${error.message}`)
    }
  }

  const handleLogout = () => {
    if (confirm('Czy na pewno chcesz się wylogować?')) {
      authStore.logout()
      router.push('/login')
    }
  }
</script>

<style scoped>
.header-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header-bar h1 {
  margin: 0;
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 12px;
}

.username {
  font-size: 14px;
  color: #666;
}

.logout-btn {
  padding: 8px 16px;
  background: #f44336;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.logout-btn:hover {
  background: #d32f2f;
}

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
  position: relative;
  /* ⬅️ ZMIENIONE */
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
  position: absolute;
  /* ⬅️ NOWE */
  bottom: 4px;
  /* ⬅️ NOWE */
  left: 4px;
  /* ⬅️ NOWE */
  font-size: 14px;
  color: #2e7d32;
  font-weight: 600;
  background: rgba(255, 255, 255, 0.8);
  /* ⬅️ NOWE - białe tło */
  padding: 2px 4px;
  /* ⬅️ NOWE */
  border-radius: 3px;
  /* ⬅️ NOWE */
}

.calendar-day.today .earnings {
  color: #fff;
  background: rgba(0, 0, 0, 0.2);
  /* ⬅️ NOWE - ciemne tło dla today */
}

.calendar-day.has-work .earnings {
  background: rgba(255, 255, 255, 0.9);
  /* ⬅️ NOWE */
}
</style>