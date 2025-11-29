using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Pages.Home;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Todos;

namespace PagePlay.Site.Pages.Shared;

public interface INavView
{
    string Render();
}

public class Nav(IUserIdentityService _userIdentityService) : INavView
{
    // language=html
    public string Render()
    {
        var isAuthenticated = _userIdentityService.GetCurrentUserId().HasValue;

        return $$"""
        <nav class="main-nav">
            <div class="nav-brand">
                <a href="/{{HomePageEndpoints.PAGE_ROUTE}}">Page Play</a>
            </div>
            <div class="nav-links">
                {{renderAuthLinks(isAuthenticated)}}
            </div>
        </nav>
        """;
    }

    // language=html
    private string renderAuthLinks(bool isAuthenticated) =>
        isAuthenticated
            ? $$"""<a href="/{{TodosPageEndpoints.PAGE_ROUTE}}">Todos</a>"""
            : $$"""<a href="/{{LoginPageEndpoints.PAGE_ROUTE}}">Login</a>""";
}
