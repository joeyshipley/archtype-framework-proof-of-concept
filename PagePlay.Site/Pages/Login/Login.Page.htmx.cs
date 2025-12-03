using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageView
{
    string RenderPage();
    string RenderLoginForm();
    string RenderError(string error);
    string RenderSuccess(string message);
}

public class LoginPage : ILoginPageView
{
    // language=html
    public string RenderPage() =>
    $$"""
    <div class="login-page">
        <h1>Login</h1>
        <div id="notifications"></div>
        {{RenderLoginForm()}}
    </div>
    """;

    // language=html
    public string RenderLoginForm() =>
    $$"""
    <div id="login-form" class="login-form">
        <form hx-post="/interaction/login/authenticate"
              hx-swap="none">
            <div class="login-input-group">
                <label for="email">Email</label>
                <input id="email"
                       name="email"
                       type="email"
                       placeholder="Enter email"
                       required
                       maxlength="100" />
            </div>
            <div class="login-input-group">
                <label for="password">Password</label>
                <input id="password"
                       name="password"
                       type="password"
                       placeholder="Enter password"
                       required />
            </div>
            <button type="submit">Login</button>
        </form>
    </div>
    """;

    // language=html
    public string RenderError(string error) =>
    $$"""
    <div class="error" role="alert">
        {{error.Safe()}}
    </div>
    """;

    // language=html
    public string RenderSuccess(string message) =>
    $$"""
    <div class="success" role="alert">
        {{message.Safe()}}
    </div>
    """;
}
