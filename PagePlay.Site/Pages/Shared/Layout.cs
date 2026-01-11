using PagePlay.Site.Infrastructure.Web.Http;

namespace PagePlay.Site.Pages.Shared;

public interface IPageLayout
{
    Task<string> RenderAsync(string title, string bodyContent);
}

public class Layout(
    IAntiforgeryTokenProvider _antiforgeryTokenProvider,
    INavView _nav
) : IPageLayout
{
    // language=html
    public async Task<string> RenderAsync(string title, string bodyContent)
    {
        var antiforgeryToken = _antiforgeryTokenProvider.GetRequestToken();
        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <meta name="csrf-token" content="{{antiforgeryToken}}" />
            <title>{{title}} - TODO</title>
            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
            <script src="https://unpkg.com/idiomorph@0.3.0/dist/idiomorph-ext.min.js"></script>
            <script src="/js/component-context.js"></script>
            <script src="/js/htmx-config.js"></script>
            <link rel="stylesheet" href="/css/closed-world.css" />
        </head>
        <body hx-ext="component-context">
            {{_nav.Render()}}
            <main>
                {{bodyContent}}
            </main>
            <script src="/js/csrf-setup.js"></script>
            <script src="/js/drag-drop.js"></script>
        </body>
        </html>
        """;
    }
}
