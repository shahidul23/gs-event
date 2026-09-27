// src/stores/auth.js
import { defineStore } from 'pinia';
import toast from '@/services/toast';
import { authService } from '@/services/auth/authService';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('access_token') || null,
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
        console.warn('Backend revocation failed or token already revoked:', error);
      } finally {
        this.user = null;
        this.token = null;

        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('user');

        window.location.href = '/login';
      }
    },
  },
});