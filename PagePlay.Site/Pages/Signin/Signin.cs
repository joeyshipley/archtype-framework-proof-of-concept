namespace PagePlay.Site.Pages.Signin;

public class SigninComponent
{
    // language=html
    public string RenderPage() => $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>Sign In - PagePlay</title>
            <script src="https://unpkg.com/htmx.org@1.9.10"></script>
            <script src="https://unpkg.com/idiomorph@0.3.0/dist/idiomorph-ext.min.js"></script>
        </head>
        <body hx-ext="morph">
            <main>
                {{RenderForm()}}
            </main>
        </body>
        </html>
        """;

    // language=html
    public string RenderForm(string? error = null) => $$"""
        <div class="signin-form">
            <h1>Sign In</h1>
            {{(error != null ? $"""<div class="error" role="alert">{error}</div>""" : "")}}
            <form hx-post="/api/signin"
                  hx-target="#signin-container"
                  hx-swap="innerHTML">
                <div>
                    <label for="email">Email</label>
                    <input id="email" name="email" type="email" required />
                </div>
                <div>
                    <label for="password">Password</label>
                    <input id="password" name="password" type="password" required />
                </div>
                <button type="submit">Sign In</button>
            </form>
            <div id="signin-container"></div>
        </div>
        """;

    // language=html
    public string RenderError(string error) => $$"""
        <div class="error" role="alert">
            {{error}}
        </div>
        """;

    // language=html
    public string RenderSuccess(string token) => $$"""
        <div class="success">
            <h2>Success</h2>
            <p>Token: {{token}}</p>
        </div>
        """;
}
