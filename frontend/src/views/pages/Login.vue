<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { post } from '@/services/api'
import toast from '@/services/toast'
const router = useRouter()
const username = ref('')
const password = ref('')
const loading = ref(false)
const errorMessage = ref('')
const login = async () => {
  errorMessage.value = ''
  if (!username.value || !password.value) {
    errorMessage.value = 'Username and password are required.'
    return
  }
  loading.value = true
  try {
    const response = await post('/auth/login', {
      UsernameOrEmailOrPhone: username.value,
      password: password.value,
    })
    console.log(response);
    const token = response.data?.data?.accessToken
    if (!token) {
      toast('Login failed. Access token not found.');
      return
    }
    localStorage.setItem('access_token', token)
    await router.push({ name: 'Dashboard' })
  } catch (error) {
    toast.error(error.response?.data?.message || 'Invalid username or password.')
  } finally {
    loading.value = false
  }
}
</script>
<template>
  <div class="wrapper min-vh-100 d-flex flex-row align-items-center">
    <CContainer>
      <CRow class="justify-content-center">
        <CCol :md="4">
          <CCardGroup>
            <CCard class="p-4">
              <CCardBody>
                <CForm @submit.prevent="login">
                  <h1>Login</h1>
                  <p class="text-body-secondary">Sign In to your account</p>
                  <CAlert v-if="errorMessage" color="danger" class="mb-3">
                    {{ errorMessage }}
                  </CAlert>
                  <CInputGroup class="mb-3">
                    <CInputGroupText> <CIcon icon="cil-user" /> </CInputGroupText>
                    <CFormInput v-model="username" placeholder="Username" autocomplete="username" />
                  </CInputGroup>
                  <CInputGroup class="mb-4">
                    <CInputGroupText> <CIcon icon="cil-lock-locked" /> </CInputGroupText>
                    <CFormInput
                      v-model="password"
                      type="password"
                      placeholder="Password"
                      autocomplete="current-password"
                    />
                  </CInputGroup>
                  <CRow>
                    <CCol :xs="4">
                      <CButton type="submit" color="primary" class="px-4" :disabled="loading">
                        <CSpinner v-if="loading" size="sm" class="me-2" />
                        {{ loading ? 'Logging in...' : 'Login' }}
                      </CButton>
                    </CCol>
                    <CCol :xs="6" class="text-right">
                      <CButton color="link" class="px-0"> Forgot password? </CButton>
                    </CCol>
                  </CRow>
                </CForm>
              </CCardBody>
            </CCard>
          </CCardGroup>
        </CCol>
      </CRow>
    </CContainer>
  </div>
</template>
