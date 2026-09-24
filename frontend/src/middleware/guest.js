export default function guestMiddleware(to, from) {
    const token = localStorage.getItem('access_token');
    if (token) {
        return {
            name: 'Dashboard',
        };
    }

    return true;
}