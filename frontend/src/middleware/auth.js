// src/router/authGuard.js
export default function authMiddleware(to, from) {
  const token = localStorage.getItem('access_token');
  const userJson = localStorage.getItem('user');
  const user = userJson ? JSON.parse(userJson) : null;

  // Verify JWT expiration on client side
  let isTokenExpired = false;
  if (token) {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      if (payload.exp && payload.exp < currentTime) {
        isTokenExpired = true;
      }
    } catch (e) {
      isTokenExpired = true;
    }
  }

  // Clear storage if token is expired
  if (isTokenExpired) {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
  }

  const isAuthenticated = !!token && !isTokenExpired;

  // 1. Guest-only routes (e.g. Login page)
  if (to.meta.guestOnly && isAuthenticated) {
    return { name: 'Dashboard' };
  }

  // 2. Protected routes
  if (to.meta.requiresAuth && !isAuthenticated) {
    return {
      name: 'Login',
      query: { redirect: to.fullPath },
    };
  }

  // 3. Role-based routes
  if (to.meta.roles && Array.isArray(to.meta.roles)) {
    const userRole = user?.role;
    if (!to.meta.roles.includes(userRole)) {
      return { name: 'Unauthorized' };
    }
  }

  return true;
}