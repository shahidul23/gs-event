<script setup>
import { onMounted, ref } from 'vue'
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
const form = ref({
  fullname: '',
  email: '',
  phone: '',
  address: '',
  password: '',
  confirmPassword: '',
  role: '',
})
const loading = ref(false)
const roles = ref([])
const showPassword = ref(false)
const showConfirmPassword = ref(false);

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
const submit = async () => {
  if (form.value.password !== form.value.confirmPassword) {
    return
  }

  try {
    loading.value = true

    const payload = {
      fullname: form.value.fullname,
      email: form.value.email,
      phone: form.value.phone,
      address: form.value.address,
      password: form.value.password,
      confirmPassword: form.value.confirmPassword,
      role: Number(form.value.role),
    }
    const res = await authStore.userRegister(payload)

     if (res?.success) {
      form.value = {
        fullname: '',
        email: '',
        phone: '',
        address: '',
        password: '',
        confirmPassword: '',
        role: '',
      }
    }
  } catch (error) {
    console.error('Registration failed:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  getRoles()
})
</script>

<template>
  <CRow class="justify-content-center">
    <CCol :xl="12">
      <CCard class="mx-4">
        <CCardBody class="p-4">

          <CForm @submit.prevent="submit">

            <h4>Register</h4>

            <p class="text-body-secondary">
              Create your account
            </p>

            <!-- Full Name / Email -->
            <CRow>
              <CCol md="6">
                <CInputGroup class="mb-3">
                  <CInputGroupText>
                    <CIcon icon="cil-user" />
                  </CInputGroupText>

                  <CFormInput
                    v-model="form.fullname"
                    placeholder="Full Name"
                    autocomplete="name"
                    required
                  />
                </CInputGroup>
              </CCol>

              <CCol md="6">
                <CInputGroup class="mb-3">
                  <CInputGroupText>@</CInputGroupText>

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

            <!-- Phone / Role -->
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
                      ...roles.map(role => ({
                        label: role.name,
                        value: role.value,
                      })),
                    ]"
                    required
                  />
                </CInputGroup>
              </CCol>
            </CRow>

            <!-- Address -->
            <CInputGroup class="mb-3">
              <CInputGroupText>
                <CIcon icon="cil-location-pin" />
              </CInputGroupText>

              <CFormInput
                v-model="form.address"
                placeholder="Address"
                autocomplete="street-address"
                required
              />
            </CInputGroup>

            <!-- Password / Confirm Password -->
            <CRow>
              <CCol md="6">
                <CInputGroup class="mb-4">
                  <CInputGroupText type="button" color="secondary" variant="outline"
                    @click="showPassword = !showPassword">
                    <CIcon :icon="showPassword ? cilLockUnlocked : cilLockLocked"/>
                  </CInputGroupText>

                  <CFormInput
                    v-model="form.password"
                    :type="showPassword? 'text' : 'password'"
                    placeholder="Password"
                    autocomplete="new-password"
                    required
                  />
                </CInputGroup>
              </CCol>

              <CCol md="6">
                <CInputGroup class="mb-4">
                 <CInputGroupText type="button" color="secondary" variant="outline"
                    @click="showConfirmPassword = !showConfirmPassword">
                    <CIcon :icon="showConfirmPassword ? cilLockUnlocked : cilLockLocked"/>
                  </CInputGroupText>

                  <CFormInput
                    v-model="form.confirmPassword"
                    :type="showConfirmPassword ? 'text' : 'password'"
                    placeholder="Repeat password"
                    autocomplete="new-password"
                    required
                  />
                </CInputGroup>
              </CCol>
            </CRow>

            <!-- Submit -->
            <CCol md="2">
              <div class="d-grid">
                <CButton
                  type="submit"
                  color="success"
                  :disabled="loading"
                >
                  {{ loading ? 'Creating...' : 'Create Account' }}
                </CButton>
              </div>
            </CCol>

          </CForm>

        </CCardBody>
      </CCard>
    </CCol>
  </CRow>
</template>

