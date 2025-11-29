// HTMX extension that sends component context with each request
htmx.defineExtension('component-context', {
    onEvent: function(name, evt) {
        if (name === 'htmx:configRequest') {
            // Find all components on page
            const components = document.querySelectorAll('[data-component]');

            // Build context array
            const context = Array.from(components).map(el => ({
                id: el.id,
                componentType: el.dataset.component,
                domain: el.dataset.domain
            }));

            // Add to request headers
            evt.detail.headers['X-Component-Context'] = JSON.stringify(context);
        }
    }
});
