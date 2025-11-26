using Microsoft.AspNetCore.Antiforgery;

namespace PagePlay.Site.Pages.Shared;

public interface IPageLayout
{
    string Render(HttpContext context, string title, string bodyContent);
}

public class Layout(IAntiforgery _antiforgery) : IPageLayout
{
    // language=html
    public string Render(HttpContext context, string title, string bodyContent)
    {
        var tokens = _antiforgery.GetAndStoreTokens(context);
        var antiforgeryToken = tokens.RequestToken;

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
            <link rel="stylesheet" href="/css/site.css" />
        </head>
        <body>
            <main>
                {{bodyContent}}
            </main>
            <script>
                document.body.addEventListener('htmx:configRequest', function(evt) {
                    const token = document.querySelector('meta[name="csrf-token"]')?.content;
                    if (token) {
                        evt.detail.headers['RequestVerificationToken'] = token;
                    }
                });
            </script>
        </body>
        </html>
        """;
    }
}
