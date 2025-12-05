using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageInteraction : IEndpoint {}

public class LoginPageEndpoints(
    IPageLayout _layout,
    LoginPage _page,
    IEnumerable<ILoginPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "login";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            var ctx = DataContext.Empty();
            var bodyContent = ((IServerComponent)_page).Render(ctx);

            // Layout handles its own component composition
            var page = await _layout.RenderAsync("Login", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
