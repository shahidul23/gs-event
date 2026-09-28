<script setup>
import { computed, ref } from 'vue'
import { jwtDecode } from 'jwt-decode'
import { useAuthStore } from '@/stores/auth'
import toast from '@/services/toast' 

const authStore = useAuthStore()
// Modal Visibility State
const showPasswordModal = ref(false)
const oldPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const loading = ref(false)

// Dynamically decode token
const user = computed(() => {
  const token = authStore.token || localStorage.getItem('access_token')
  if (!token) return null

  try {
    return jwtDecode(token)
  } catch (error) {
    toast.error('Invalid JWT token')
    return null
  }
})

const fullName = computed(() => {
  return user.value?.fullName || user.value?.nameid || 'User'
})

const initials = computed(() => {
  const name = fullName.value
  if (!name) return 'U'
  return name
    .trim()
    .split(/\s+/)
    .map((word) => word.charAt(0))
    .join('')
    .substring(0, 2)
    .toUpperCase()
})

const avatarColors = [
  '#0d6efd',
  '#6610f2',
  '#6f42c1',
  '#d63384',
  '#dc3545',
  '#fd7e14',
  '#198754',
  '#20c997',
  '#0dcaf0',
]

const avatarColor = computed(() => {
  let hash = 0
  for (let i = 0; i < fullName.value.length; i++) {
    hash = fullName.value.charCodeAt(i) + ((hash << 5) - hash)
  }
  const index = Math.abs(hash) % avatarColors.length
  return avatarColors[index]
})

// Delegate logout action to Auth Store
const handleLogout = async () => {
  await authStore.logout()
}
const goToPasswordChange = () => {
  showPasswordModal.value = true
}
const handlePasswordChange = async () => {
  if (newPassword.value !== confirmPassword.value) {
    toast.error('New passwords do not match')
    return
  }

  loading.value = true
  try {
    await authStore.changePassword({
      OldPassword: oldPassword.value,
      NewPassword: newPassword.value,
      ConfirmPassword:confirmPassword.value
    });
    // Reset inputs on success
    oldPassword.value = '';
    newPassword.value = '';
    confirmPassword.value = '';
  } catch (error) {
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <CDropdown placement="bottom-end" variant="nav-item">
    <CDropdownToggle class="py-0 pe-0" :caret="false">
      <CAvatar
        size="md"
        class="rounded-circle text-white fw-semibold"
        :style="{ backgroundColor: avatarColor }"
      >
        {{ initials }}
      </CAvatar>
    </CDropdownToggle>
    <CDropdownMenu class="pt-0">
      <CDropdownHeader
        component="h6"
        class="bg-body-secondary text-body-secondary fw-semibold my-2"
      >
        {{ fullName }}
      </CDropdownHeader>
      <CDropdownDivider />
      <CDropdownItem @click="goToPasswordChange" style="cursor: pointer">
        <CIcon icon="cil-shield-alt" /> Password change
      </CDropdownItem>
      <CDropdownItem @click="handleLogout" style="cursor: pointer">
        <CIcon icon="cil-lock-locked" /> Logout
      </CDropdownItem>
    </CDropdownMenu>
  </CDropdown>
  <!-- Change Password Modal -->
  <CModal :visible="showPasswordModal" @close="showPasswordModal = false">
    <CModalHeader>
      <CModalTitle>Change Password</CModalTitle>
    </CModalHeader>
    <CModalBody>
      <CForm @submit.prevent="handlePasswordChange">
        <div class="mb-3">
          <CFormLabel>Current Password</CFormLabel>
          <CFormInput v-model="oldPassword" type="password" required />
        </div>
        <div class="mb-3">
          <CFormLabel>New Password</CFormLabel>
          <CFormInput v-model="newPassword" type="password" required />
        </div>
        <div class="mb-3">
          <CFormLabel>Confirm New Password</CFormLabel>
          <CFormInput v-model="confirmPassword" type="password" required />
        </div>
        <CModalFooter>
          <CButton color="secondary" @click="showPasswordModal = false">Cancel</CButton>
          <CButton color="primary" type="submit" :disabled="loading">
            <CSpinner v-if="loading" size="sm" class="me-2" />
            Save Changes
          </CButton>
        </CModalFooter>
      </CForm>
    </CModalBody>
  </CModal>
</template>
