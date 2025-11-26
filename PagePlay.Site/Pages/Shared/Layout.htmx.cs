using Microsoft.AspNetCore.Antiforgery;

namespace PagePlay.Site.Pages.Shared;

public interface IPageLayout
{
    string Render(string title, string bodyContent);
}

public class Layout : IPageLayout
{
    public readonly string antiforgeryToken;
    
    public Layout(
        HttpContext context,        
        IAntiforgery antiforgery
    )
    {
        var tokens = antiforgery.GetAndStoreTokens(context);
        antiforgeryToken = tokens.RequestToken;
    }
    
    // language=html
    public string Render(string bodyContent, string title) =>
    $$"""
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
