// src/services/authService.js
import { post } from "../api";


export const authService = {
  login(credentials) {
    return post('/login', credentials);
  },

  logout() {
    const refreshToken = localStorage.getItem('refresh_token');
    return post('/logout', { refreshToken });
  },
  passwordChange(password){
    return post('/change-password',{password});
  },
  getProfile() {
    return get('/profile');
  },
};