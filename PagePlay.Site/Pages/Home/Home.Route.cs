using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Home;

public class HomePageEndpoints(
    IPageLayout _layout,
    IHomePageView _page,
    IFrameworkOrchestrator _framework
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var views = new IView[] { _page };
            var renderedViews = await _framework.RenderViewsAsync(views);
            var bodyContent = renderedViews[_page.ViewId];

            var page = await _layout.RenderAsync("Home", bodyContent);
            return Results.Content(page, "text/html");
        });
    }
}
