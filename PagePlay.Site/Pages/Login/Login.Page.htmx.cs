using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageHtmx :
    IHtmxPage<LoginPageData>,
    IHtmxFragment<LoginResponse>
{}

public static class LoginEndpoints
{
    public static void MapLoginRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHtmxPage<ILoginPageHtmx, LoginPageData>("/htmx/login", "Login");
        endpoints.MapHtmxFragment<ILoginPageHtmx, LoginRequest, LoginResponse>("/htmx/api/login");
    }
}

public record LoginPageData(string UserEmail);

public class LoginPageDataLoader(IUserRepository _userRepository) 
    : IPageDataLoader<LoginPageData>
{
    public async Task<LoginPageData> Load()
    {
        var user = await _userRepository.Get(User.ById(1));
        return new LoginPageData(user.Email);
    }
}

public class LoginPage : ILoginPageHtmx
{
    // language=html
    public string RenderPage(string antiforgeryToken, LoginPageData data) =>
    $$"""
    <div>
        {{RenderForm(antiforgeryToken, data)}}
    </div>
    """;

    // language=html
    public string RenderForm(string antiforgeryToken, LoginPageData data, string? error = null) =>
    $$"""
    <div class="login-form">
        <h1>Login</h1>
        <p>Preloaded user email: {{data.UserEmail}}</p>
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
    public string RenderError(IEnumerable<ResponseErrorEntry> errors) =>
    $$"""
    <div class="errors">
        {{string.Join("", errors.Select(e =>
            $"""<div class="error" role="alert">{e.Message}</div>"""
        ))}}
    </div>
    """;

    // language=html
    public string RenderSuccess(LoginResponse model) =>
    $$"""
    <div class="success">
        <h2>Success</h2>
        <p>Token: {{model.Token}}</p>
        <p>User ID: {{model.UserId}}</p>
    </div>
    """;
}
