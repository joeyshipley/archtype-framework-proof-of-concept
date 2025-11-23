using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public interface IHtmxPage
{
    string RenderPage(string antiforgeryToken = "");
}

public static class PlumbingExplorations
{
    // Generic GET endpoint for any page that implements IHtmxPage
    public static void MapHtmxPageGet<TPageInterface>(
        this IEndpointRouteBuilder endpoints,
        string route,
        string pageTitle
    ) where TPageInterface : IHtmxPage
    {
        endpoints.MapGet(route, (
            HttpContext context,
            [FromServices] IAntiforgery antiforgery,
            [FromServices] TPageInterface page
        ) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var bodyContent = page.RenderPage(tokens.RequestToken);
            var fullPage = Layout.Render(bodyContent, pageTitle);
            return Results.Content(fullPage, "text/html");
        });
    }
}

public static class LoginEndpoints
{
    public static void MapLoginRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHtmxPageGet<ILoginPageHtmx>("/htmx/login", "Login");

        endpoints.MapPost("/htmx/api/login", async (
            [FromServices] ILoginPageHtmx page,
            [FromServices] IWorkflow<LoginRequest, LoginResponse> loginWorkflow,
            [FromForm] string email,
            [FromForm] string password
        ) =>
        {
            var request = new LoginRequest { Email = email, Password = password };
            var result = await loginWorkflow.Perform(request);

            if (!result.Success)
            {
                var errorMessage = result.Errors?.FirstOrDefault()?.Message ?? "An error occurred";
                return Results.Content(
                    page.RenderError(errorMessage),
                    "text/html"
                );
            }

            return Results.Content(
                page.RenderSuccess(result.Model.Token), 
                "text/html"
            );
        });
    }
}
