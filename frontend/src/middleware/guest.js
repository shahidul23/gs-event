export default function guestMiddleware(to, from, next) {
    const token = localStorage.getItem('access_token');
    if (token) {
        return next({
            name: 'Dashboard',
        });
    }
    next();
}