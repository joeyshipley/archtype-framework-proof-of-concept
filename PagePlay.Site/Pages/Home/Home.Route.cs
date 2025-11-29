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
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var bodyContent = _page.RenderPage();

            // Layout handles its own component composition
            var page = await _layout.RenderAsync("Home", bodyContent);
            return Results.Content(page, "text/html");
        });
    }
}
