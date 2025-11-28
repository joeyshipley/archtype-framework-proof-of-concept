// CSRF Token Configuration for HTMX
// This script automatically adds the CSRF token to all HTMX requests.
// The token is injected into a meta tag by the server-side Layout and read here.

document.body.addEventListener('htmx:configRequest', function(evt) {
    const token = document.querySelector('meta[name="csrf-token"]')?.content;
    if (token) {
        evt.detail.headers['X-XSRF-TOKEN'] = token;
    }
});
