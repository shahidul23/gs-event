export default function authMiddleware(to, from) {
    const token = localStorage.getItem('access_token');

    if (!token) {
        return {
            name: 'Login',
        };
    }
    return true;
}