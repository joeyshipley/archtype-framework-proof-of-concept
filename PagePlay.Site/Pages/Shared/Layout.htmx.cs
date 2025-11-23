namespace PagePlay.Site.Pages.Shared;

public static class Layout
{
    // language=html
    public static string Render(string bodyContent, string title) =>
    $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>{{title}} - PagePlay</title>
        <script src="https://unpkg.com/htmx.org@1.9.10"></script>
        <script src="https://unpkg.com/idiomorph@0.3.0/dist/idiomorph-ext.min.js"></script>
        <link rel="stylesheet" href="/css/site.css" />
    </head>
    <body>
        <main>
            {{bodyContent}}
        </main>
    </body>
    </html>
    """;
}
