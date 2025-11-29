using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Home;

public class HomePageEndpoints(
    IPageLayout _layout,
    IHomePageView _page,
    IWelcomeWidget _welcomeWidget,
    IFrameworkOrchestrator _framework,
    IUserIdentityService _userIdentity
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var bodyContent = _page.RenderPage();

            // Render welcome widget based on authentication status
            string welcomeHtml;
            if (_userIdentity.GetCurrentUserId().HasValue)
            {
                // Authenticated: render with todo data
                var components = new[] { _welcomeWidget };
                var renderedComponents = await _framework.RenderComponentsAsync(components);
                welcomeHtml = renderedComponents[_welcomeWidget.ComponentId];
            }
            else
            {
                // Not authenticated: render simple welcome message
                welcomeHtml = _welcomeWidget.RenderUnauthenticated();
            }

            var page = _layout.Render("Home", bodyContent, welcomeHtml);
            return Results.Content(page, "text/html");
        });
    }
}
