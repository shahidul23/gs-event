// src/services/api.js
import axios from 'axios';
import toast from './toast';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5071/api',
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error);
    } else {
      promise.resolve(token);
    }
  });
  failedQueue = [];
};

const clearAuth = () => {
  localStorage.removeItem('access_token');
  localStorage.removeItem('refresh_token');
  localStorage.removeItem('user');
  window.location.href = '/login';
};

// Request Interceptor
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('access_token');
    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor
api.interceptors.response.use(
  (response) => response.data,
  async (error) => {
    const originalRequest = error.config;

    if (!error.response) {
      toast.error('Network error. Please check your connection.');
      return Promise.reject(error);
    }

    const { status, data } = error.response;
    const errorCode = data?.code;

    if (status !== 401) {
      const errorMessage = data?.message || 'An unexpected error occurred.';
      toast.error(errorMessage);
      return Promise.reject(error.response?.data || error);
    }

    if (originalRequest?.url?.includes('/refresh-token')) {
      processQueue(error, null);
      clearAuth();
      return Promise.reject(error);
    }

    if (errorCode === 'INVALID_ACCESS_TOKEN' || errorCode === 'ACCESS_TOKEN_REVOKED') {
      clearAuth();
      return Promise.reject(error);
    }

    if (originalRequest?._retry) {
      return Promise.reject(error);
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject });
      })
        .then((newToken) => {
          originalRequest.headers = originalRequest.headers || {};
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return api(originalRequest);
        })
        .catch((err) => Promise.reject(err));
    }

    originalRequest._retry = true;
    isRefreshing = true;

    const accessToken = localStorage.getItem('access_token');
    const refreshToken = localStorage.getItem('refresh_token');

    if (!refreshToken) {
      isRefreshing = false;
      clearAuth();
      return Promise.reject(error);
    }

    try {
      const response = await axios.post(
        `${api.defaults.baseURL}/refresh-token`,
        { token: accessToken, refreshToken },
        {
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
          },
        }
      );

      const resData = response.data?.data;

      if (!resData?.token || !resData?.refreshToken) {
        throw new Error('Invalid refresh response');
      }

      localStorage.setItem('access_token', resData.token);
      localStorage.setItem('refresh_token', resData.refreshToken);

      processQueue(null, resData.token);

      originalRequest.headers = originalRequest.headers || {};
      originalRequest.headers.Authorization = `Bearer ${resData.token}`;

      return api(originalRequest);
    } catch (refreshError) {
      processQueue(refreshError, null);
      toast.error('Session expired. Please log in again.');
      clearAuth();
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  }
);

export const get = (url, params = {}, config = {}) => api.get(url, { params, ...config });
export const post = (url, data = {}, config = {}) => api.post(url, data, config);
export const put = (url, data = {}, config = {}) => api.put(url, data, config);
export const patch = (url, data = {}, config = {}) => api.patch(url, data, config);
export const remove = (url, config = {}) => api.delete(url, config);

export default api;