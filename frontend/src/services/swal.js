
import Swal from 'sweetalert2'

const swal = {
  // Confirmation dialog
  confirm(options = {}) {
    return Swal.fire({
      title: options.title || 'Are you sure?',
      text: options.text || 'Do you want to continue?',
      icon: options.icon || 'warning',

      showCancelButton: true,
      confirmButtonColor: options.confirmButtonColor || '#321fdb',
      cancelButtonColor: options.cancelButtonColor || '#6c757d',

      confirmButtonText: options.confirmButtonText || 'Yes, continue',
      cancelButtonText: options.cancelButtonText || 'Cancel',

      reverseButtons: true,
      focusCancel: true,

      ...options,
    }).then(result => result.isConfirmed)
  },

  // Success alert
  success(message = 'Operation completed successfully', title = 'Success!') {
    return Swal.fire({
      title,
      text: message,
      icon: 'success',
      confirmButtonColor: '#321fdb',
    })
  },

  // Error alert
  error(message = 'Something went wrong', title = 'Error!') {
    const errorMessage =
      typeof message === 'string'
        ? message
        : message?.response?.data?.message || message?.message || 'Something went wrong'

    return Swal.fire({
      title,
      text: errorMessage,
      icon: 'error',
      confirmButtonColor: '#d33',
    })
  },

  // Warning alert
  warning(message = 'Please be careful', title = 'Warning!') {
    return Swal.fire({
      title,
      text: message,
      icon: 'warning',
      confirmButtonColor: '#f0ad4e',
    })
  },

  // Information alert
  info(message = 'Here is some information', title = 'Information') {
    return Swal.fire({
      title,
      text: message,
      icon: 'info',
      confirmButtonColor: '#321fdb',
    })
  },

  // Loading dialog
  loading(message = 'Please wait...') {
    Swal.fire({
      title: message,
      allowOutsideClick: false,
      allowEscapeKey: false,
      didOpen: () => Swal.showLoading(),
    })
  },

  // Close active dialog
  close() {
    Swal.close()
  },
}

export default swal
