using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Home;

public class HomePageEndpoints(
    IPageLayout _layout,
    HomePage _page
) : IClientEndpoint
{
    public const string ROUTE_BASE = "";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ROUTE_BASE, () =>
        {
            var bodyContent = _page.RenderPage();
            var page = _layout.Render("Home", bodyContent);
            return Results.Content(page, "text/html");
        });
    }
}
