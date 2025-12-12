// HTMX extension that sends view context with each request
htmx.defineExtension('component-context', {
    onEvent: function(name, evt) {
        if (name === 'htmx:configRequest') {
            // Find all views on page
            const views = document.querySelectorAll('[data-view]');

            // Build context array
            const context = Array.from(views).map(el => ({
                id: el.id,
                viewType: el.dataset.view,
                domain: el.dataset.domain
            }));

            // Add to request headers
            evt.detail.headers['X-Component-Context'] = JSON.stringify(context);
        }
    }
});
