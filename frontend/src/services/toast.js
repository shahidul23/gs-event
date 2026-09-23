import { toast } from 'vue3-toastify'

const defaultOptions = {
  autoClose: 3000,
}

export default {
  success(message, options = {}) {
    toast.success(message, { ...defaultOptions, ...options })
  },

  error(message, options = {}) {
    toast.error(message, { ...defaultOptions, ...options })
  },

  info(message, options = {}) {
    toast.info(message, { ...defaultOptions, ...options })
  },

  warning(message, options = {}) {
    toast.warning(message, { ...defaultOptions, ...options })
  },

  show(message, options = {}) {
    toast(message, { ...defaultOptions, ...options })
  },
}