<template>
  <aside class="sidebar">
    <nav class="sidebar-nav">
      <!-- Każda sekcja (PRACA, ANALIZA, USTAWIENIA) -->
      <div 
        v-for="section in menuSections" 
        :key="section.id"
        class="menu-section"
        :class="{ 'has-active': sectionHasActive(section) }"
      >
        <!-- Section Header (klikalny) -->
        <button 
          @click="toggleSection(section.id)"
          class="section-header"
        >
          <span class="section-title">{{ section.title }}</span>
          <span 
            class="expand-icon"
            :class="{ expanded: section.expanded }"
          >
            ▶
          </span>
        </button>

        <!-- Section Items (rozwijane) -->
        <Transition name="expand">
          <div v-show="section.expanded" class="section-items">
            <router-link
              v-for="item in section.items"
              :key="item.id"
              :to="item.disabled ? '#' : item.path"
              class="menu-item"
              :class="{
                active: isActive(item.path),
                disabled: item.disabled
              }"
              @click="item.disabled && $event.preventDefault()"
            >
              <span class="item-icon">{{ item.icon }}</span>
              <span class="item-title">{{ item.title }}</span>
              <span v-if="isActive(item.path)" class="active-dot">●</span>
            </router-link>
          </div>
        </Transition>
      </div>
    </nav>
  </aside>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

// Menu structure
const menuSections = ref([
  {
    id: 'work',
    title: 'PRACA',
    icon: '💼',
    expanded: true,  // Domyślnie rozwinięte
    items: [
      {
        id: 'work-log',
        title: 'Work Log',
        icon: '📅',
        path: '/work-log',
        disabled: false
      },
      {
        id: 'hourly-rates',
        title: 'Stawki',
        icon: '💰',
        path: '/hourly-rates',
        disabled: false
      }
    ]
  },
  {
    id: 'analysis',
    title: 'ANALIZA',
    icon: '📊',
    expanded: false,
    items: [
      {
        id: 'reports',
        title: 'Raporty',
        icon: '📊',
        path: '/reports',
        disabled: true
      },
      {
        id: 'earnings',
        title: 'Zarobki',
        icon: '💰',
        path: '/earnings',
        disabled: true
      },
      {
        id: 'statistics',
        title: 'Statystyki',
        icon: '📈',
        path: '/statistics',
        disabled: true
      }
    ]
  },
  {
    id: 'settings',
    title: 'USTAWIENIA',
    icon: '⚙️',
    expanded: false,
    items: [
      {
        id: 'rates',
        title: 'Stawki',
        icon: '💵',
        path: '/settings/rates',
        disabled: true
      },
      {
        id: 'profile',
        title: 'Profil',
        icon: '👤',
        path: '/settings/profile',
        disabled: true
      },
      {
        id: 'preferences',
        title: 'Preferencje',
        icon: '⚙️',
        path: '/settings/preferences',
        disabled: true
      }
    ]
  }
])

// Toggle section expand/collapse
const toggleSection = (sectionId) => {
  const section = menuSections.value.find(s => s.id === sectionId)
  if (section) {
    section.expanded = !section.expanded
  }
}

// Check if item is active
const isActive = (path) => {
  return route.path === path
}

// Check if section has active item
const sectionHasActive = (section) => {
  return section.items.some(item => isActive(item.path))
}
</script>

<style scoped>
/* Sidebar */
.sidebar {
  width: 260px;
  background: white;
  border-right: 1px solid #e2e8f0;
  overflow-y: auto;
  flex-shrink: 0;
}

.sidebar-nav {
  padding: 1rem 0;
}

/* Menu Section */
.menu-section {
  margin-bottom: 0.5rem;
}

.menu-section.has-active {
  border-left: 3px solid #0ea5e9;
}

/* Section Header */
.section-header {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
}

.section-header:hover {
  background: #f8fafc;
}

.section-title {
  font-size: 0.75rem;
  font-weight: 600;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  color: #64748b;
}

.expand-icon {
  font-size: 0.625rem;
  color: #94a3b8;
  transition: transform 0.2s;
  display: inline-block;
}

.expand-icon.expanded {
  transform: rotate(90deg);
}

/* Section Items */
.section-items {
  padding-left: 0.5rem;
  overflow: hidden;
}

/* Menu Item */
.menu-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  color: #64748b;
  text-decoration: none;
  border-radius: 8px;
  margin: 0.25rem 0.5rem;
  transition: all 0.2s;
  position: relative;
  font-size: 0.95rem;
}

.menu-item:hover:not(.disabled) {
  background: #f8fafc;
  color: #1e293b;
}

/* Active item */
.menu-item.active {
  background: #e0f2fe;
  color: #0ea5e9;
  font-weight: 600;
  border-left: 3px solid #0ea5e9;
}

/* Disabled item */
.menu-item.disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.menu-item.disabled:hover {
  background: transparent;
}

.item-icon {
  font-size: 1.25rem;
  line-height: 1;
}

.item-title {
  flex: 1;
}

.active-dot {
  color: #0ea5e9;
  font-size: 0.5rem;
  margin-left: auto;
}

/* Expand Animation */
.expand-enter-active,
.expand-leave-active {
  transition: all 0.3s ease;
  max-height: 500px;
}

.expand-enter-from,
.expand-leave-to {
  max-height: 0;
  opacity: 0;
}

/* Mobile */
@media (max-width: 768px) {
  .sidebar {
    width: 100%;
    max-width: 280px;
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    z-index: 200;
    transform: translateX(-100%);
    transition: transform 0.3s ease;
  }

  .sidebar.open {
    transform: translateX(0);
  }
}
</style>