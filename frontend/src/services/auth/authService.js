// src/services/authService.js
import { post, get } from "../api";


export const authService = {
  login(credentials) {
    return post('/login', credentials);
  },

  logout() {
    const refreshToken = localStorage.getItem('refresh_token');
    return post('/logout', { refreshToken });
  },
  passwordChange(password){
    return post('/change-password',password);
  },

  getProfile() {
    return get('/profile');
  },
  getAllRoles(){
    return get('/get-roles');
  },
  register(paylod) {
    return post('/register', paylod);
  },
  getAllUsers(params = {}) {
    return get('/get-users',params )
  }
};