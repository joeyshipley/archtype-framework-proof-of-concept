using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.StyleTest;

public interface IStyleTestPageInteraction : IEndpoint {}

public class StyleTestPageEndpoints(
    IPageLayout _layout,
    IStyleTestPageView _page,
    IEnumerable<IStyleTestPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "style-test";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var bodyContent = _page.RenderPage();

            var page = await _layout.RenderAsync("Style Test", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
