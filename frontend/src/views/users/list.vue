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
            :default-page-size="10"
            default-sort-by="fullName"
            default-sort-direction="asc"
            @change="handleTableChange"
            @action="handleAction"
          />
        </CCardBody>
      </CCard>
    </CCol>
  </CRow>
</template>


<script setup>
import {
  ref,
  onMounted,
} from 'vue'

import BaseTable from '@/components/BaseTable.vue'

import {
  cilZoomIn,
  cilPencil,
  cilTrash,
} from '@coreui/icons'

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
    sortable: true,
  },

  {
    key: 'role',
    label: 'Role',
    sortable: false,
  },

  // {
  //   key: 'status',
  //   label: 'Status',
  //   type: 'badge',
  //   sortable: true,

  //   badgeColors: {
  //     Active: 'success',
  //     Inactive: 'danger',
  //   },
  // },

  {
    key: 'actions',
    label: 'Action',
    type: 'actions',

    actions: [
      // {
      //   name: 'view',
      //   label: 'View',
      //   color: 'info',
      //   icon: cilZoomIn,
      // },

      {
        name: 'edit',
        label: 'Edit',
        color: 'primary',
        icon: cilPencil,
      },

      {
        name: 'delete',
        label: 'Delete',
        color: 'danger',
        icon: cilTrash,
      },
    ],
  },
]


let requestId = 0

const loadUsers = async (params = {}) => {
  const currentRequestId = ++requestId

  loading.value = true

  try {
    const res = await authStore.getAllUsers(params)

    /*
     * Ignore old API response
     */
    if (currentRequestId !== requestId) {
      return
    }

    if (res?.success) {
      users.value = res.data?.data || []

      totalItems.value =
        res.data?.totalItems || 0

      totalPages.value =
        res.data?.totalPages || 0
    }
  } catch (error) {
    /*
     * Ignore error from old request
     */
    if (currentRequestId !== requestId) {
      return
    }

    console.error(
      'Failed to load users:',
      error
    )
  } finally {
    /*
     * Only latest request controls loading
     */
    if (currentRequestId === requestId) {
      loading.value = false
    }
  }
}

const handleTableChange = (params) => {
  console.log(
    'Table params:',
    params
  )

  loadUsers(params)
}


const handleAction = ({
  action,
  item,
}) => {

  switch (action) {

    case 'view':
      handleView(item)
      break

    case 'edit':
      handleEdit(item)
      break

    case 'delete':
      handleDelete(item)
      break
  }
}

const handleView = (user) => {
  console.log(
    'View user:',
    user
  )

  // Example:
  // router.push(`/users/${user.id}`)
}


const handleEdit = (user) => {
  console.log(
    'Edit user:',
    user
  )

  // Example:
  // router.push(`/users/${user.id}/edit`)
}

const handleDelete = (user) => {
  console.log(
    'Delete user:',
    user
  )

  // Confirmation modal
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