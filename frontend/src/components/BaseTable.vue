<template>
  <div>
    <!-- Filter -->
    <div v-if="filters.length" class="d-flex flex-wrap gap-2 mb-3">
      <div v-for="filter in filters" :key="filter.key" :style="{ width: filter.width || '180px' }">
        <!-- Select -->
        <CFormSelect
          v-if="filter.type === 'select'"
          v-model="filterValues[filter.key]"
          :options="filter.options"
          @change="handleFilterChange"
        />

        <!-- Input -->
        <CFormInput
          v-else-if="filter.type === 'text'"
          v-model="filterValues[filter.key]"
          :placeholder="filter.placeholder || filter.label"
          @input="handleFilterChange"
        />

        <!-- Date -->
        <CFormInput
          v-else-if="filter.type === 'date'"
          v-model="filterValues[filter.key]"
          type="date"
          :placeholder="filter.label"
          @change="handleFilterChange"
        />
      </div>

      <CButton color="secondary" variant="outline" @click="clearFilters"> Clear </CButton>
    </div>

    <!-- Table -->
    <CTable hover responsive>
      <CTableHead>
        <CTableRow>
          <CTableHeaderCell> # </CTableHeaderCell>

          <CTableHeaderCell
            v-for="column in columns"
            :key="column.key"
            :class="{ sortable: column.sortable }"
            @click="column.sortable && handleSort(column.key)"
          >
            <div class="d-flex align-items-center gap-1">
              {{ column.label }}

              <span v-if="column.sortable && sortBy === column.key">
                {{ sortDirection === 'asc' ? '↑' : '↓' }}
              </span>
            </div>
          </CTableHeaderCell>
        </CTableRow>
      </CTableHead>

      <CTableBody>
        <!-- Loading -->
        <CTableRow v-if="loading">
          <CTableDataCell :colspan="columns.length + 1" class="text-center py-4">
            Loading...
          </CTableDataCell>
        </CTableRow>

        <!-- Empty -->
        <CTableRow v-else-if="items.length === 0">
          <CTableDataCell :colspan="columns.length + 1" class="text-center py-4">
            {{ emptyText }}
          </CTableDataCell>
        </CTableRow>

        <!-- Data -->
        <CTableRow v-else v-for="(item, index) in items" :key="item[rowKey] || index">
          <CTableDataCell>
            {{ startIndex + index + 1 }}
          </CTableDataCell>

          <CTableDataCell v-for="column in columns" :key="column.key">
            {{ getValue(item, column.key) }}
          </CTableDataCell>
        </CTableRow>
      </CTableBody>
    </CTable>

    <!-- Footer -->
    <div class="d-flex justify-content-between align-items-center mt-3">
      <div class="d-flex align-items-center gap-3">
        <CFormSelect
          v-model="pageSize"
          :options="pageSizeOptions"
          style="width: 100px"
          @change="handlePageSizeChange"
        />

        <span>
          Showing
          {{ items.length ? startIndex + 1 : 0 }}
          -
          {{ Math.min(startIndex + items.length, totalItems) }}
          of {{ totalItems }}
        </span>
      </div>

      <CPagination align="end" aria-label="Table pagination">
        <CPaginationItem :disabled="page === 1" @click="goToPage(page - 1)">
          Previous
        </CPaginationItem>

        <CPaginationItem
          v-for="pageNumber in visiblePages"
          :key="pageNumber"
          :active="pageNumber === page"
          @click="goToPage(pageNumber)"
        >
          {{ pageNumber }}
        </CPaginationItem>

        <CPaginationItem
          :disabled="page === totalPages || totalPages === 0"
          @click="goToPage(page + 1)"
        >
          Next
        </CPaginationItem>
      </CPagination>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  items: {
    type: Array,
    default: () => [],
  },

  columns: {
    type: Array,
    required: true,
  },

  filters: {
    type: Array,
    default: () => [],
  },

  loading: {
    type: Boolean,
    default: false,
  },

  totalItems: {
    type: Number,
    default: 0,
  },

  totalPages: {
    type: Number,
    default: 0,
  },

  rowKey: {
    type: String,
    default: 'id',
  },

  emptyText: {
    type: String,
    default: 'No data found.',
  },

  defaultPageSize: {
    type: Number,
    default: 10,
  },

  defaultSortBy: {
    type: String,
    default: '',
  },

  defaultSortDirection: {
    type: String,
    default: 'asc',
  },
})

const emit = defineEmits(['change'])

const page = ref(1)

const pageSize = ref(props.defaultPageSize)

const sortBy = ref(props.defaultSortBy)

const sortDirection = ref(props.defaultSortDirection)

const filterValues = ref({})

props.filters.forEach((filter) => {
  filterValues.value[filter.key] = filter.defaultValue ?? ''
})

const pageSizeOptions = [
  { label: '5', value: 5 },
  { label: '10', value: 10 },
  { label: '20', value: 20 },
  { label: '50', value: 50 },
  { label: '100', value: 100 },
]

const startIndex = computed(() => {
  return (page.value - 1) * pageSize.value
})

const visiblePages = computed(() => {
  const pages = []

  const start = Math.max(1, page.value - 2)

  const end = Math.min(props.totalPages, start + 4)

  for (let i = start; i <= end; i++) {
    pages.push(i)
  }

  return pages
})

const getValue = (item, key) => {
  return key.split('.').reduce((value, part) => value?.[part], item) ?? ''
}

const emitChange = () => {
  emit('change', {
    page: page.value,

    pageSize: pageSize.value,

    sortBy: sortBy.value,

    sortDirection: sortDirection.value,

    ...filterValues.value,
  })
}

const goToPage = (pageNumber) => {
  if (pageNumber < 1 || pageNumber > props.totalPages) {
    return
  }

  page.value = pageNumber

  emitChange()
}

const handlePageSizeChange = () => {
  page.value = 1

  emitChange()
}

const handleSort = (column) => {
  if (sortBy.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortBy.value = column

    sortDirection.value = 'asc'
  }

  page.value = 1

  emitChange()
}

const handleFilterChange = () => {
  page.value = 1

  emitChange()
}

const clearFilters = () => {
  props.filters.forEach((filter) => {
    filterValues.value[filter.key] = filter.defaultValue ?? ''
  })

  page.value = 1

  emitChange()
}
</script>

<style scoped>
.sortable {
  cursor: pointer;
  user-select: none;
}

.sortable:hover {
  background-color: rgba(0, 0, 0, 0.05);
}
</style>
