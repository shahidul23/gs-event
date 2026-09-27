// src/services/authService.js

import { post } from "../api";


export const authService = {
  /**
   * User Login
   * @param {Object} credentials - { email, password }
   */
  login(credentials) {
    return post('/auth/login', credentials);
  },

  /**
   * User Logout
   * Revokes the refresh token on ASP.NET Core backend.
   */
  logout() {
    const refreshToken = localStorage.getItem('refresh_token');
    return post('/auth/logout', { refreshToken });
  },

};