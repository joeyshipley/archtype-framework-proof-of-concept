using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Login;

public static class LoginEndpoints
{
    public static void MapLoginRoutes(this IEndpointRouteBuilder endpoints)
    {
        var page = new LoginPage();

        endpoints.MapGet("/htmx/login", (IAntiforgery antiforgery, HttpContext context) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var bodyContent = page.RenderPage(tokens.RequestToken!);
            var fullPage = Layout.Render(bodyContent, "Login");
            return Results.Content(fullPage, "text/html");
        });

        endpoints.MapPost("/htmx/api/login", async (
            [FromForm] string email,
            [FromForm] string password,
            IWorkflow<LoginRequest, LoginResponse> loginWorkflow) =>
        {
            var request = new LoginRequest { Email = email, Password = password };
            var result = await loginWorkflow.Perform(request);

            if (!result.Success)
            {
                var errorMessage = result.Errors?.FirstOrDefault()?.Message ?? "An error occurred";
                return Results.Content(
                    page.RenderError(errorMessage),
                    "text/html");
            }

            return Results.Content(
                page.RenderSuccess(result.Model.Token),
                "text/html");
        });
    }
}
