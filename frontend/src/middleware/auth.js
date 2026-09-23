export default function authMiddleware(to, from, next) {
    const token = localStorage.getItem('access_token');
    if (!token) {
        return next({
            name: 'Login',
        });
    }
    next();
}