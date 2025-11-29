using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public interface ILoginPageInteraction : IEndpoint {}

public class LoginPageEndpoints(
    IPageLayout _layout,
    ILoginPageView _page,
    IEnumerable<ILoginPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "login";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, () =>
        {
            var bodyContent = _page.RenderPage();
            var page = _layout.Render("Login", bodyContent);
            return Results.Content(page, "text/html");
        });

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
