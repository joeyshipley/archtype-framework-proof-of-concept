using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageInteraction : IEndpoint {}

public class LoginPageEndpoints(
    IPageLayout _layout,
    LoginPage _page,
    IFrameworkOrchestrator _framework,
    IEnumerable<ILoginPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "login";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            // Framework handles data loading and metadata injection
            var components = new IServerComponent[] { _page };
            var renderedComponents = await _framework.RenderComponentsAsync(components);
            var bodyContent = renderedComponents[_page.ComponentId];

            var page = await _layout.RenderAsync("Login", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
