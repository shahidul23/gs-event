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
    window.location.href = '/login';
};

/*
 * Add access token to every request.
 */
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('access_token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

/*
 * Handle expired access token.
 */
api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        const originalRequest = error.config;

        /*
         * Only handle 401 responses.
         */
        if (error.response?.status !== 401) {
            return Promise.reject(error);
        }

        /*
         * Don't retry the same request.
         */
        if (originalRequest?._retry) {
            return Promise.reject(error);
        }

        /*
         * Don't refresh if the refresh endpoint itself
         * returned 401.
         */
        if (originalRequest?.url?.includes('/refresh-token')) {
            clearAuth();

            return Promise.reject(error);
        }

        /*
         * If another request is already refreshing the token,
         * wait for that request to finish.
         */
        if (isRefreshing) {
            return new Promise((resolve, reject) => {
                failedQueue.push({
                    resolve,
                    reject,
                });
            }).then((token) => {
                originalRequest.headers.Authorization =
                    `Bearer ${token}`;

                return api(originalRequest);
            });
        }
        originalRequest._retry = true;

        isRefreshing = true;

        const refreshToken =
            localStorage.getItem('refresh_token');
        const token = localStorage.getItem('access_token');

        /*
         * No refresh token means the user must log in again.
         */
        if (!refreshToken) {
            isRefreshing = false;
            clearAuth();
            return Promise.reject(error);
        }

        try {
            /*
             * Use axios directly instead of api().
             *
             * This prevents the refresh request from
             * going through the same interceptor.
             */
            const response = await axios.post(
                `${api.defaults.baseURL}/refresh-token`,
                {
                    token,
                    refreshToken,
                },
                {
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json',
                    },
                }
            );

            const data = response.data?.data;

            if (!data?.accessToken || !data?.refreshToken) {
                toast.error('Invalid refresh token response.');
            }

            /*
             * Save the new access token.
             */
            localStorage.setItem(
                'access_token',
                data.accessToken
            );

            /*
             * Save the new refresh token.
             *
             * Your backend rotates the refresh token,
             * so the old refresh token must be replaced.
             */
            localStorage.setItem(
                'refresh_token',
                data.refreshToken
            );

            /*
             * Resolve all queued requests.
             */
            processQueue(
                null,
                data.accessToken
            );

            /*
             * Retry the original request.
             */
            originalRequest.headers.Authorization =
                `Bearer ${data.accessToken}`;

            return api(originalRequest);

        } catch (refreshError) {

            /*
             * Reject all queued requests.
             */
            processQueue(
                refreshError,
                null
            );

            /*
             * Refresh token is invalid or expired.
             */
            clearAuth();

            return Promise.reject(refreshError);

        } finally {
            isRefreshing = false;
        }
    }
);

/*
 * GET
 */
export const get = (url, params = {}, config = {}) => 
{
    return api.get(url, {
        params,
        ...config,
    });
};


/*
 * POST
 */
export const post = (url, data = {}, config = {}) => 
{
    return api.post(
        url,
        data,
        config
    );
};


/*
 * PUT
 */
export const put = (url, data = {}, config = {}) =>
{
    return api.put(
        url,
        data,
        config
    );
};


/*
 * PATCH
 */
export const patch = (url,data = {},config = {}) =>
{
    return api.patch(
        url,
        data,
        config
    );
};

/*
 * DELETE
 */
export const remove = (url, config = {}) =>
{
    return api.delete(
        url,
        config
    );
};


export default api;

