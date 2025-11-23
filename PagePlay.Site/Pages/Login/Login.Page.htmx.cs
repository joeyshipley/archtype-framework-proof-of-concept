namespace PagePlay.Site.Pages.Login;

public interface ILoginPageHtmx
{
    string RenderPage(string antiforgeryToken = "");
    string RenderError(string error);
    string RenderSuccess(string token);
}

public class LoginPage : ILoginPageHtmx
{
    // language=html
    public string RenderPage(string antiforgeryToken = "") =>
    $$"""
    <div>
        {{RenderForm(antiforgeryToken)}}
    </div>
    """;

    // language=html
    public string RenderForm(string antiforgeryToken, string? error = null) =>
    $$"""
    <div class="login-form">
        <h1>Login</h1>
        {{(error != null ? $"""<div class="error" role="alert">{error}</div>""" : "")}}
        <form hx-post="/htmx/api/login"
              hx-target="#login-container"
              hx-swap="innerHTML">
            <input type="hidden" name="__RequestVerificationToken" value="{{antiforgeryToken}}" />
            <div>
                <label for="email">Email</label>
                <input id="email" name="email" type="email" required />
            </div>
            <div>
                <label for="password">Password</label>
                <input id="password" name="password" type="password" required />
            </div>
            <button type="submit">Login</button>
        </form>
        <div id="login-container"></div>
    </div>
    """;

    // language=html
    public string RenderError(string error) =>
    $$"""
    <div class="error" role="alert">
        {{error}}
    </div>
    """;

    // language=html
    public string RenderSuccess(string token) =>
    $$"""
    <div class="success">
        <h2>Success</h2>
        <p>Token: {{token}}</p>
    </div>
    """;
}
