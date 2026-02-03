<template>
  <div class="calendar-view">

    <header class="app-header">
      <div class="logo">
        <span class="logo-icon">🏠</span>
        <h1 class="logo-text">
          <span class="logo-home">Home</span><span class="logo-organizer">Organizer</span>
        </h1>
      </div>

      <div class="user-actions">
        <div class="user-menu-wrapper">
          <button @click="toggleUserMenu" class="user-button" :class="{ active: isUserMenuOpen }">
            <span class="user-avatar">👨‍💼</span>
            <span class="user-name">{{ authStore.user?.email }}</span>
            <span class="dropdown-icon">▼</span>
          </button>

          <div v-if="isUserMenuOpen" class="dropdown-menu">
            <button @click="goToSettings" class="dropdown-item">
              <span class="item-icon">💰</span>
              <span>Zarządzanie stawką</span>
            </button>
          </div>
        </div>


        <button @click="handleLogout" class="logout-btn">Wyloguj</button>
      </div>
    </header>

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
import { ref, computed, watch, onMounted, onUnmounted } from 'vue';
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
const isUserMenuOpen = ref(false)

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

const toggleUserMenu = () => {
  isUserMenuOpen.value = !isUserMenuOpen.value
}

const closeUserMenu = (event) => {
  const userMenuWrapper = document.querySelector('.user-menu-wrapper')
  if (userMenuWrapper && !userMenuWrapper.contains(event.target)) {
    isUserMenuOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', closeUserMenu)
})

onUnmounted(() => {
  document.removeEventListener('click', closeUserMenu)
})

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

watch([currentMonth, currentYear], () => {
  fetchMonthData()
})

onMounted(() => {
  fetchMonthData();
  document.addEventListener('click', closeUserMenu);
})

onUnmounted(() => {
  document.removeEventListener('click', closeUserMenu)
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

const goToSettings = () => {
  isUserMenuOpen.value = false
  alert('Wkrótce: Zarządzanie stawką godzinową!')
  // Później: router.push('/settings')
}
</script>

<style scoped>
.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  background: white;
  border-bottom: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-right: auto;
}

.logo-icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 4px rgba(102, 126, 234, 0.2));
}

.logo-text {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  display: flex;
  letter-spacing: -0.5px;
}

.logo-home {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.logo-organizer {
  color: #1e293b;
  margin-left: 0.1rem;
}

.user-actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.user-menu-wrapper {
  position: relative;
}

.user-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
  font-size: 0.95rem;
}

.user-button:hover,
.user-button.active {
  background: #f1f5f9;
  border-color: #cbd5e1;
}

.user-avatar {
  font-size: 1.25rem;
}

.user-name {
  font-weight: 500;
  color: #1e293b;
  font-size: 0.95rem;
}

.dropdown-icon {
  font-size: 0.75rem;
  color: #64748b;
  transition: transform 0.2s;
}

.dropdown-icon.open {
  transform: rotate(180deg);
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 0.5rem);
  right: 0;
  min-width: 240px;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1),
              0 4px 6px -2px rgba(0, 0, 0, 0.05);
  padding: 0.5rem;
  z-index: 1000;
  animation: dropdownFadeIn 0.2s ease-out;
}

@keyframes dropdownFadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dropdown-item {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.15rem 1rem;
  background: transparent;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
  font-size: 0.95rem;
  color: #1e293b;
  text-align: left;
}

.dropdown-item:hover {
  background: #f8fafc;
}

.item-icon {
  font-size: 1.25rem;
}

.header-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header-bar h1 {
  margin: 0;
}

.username {
  font-size: 14px;
  color: #666;
}

.logout-btn {
  padding: 0.70rem 1.25rem;
  background: #f1f5f9;
  color: #475569;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.95rem;
  font-weight: 500;
  transition: all 0.2s;
  font-family: inherit;
}

.logout-btn:hover {
  background: #e2e8f0;
  border-color: #cbd5e1;
  color: #1e293b;
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
  bottom: 4px;
  left: 4px;
  font-size: 14px;
  color: #2e7d32;
  font-weight: 600;
  background: rgba(255, 255, 255, 0.8);
  padding: 2px 4px;
  border-radius: 3px;
}

.calendar-day.today .earnings {
  color: #fff;
  background: rgba(0, 0, 0, 0.2);
}

.calendar-day.has-work .earnings {
  background: rgba(255, 255, 255, 0.9);
}
</style>