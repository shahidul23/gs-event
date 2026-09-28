// src/router/authGuard.js
import axios from 'axios';

const isJwtExpired = (token) => {
  if (!token) return true;
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const currentTime = Math.floor(Date.now() / 1000);
    return payload.exp && payload.exp < currentTime;
  } catch (e) {
    return true;
  }
};

const clearAuthStorage = () => {
  localStorage.removeItem('access_token');
  localStorage.removeItem('refresh_token');
  localStorage.removeItem('user');
};

export default async function authMiddleware(to, from) {
  let token = localStorage.getItem('access_token');
  const refreshToken = localStorage.getItem('refresh_token');
  const userJson = localStorage.getItem('user');
  const user = userJson ? JSON.parse(userJson) : null;
  const baseURL = import.meta.env.VITE_API_URL || 'http://localhost:5071/api';

  // Handle Token Refresh during route changes if access token is expired
  if (token && isJwtExpired(token)) {
    if (refreshToken) {
      try {
        const response = await axios.post(
          `${baseURL}/refresh-token`,
          { token, refreshToken },
          { headers: { 'Content-Type': 'application/json' } }
        );

        const resData = response.data?.data;

        if (resData?.token && resData?.refreshToken) {
          token = resData.token;
          localStorage.setItem('access_token', resData.token);
          localStorage.setItem('refresh_token', resData.refreshToken);
        } else {
          throw new Error('Invalid refresh token payload');
        }
      } catch (error) {
        clearAuthStorage();
        token = null;
      }
    } else {
      clearAuthStorage();
      token = null;
    }
  }

  const isAuthenticated = !!token && !isJwtExpired(token);

  // Route Guards
  if (to.meta.guestOnly && isAuthenticated) {
    return { name: 'Dashboard' };
  }

  if (to.meta.requiresAuth && !isAuthenticated) {
    return {
      name: 'Login',
      query: { redirect: to.fullPath },
    };
  }

  if (to.meta.roles && Array.isArray(to.meta.roles)) {
    const userRole = user?.role;
    if (!to.meta.roles.includes(userRole)) {
      return { name: 'Unauthorized' };
    }
  }

  return true;
}