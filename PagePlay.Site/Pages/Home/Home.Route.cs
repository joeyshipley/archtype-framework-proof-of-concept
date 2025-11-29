using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Home;

public class HomePageEndpoints(
    IPageLayout _layout,
    IHomePageView _page
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, () =>
        {
            var bodyContent = _page.RenderPage();
            var page = _layout.Render("Home", bodyContent);
            return Results.Content(page, "text/html");
        });
    }
}
