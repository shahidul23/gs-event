<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  CButton,
  CCard,
  CCardBody,
  CCol,
  CForm,
  CFormInput,
  CFormSelect,
  CInputGroup,
  CInputGroupText,
  CRow,
} from '@coreui/vue'
import { useAuthStore } from '@/stores/auth'
import { cilLockLocked, cilLockUnlocked, cilPhone } from '@coreui/icons'

const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()

const form = ref({
  fullName: '',
  email: '',
  phone: '',
  address: '',
  password: '',
  role: '',
})

const loading = ref(false)
const loadingUser = ref(false)

const roles = ref([])

const showPassword = ref(false)

const userId = route.params.id

const getRoles = async () => {
  try {
    const res = await authStore.getAllRoles()

    if (res?.success) {
      roles.value = res.data || []
    }
  } catch (error) {
    console.error('Failed to load roles:', error)
  }
}

const getUser = async () => {
  try {
    loadingUser.value = true

    const res = await authStore.getUser(userId)
    console.log(res)

    if (res?.success) {
      const user = res.data

      form.value = {
        fullName: user.fullName || '',
        email: user.email || '',
        phone: user.phone || '',
        password: '',
        role:  '',
      }
      const userRole = roles.value.find(
        (role) => role.name.toLowerCase() === user.role.toLowerCase()
      )
      if(userRole){
        form.value.role = String(userRole.value)
      }
    }
  } catch (error) {
    console.error('Failed to load user:', error)
  } finally {
    loadingUser.value = false
  }
}

const submit = async () => {
  try {
    loading.value = true

    const payload = {
      fullName: form.value.fullName,
      email: form.value.email,
      phone: form.value.phone,
      password: form.value.password || null,
      confirmpassword: form.value.password || null,
      role: Number(form.value.role),
    }

    const res = await authStore.updateUser(userId, payload)
    if (res?.success) {
      router.push('/user/list')
    }
  } catch (error) {
    console.error('User update failed:', error)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await getRoles()
  await getUser()
})
</script>

<template>
  <CRow class="justify-content-center">
    <CCol :xl="12">
      <CCard class="mx-4">
        <CCardBody class="p-4">
          <CForm @submit.prevent="submit">
            <h4>Update User</h4>

            <p class="text-body-secondary">Update user information</p>

            <CRow>
              <CCol md="6">
                <CInputGroup class="mb-3">
                  <CInputGroupText>
                    <CIcon icon="cil-user" />
                  </CInputGroupText>

                  <CFormInput
                    v-model="form.fullName"
                    placeholder="Full Name"
                    autocomplete="name"
                    required
                  />
                </CInputGroup>
              </CCol>

              <CCol md="6">
                <CInputGroup class="mb-3">
                  <CInputGroupText> @ </CInputGroupText>

                  <CFormInput
                    v-model="form.email"
                    type="email"
                    placeholder="Email"
                    autocomplete="email"
                    required
                  />
                </CInputGroup>
              </CCol>
            </CRow>

            <CRow>
              <CCol md="6">
                <CInputGroup class="mb-3">
                  <CInputGroupText>
                    <CIcon :icon="cilPhone" />
                  </CInputGroupText>

                  <CFormInput
                    v-model="form.phone"
                    type="tel"
                    placeholder="Phone Number"
                    autocomplete="tel"
                    required
                  />
                </CInputGroup>
              </CCol>
              <CCol md="6">
                <CInputGroup class="mb-3">
                  <CInputGroupText>
                    <CIcon icon="cil-user" />
                  </CInputGroupText>

                  <CFormSelect
                    v-model="form.role"
                    :options="[
                      { label: 'Select Role', value: '' },
                      ...roles.map((role) => ({
                        label: role.name,
                        value: String(role.value),
                      })),
                    ]"
                    required
                  />
                </CInputGroup>
              </CCol>
            </CRow>

            <CInputGroup class="mb-4">
              <CInputGroupText
                type="button"
                color="secondary"
                variant="outline"
                @click="showPassword = !showPassword"
              >
                <CIcon :icon="showPassword ? cilLockUnlocked : cilLockLocked" />
              </CInputGroupText>

              <CFormInput
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="New Password (leave blank to keep current)"
                autocomplete="new-password"
              />
            </CInputGroup>

            <CCol md="2">
              <div class="d-grid">
                <CButton type="submit" color="success" :disabled="loading || loadingUser">
                  {{ loading ? 'Updating...' : 'Update User' }}
                </CButton>
              </div>
            </CCol>
          </CForm>
        </CCardBody>
      </CCard>
    </CCol>
  </CRow>
</template>
