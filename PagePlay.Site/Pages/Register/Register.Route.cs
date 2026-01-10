using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Register;

public interface IRegisterPageInteraction : IEndpoint {}

public class RegisterPageEndpoints(
    IPageLayout _layout,
    IRegisterPageView _page,
    IFrameworkOrchestrator _framework,
    IEnumerable<IRegisterPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "register";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var views = new IView[] { _page };
            var renderedViews = await _framework.RenderViewsAsync(views);
            var bodyContent = renderedViews[_page.ViewId];

            var page = await _layout.RenderAsync("Create Account", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
