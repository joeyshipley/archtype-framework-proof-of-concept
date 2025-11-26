using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageInteraction : IPageInteraction {}

public class LoginPageEndpoints(
    IPageLayout _layout,
    ILoginPageView _page,
    IEnumerable<ILoginPageInteraction> _interactions
) : IClientEndpoint
{
    public const string ROUTE_BASE = "login";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ROUTE_BASE, () =>
        {
            var bodyContent = _page.RenderPage();
            var page = _layout.Render("Login", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
