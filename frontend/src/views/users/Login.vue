<script setup>
import { ref } from 'vue'
import { cilLockUnlocked, cilLockLocked } from '@coreui/icons'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import toast from '@/services/toast'
import { CButton } from '@coreui/vue'
import CIcon from '@coreui/icons-vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const loading = ref(false)
const errorMessage = ref('')
const showPassword = ref(false)

const login = async () => {
  errorMessage.value = ''
  
  if (!username.value || !password.value) {
    toast.error('Username and password are required.')
    return
  }

  loading.value = true

  try {
    // Delegate login logic to the Pinia Store
    await authStore.login({
      UsernameOrEmailOrPhone: username.value,
      password: password.value,
    })

    // Redirect to requested route (if available) or fallback to Dashboard
    const redirectPath = route.query.redirect || { name: 'Dashboard' }
    await router.push(redirectPath)
  } catch (error) {
    // If interceptor doesn't display message, show local alert fallback
    toast.error(error?.message || 'Invalid username or password.');
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

                  <CInputGroup class="mb-3">
                    
                    <CFormInput
                      v-model="username"
                      placeholder="Email or Phone"
                      autocomplete="username"
                      required
                    />
                    <CInputGroupText>
                      <CIcon icon="cil-user" />
                    </CInputGroupText>
                  </CInputGroup>

                  <CInputGroup class="mb-4">
                    <CFormInput
                      v-model="password"
                      :type="showPassword ? 'text' : 'password'"
                      placeholder="Password"
                      autocomplete="current-password"
                      required
                    />
                    <CInputGroupText type="button" color="secondary" variant="outline"
                      @click="showPassword = !showPassword">
                      <CIcon :icon="showPassword ? cilLockUnlocked : cilLockLocked"/>
                    </CInputGroupText>
                  </CInputGroup>

                  <CRow>
                    <CCol :xs="6">
                      <CButton
                        type="submit"
                        color="primary"
                        class="px-4"
                        :disabled="loading"
                      >
                        <CSpinner v-if="loading" size="sm" class="me-2" />
                        {{ loading ? 'Logging in...' : 'Login' }}
                      </CButton>
                    </CCol>
                    <CCol :xs="6" class="text-end">
                      <CButton color="link" class="px-0">
                        Forgot password?
                      </CButton>
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