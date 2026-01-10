using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.UI;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using PagePlay.Site.Pages.Home;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Todos;

namespace PagePlay.Site.Pages.Shared;

public interface INavView
{
    string Render();
}

public class Nav(IUserIdentityService _userIdentityService, IHtmlRenderer _renderer) : INavView
{
    public string Render()
    {
        var isAuthenticated = _userIdentityService.GetCurrentUserId().HasValue;

        var topNav = new TopNav()
            .Logo("TODO", $"/{HomePageEndpoints.PAGE_ROUTE}")
            .Actions(getAuthActions(isAuthenticated));

        return _renderer.Render(topNav);
    }

    private IElement[] getAuthActions(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            return new IElement[]
            {
                new Link("Todos", $"/{TodosPageEndpoints.PAGE_ROUTE}")
                    .Style(LinkStyle.Ghost)
            };
        }

        return new IElement[]
        {
            new Link("Login", $"/{LoginPageEndpoints.PAGE_ROUTE}")
                .Style(LinkStyle.Button)
        };
    }
}
