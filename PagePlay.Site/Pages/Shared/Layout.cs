using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Http;

namespace PagePlay.Site.Pages.Shared;

public interface IPageLayout
{
    Task<string> RenderAsync(string title, string bodyContent);
}

public class Layout(
    IAntiforgeryTokenProvider _antiforgeryTokenProvider,
    INavView _nav,
    IWelcomeWidget _welcomeWidget,
    IFrameworkOrchestrator _framework,
    IUserIdentityService _userIdentity
) : IPageLayout
{
    // language=html
    public async Task<string> RenderAsync(string title, string bodyContent)
    {
        var antiforgeryToken = _antiforgeryTokenProvider.GetRequestToken();

        // Layout handles its own component composition
        var welcomeHtml = await renderWelcomeWidget();

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <meta name="csrf-token" content="{{antiforgeryToken}}" />
            <title>{{title}} - PagePlay</title>
            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
            <script src="https://unpkg.com/idiomorph@0.3.0/dist/idiomorph-ext.min.js"></script>
            <script src="/js/component-context.js"></script>
            <script src="/js/htmx-config.js"></script>
            <link rel="stylesheet" href="/css/closed-world.css" />
        </head>
        <body hx-ext="component-context">
            {{_nav.Render()}}
            {{welcomeHtml}}
            <main>
                {{bodyContent}}
            </main>
            <script src="/js/csrf-setup.js"></script>
        </body>
        </html>
        """;
    }

    private async Task<string> renderWelcomeWidget()
    {
        // Render welcome widget based on authentication status
        if (_userIdentity.GetCurrentUserId().HasValue)
        {
            // Authenticated: render with domain data
            var views = new IView[] { _welcomeWidget };
            var renderedViews = await _framework.RenderViewsAsync(views);
            return renderedViews[_welcomeWidget.ViewId];
        }

        // Not authenticated: render simple welcome message
        return _welcomeWidget.RenderUnauthenticated();
    }
}
