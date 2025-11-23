using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Signin2;

public static class Signin2Endpoints
{
    public static void MapSignin2Routes(this IEndpointRouteBuilder endpoints)
    {
        var page = new Signin2Page();

        endpoints.MapGet("/signin2", (IAntiforgery antiforgery, HttpContext context) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var bodyContent = page.RenderPage(tokens.RequestToken!);
            var fullPage = Layout.Render(bodyContent, "Sign In");
            return Results.Content(fullPage, "text/html");
        });

        endpoints.MapPost("/api/signin2", async (
            [FromForm] string email,
            [FromForm] string password,
            IWorkflow<LoginRequest, LoginResponse> workflow) =>
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var result = await workflow.Perform(request);

            if (!result.Success)
            {
                var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "An error occurred";
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
