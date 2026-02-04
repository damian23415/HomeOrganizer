<template>
  <div class="main-layout">
    <!-- Top Bar -->
    <header class="app-header">
      <div class="logo">
        <span class="logo-icon">🏠</span>
        <h1 class="logo-text">
          <span class="logo-home">Home</span><span class="logo-organizer">Organizer</span>
        </h1>
      </div>

      <div class="user-actions">
        <div class="user-menu-wrapper">
            <span class="user-avatar">👨‍💼</span>
            <span class="user-name">{{ authStore.user?.email }}</span>
        </div>

        <button @click="handleLogout" class="logout-btn">
          <span class="logout-icon">🚪</span>
          Wyloguj
        </button>
      </div>
    </header>

    <div class="layout-body">
      <Sidebar />

      <main class="content-area">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import Sidebar from '@/components/Sidebar.vue'

const router = useRouter()
const authStore = useAuthStore()

// User menu state
const isUserMenuOpen = ref(false)

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

const handleLogout = () => {
  if (confirm('Czy na pewno chcesz się wylogować?')) {
    authStore.logout()
    router.push('/login')
  }
}
</script>

<style scoped>
/* Layout */
.main-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f8fafc;
}

/* Top Bar */
.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  background: white;
  border-bottom: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  z-index: 100;
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
    gap: 0.5rem;
  padding: 0.5rem 1.25rem;
  background: #f1f5f9;
  color: #475569;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
}

.user-avatar {
  font-size: 1.5rem;
  line-height: 1;
}

.user-name {
  font-weight: 500;
  color: #1e293b;
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

.logout-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1.25rem;
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

.logout-icon {
  font-size: 1rem;
}

/* Body (Sidebar + Content) */
.layout-body {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.content-area {
  flex: 1;
  overflow-y: auto;
  padding: 2rem;
  background: #f8fafc;
}

/* Mobile */
@media (max-width: 768px) {
  .content-area {
    padding: 1rem;
  }
}
</style>