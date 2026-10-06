<template>
  <div>
    <!-- Filters -->
    <div
      v-if="filters.length"
      class="d-flex flex-wrap gap-2 mb-3"
    >
      <div
        v-for="filter in filters"
        :key="filter.key"
        :style="{ width: filter.width || '180px' }"
      >
        <!-- Select -->
        <CFormSelect
          v-if="filter.type === 'select'"
          v-model="filterValues[filter.key]"
          :options="filter.options"
          @change="handleFilterChange(filter.key)"
        />

        <!-- Text -->
        <CFormInput
          v-else-if="filter.type === 'text'"
          v-model="filterValues[filter.key]"
          :placeholder="filter.placeholder || filter.label"
          @input="handleFilterChange(filter.key)"
        />

        <!-- Date -->
        <CFormInput
          v-else-if="filter.type === 'date'"
          v-model="filterValues[filter.key]"
          type="date"
          :placeholder="filter.label"
          @change="handleFilterChange(filter.key)"
        />
      </div>

      <CButton
        color="secondary"
        variant="outline"
        @click="clearFilters"
      >
        Clear
      </CButton>
    </div>


    <!-- Table -->
    <CTable hover responsive>
      <CTableHead>
        <CTableRow>

          <!-- Serial -->
          <CTableHeaderCell>
            #
          </CTableHeaderCell>


          <!-- Dynamic Columns -->
          <CTableHeaderCell
            v-for="column in columns"
            :key="column.key"
            :class="{ sortable: column.sortable }"
            @click="column.sortable && handleSort(column.key)"
          >
            <div class="d-flex align-items-center gap-1">

              {{ column.label }}

              <span
                v-if="
                  column.sortable &&
                  sortBy === column.key
                "
              >
                {{ sortDirection === 'asc' ? '↑' : '↓' }}
              </span>

            </div>
          </CTableHeaderCell>

        </CTableRow>
      </CTableHead>


      <CTableBody>

        <!-- Loading -->
        <CTableRow v-if="loading">
          <CTableDataCell
            :colspan="columns.length + 1"
            class="text-center py-4"
          >
            Loading...
          </CTableDataCell>
        </CTableRow>


        <!-- Empty -->
        <CTableRow v-else-if="items.length === 0">
          <CTableDataCell
            :colspan="columns.length + 1"
            class="text-center py-4"
          >
            {{ emptyText }}
          </CTableDataCell>
        </CTableRow>


        <!-- Data -->
        <CTableRow
          v-else
          v-for="(item, index) in items"
          :key="item[rowKey] || index"
        >

          <!-- Serial -->
          <CTableDataCell>
            {{ startIndex + index + 1 }}
          </CTableDataCell>


          <!-- Dynamic Columns -->
          <CTableDataCell
            v-for="column in columns"
            :key="column.key"
          >

            <!-- Badge Column -->
            <CBadge
              v-if="column.type === 'badge'"
              :color="
                getBadgeColor(
                  getValue(item, column.key),
                  column
                )
              "
            >
              {{ getValue(item, column.key) }}
            </CBadge>


            <!-- Actions Column -->
            <div
              v-else-if="column.type === 'actions'"
              class="d-flex justify-content-center gap-1"
            >

              <CButton
                v-for="action in column.actions || []"
                :key="action.name"
                :color="action.color || 'secondary'"
                size="sm"
                variant="outline"
                :title="action.label"
                @click="handleAction(action.name, item)"
              >

                <CIcon
                  v-if="action.icon"
                  :icon="action.icon"
                  size="sm"
                />

                <span v-else>
                  {{ action.label }}
                </span>

              </CButton>

            </div>


            <!-- Custom Column -->
            <span
              v-else-if="column.type === 'custom'"
            >
              {{
                column.formatter
                  ? column.formatter(item)
                  : getValue(item, column.key)
              }}
            </span>


            <!-- Normal Column -->
            <span v-else>
              {{ getValue(item, column.key) }}
            </span>

          </CTableDataCell>

        </CTableRow>

      </CTableBody>
    </CTable>


    <!-- Footer -->
    <div
      class="d-flex justify-content-between align-items-center mt-3"
    >

      <!-- Page Size + Information -->
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
          {{
            Math.min(
              startIndex + items.length,
              totalItems
            )
          }}
          of {{ totalItems }}
        </span>

      </div>


      <!-- Pagination -->
      <CPagination
        align="end"
        aria-label="Table pagination"
      >

        <!-- Previous -->
        <CPaginationItem
          :disabled="page === 1"
          @click="goToPage(page - 1)"
        >
          Previous
        </CPaginationItem>


        <!-- Pages -->
        <CPaginationItem
          v-for="pageNumber in visiblePages"
          :key="pageNumber"
          :active="pageNumber === page"
          @click="goToPage(pageNumber)"
        >
          {{ pageNumber }}
        </CPaginationItem>


        <!-- Next -->
        <CPaginationItem
          :disabled="
            page === totalPages ||
            totalPages === 0
          "
          @click="goToPage(page + 1)"
        >
          Next
        </CPaginationItem>

      </CPagination>

    </div>
  </div>
