// src/services/authService.js
import { post, get, put } from "../api";


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
  getAllRoles(){
    return get('/get-roles');
  },
  register(paylod) {
    return post('/register', paylod);
  },
  getAllUsers(params = {}) {
    return get('/get-users',params )
  },
  getUser(id) {
    return get(`/get-user/${id}`)
  },
  updateUser(id, payload){
    return put(`/user-update/${id}`, payload)
  },
};