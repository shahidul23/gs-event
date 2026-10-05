<template>
  <CRow>
    <CCol :xs="12">
      <CCard class="mb-4">
        <CCardHeader>
          <strong>Users</strong>
        </CCardHeader>

        <CCardBody>
          <BaseTable
            :items="users"
            :columns="columns"
            :filters="filters"
            :loading="loading"
            :total-items="totalItems"
            :total-pages="totalPages"
            row-key="id"
            default-page-size="10"
            default-sort-by="fullName"
            default-sort-direction="asc"
            @change="handleTableChange"
          />
        </CCardBody>
      </CCard>
    </CCol>
  </CRow>
</template>

<script setup>
import { ref, onMounted } from 'vue'

import BaseTable from '@/components/BaseTable.vue'

import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const users = ref([])

const loading = ref(false)

const totalItems = ref(0)

const totalPages = ref(0)

const filters = [
  {
    key: 'search',
    label: 'Search',
    type: 'text',
    placeholder: 'Search users...',
    width: '250px',
  },
]

const columns = [
  {
    key: 'fullName',
    label: 'Full Name',
    sortable: true,
  },

  {
    key: 'userName',
    label: 'Username',
    sortable: true,
  },

  {
    key: 'email',
    label: 'Email',
    sortable: true,
  },

  {
    key: 'phone',
    label: 'Phone',
    sortable: false,
  },

  {
    key: 'role',
    label: 'Role',
    sortable: true,
  },
]

const loadUsers = async (params = {}) => {
  loading.value = true

  try {
    const res = await authStore.getAllUsers(params)

    console.log('Users response:', res)

    if (res?.success) {
      users.value = res.data?.data || []

      totalItems.value = res.data?.totalItems || 0

      totalPages.value = res.data?.totalPages || 0
    }
  } catch (error) {
    console.error('Failed to load users:', error)
  } finally {
    loading.value = false
  }
}

const handleTableChange = (params) => {
  console.log('Table params:', params)

  loadUsers(params)
}

onMounted(() => {
  loadUsers({
    page: 1,
    pageSize: 10,
    sortBy: 'fullName',
    sortDirection: 'asc',
    search: '',
  })
})
</script>
