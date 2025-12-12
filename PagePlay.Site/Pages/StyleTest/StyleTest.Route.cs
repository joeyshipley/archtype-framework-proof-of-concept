using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.StyleTest;

public interface IStyleTestPageInteraction : IEndpoint {}

public class StyleTestPageEndpoints(
    IPageLayout _layout,
    StyleTestPage _page,
    IFrameworkOrchestrator _framework,
    IEnumerable<IStyleTestPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "style-test";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var views = new IView[] { _page };
            var renderedViews = await _framework.RenderViewsAsync(views);
            var bodyContent = renderedViews[_page.ViewId];

            var page = await _layout.RenderAsync("Style Test", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
