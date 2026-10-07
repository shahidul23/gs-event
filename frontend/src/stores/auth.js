// src/stores/auth.js
import { defineStore } from 'pinia';
import toast from '@/services/toast';
import { authService } from '@/services/auth/authService';


export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('access_token') || null,
    roles: [],
    users: [],
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    async login(credentials) {
      try {
        const res = await authService.login(credentials);

        if (res.success) {
          this.token = res.data.token;
          this.user = res.data.user;

          localStorage.setItem('access_token', res.data.token);
          localStorage.setItem('refresh_token', res.data.refreshToken);
          localStorage.setItem('user', JSON.stringify(res.data.user));

          toast.success(res.message || 'Login successful');
          return res;
        }
      } catch (error) {
        throw error;
      }
    },

    async logout() {
      try {
        const res = await authService.logout();
        if (res.success) {
          toast.success(res.message || 'Logout successful');
        }
      } catch (error) {
        console.warn('Backend logout failed or token already revoked:', error);
      } finally {
        this.user = null;
        this.token = null;

        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('user');

        window.location.href = '/login';
      }
    },
    async changePassword(passwordData) {
      try {
        const res = await authService.passwordChange(passwordData);
        if (res?.success) {
          toast.success(res.message || 'Password changed successfully');
          return res;
        }
      } catch (error) {
        toast.error(error?.message || 'Failed to change password');
        throw error;
      }
    },
    async getAllRoles() {
      try {
        const res = await authService.getAllRoles();
        if (res?.success) {
          this.roles = res.data || [];

          toast.success(res.message || 'Roles retrieved successfully');
        }
        return res;
      } catch (error) {
        toast.error(error?.message || 'Failed to get roles');
        throw error;
      }
    },
    async userRegister(payload) {
      try {
        const res = await authService.register(payload)

        if (res?.success) {
          toast.success(res.message || 'User registered successfully')
        }
        return res
      } catch (error) {
        toast.error(error?.message || 'Failed to register user')
        throw error
      }
    },
    async getAllUsers(params = {}) {
      try {
        const res = await authService.getAllUsers(params)
        if (res?.success) {
          this.users = res.data?.data || []
          toast.success(res.message || 'Users retrieved successfully')
        }
        return res
      } catch (error) {
        toast.error(error?.message || 'Failed to get users')
        throw error
      }
    },
    async getUser(id){
      try {
        const response = await authService.getUser(id);
        return response;
      } catch (error) {
        toast.error(error?.message || 'Failed to get user:', error)
      }
    },
    async updateUser(id, payload){
      try {
        const response = await authService.updateUser(id, payload)
        toast.success(response.message || 'Users update successfully')
        return response.data
      } catch (error) {
        toast.error('Failed to update user:', error)
      }
    }
  },
});