</template>


<script setup>
import {
  ref,
  computed,
  onBeforeUnmount,
} from 'vue'

import {
  cilZoomIn,
  cilPencil,
  cilTrash,
} from '@coreui/icons'


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


const emit = defineEmits([
  'change',
  'action',
])


/*
|--------------------------------------------------------------------------
| Table State
|--------------------------------------------------------------------------
*/

const page = ref(1)

const pageSize = ref(
  String(props.defaultPageSize)
)

const sortBy = ref(
  props.defaultSortBy
)

const sortDirection = ref(
  props.defaultSortDirection
)

const filterValues = ref({})


/*
|--------------------------------------------------------------------------
| Initialize Filters
|--------------------------------------------------------------------------
*/

props.filters.forEach((filter) => {
  filterValues.value[filter.key] =
    filter.defaultValue ?? ''
})


/*
|--------------------------------------------------------------------------
| Page Size Options
|--------------------------------------------------------------------------
*/

const pageSizeOptions = [
  {
    label: '5',
    value: '5',
  },

  {
    label: '10',
    value: '10',
  },

  {
    label: '20',
    value: '20',
  },

  {
    label: '50',
    value: '50',
  },

  {
    label: '100',
    value: '100',
  },
]


/*
|--------------------------------------------------------------------------
| Start Index
|--------------------------------------------------------------------------
*/

const startIndex = computed(() => {
  return (
    (page.value - 1) *
    Number(pageSize.value)
  )
})


/*
|--------------------------------------------------------------------------
| Visible Pages
|--------------------------------------------------------------------------
*/

const visiblePages = computed(() => {
  const pages = []

  const start = Math.max(
    1,
    page.value - 2
  )

  const end = Math.min(
    props.totalPages,
    start + 4
  )

  for (
    let i = start;
    i <= end;
    i++
  ) {
    pages.push(i)
  }

  return pages
})


/*
|--------------------------------------------------------------------------
| Get Value
|--------------------------------------------------------------------------
*/

const getValue = (
  item,
  key
) => {
  return (
    key
      .split('.')
      .reduce(
        (value, part) =>
          value?.[part],
        item
      ) ?? ''
  )
}


/*
|--------------------------------------------------------------------------
| Badge Color
|--------------------------------------------------------------------------
*/

const getBadgeColor = (
  value,
  column
) => {
  if (
    column.badgeColors &&
    column.badgeColors[value]
  ) {
    return column.badgeColors[value]
  }

  return 'secondary'
}


/*
|--------------------------------------------------------------------------
| Emit Table Change
|--------------------------------------------------------------------------
*/

const emitChange = () => {

  const params = {
    page: Number(page.value),

    pageSize: Number(
      pageSize.value
    ),

    sortBy: sortBy.value,

    sortDirection:
      sortDirection.value,

    ...filterValues.value,
  }

  emit(
    'change',
    params
  )
}


/*
|--------------------------------------------------------------------------
| Pagination
|--------------------------------------------------------------------------
*/

const goToPage = (
  pageNumber
) => {

  if (
    pageNumber < 1 ||
    pageNumber > props.totalPages
  ) {
    return
  }

  if (
    pageNumber === page.value
  ) {
    return
  }

  page.value = pageNumber

  emitChange()
}


/*
|--------------------------------------------------------------------------
| Page Size
|--------------------------------------------------------------------------
*/

const handlePageSizeChange = (
  event
) => {

  pageSize.value =
    String(event.target.value)

  page.value = 1

  emitChange()
}


/*
|--------------------------------------------------------------------------
| Sorting
|--------------------------------------------------------------------------
*/

const handleSort = (
  column
) => {

  if (
    sortBy.value === column
  ) {

    sortDirection.value =
      sortDirection.value === 'asc'
        ? 'desc'
        : 'asc'

  } else {

    sortBy.value = column

    sortDirection.value = 'asc'

  }

  page.value = 1

  emitChange()
}


/*
|--------------------------------------------------------------------------
| Filter
|--------------------------------------------------------------------------
*/

let searchTimer = null


const handleFilterChange = (
  filterKey
) => {

  page.value = 1


  /*
   * Search debounce
   */
  if (
    filterKey === 'search'
  ) {

    clearTimeout(
      searchTimer
    )

    searchTimer = setTimeout(() => {

      emitChange()

    }, 500)

    return
  }


  /*
   * Other filters
   */
  emitChange()
}


/*
|--------------------------------------------------------------------------
| Clear Filters
|--------------------------------------------------------------------------
*/

const clearFilters = () => {

  clearTimeout(
    searchTimer
  )

  props.filters.forEach(
    (filter) => {

      filterValues.value[
        filter.key
      ] =
        filter.defaultValue ?? ''

    }
  )

  page.value = 1

  emitChange()
}


/*
|--------------------------------------------------------------------------
| Dynamic Actions
|--------------------------------------------------------------------------
*/

const handleAction = (
  action,
  item
) => {

  emit('action', {
    action,
    item,
  })
}

onBeforeUnmount(() => {

  clearTimeout(
    searchTimer
  )

})
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