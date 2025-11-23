using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Pages.Signin;

public static class SigninEndpoint
{
    public static void MapSigninRoutes(this IEndpointRouteBuilder endpoints)
    {
        var component = new SigninComponent();

        endpoints.MapGet("/signin", () =>
            Results.Content(component.RenderPage(), "text/html"));

        endpoints.MapPost("/api/signin", async (
            [FromForm] string email,
            [FromForm] string password,
            IWorkflow<LoginRequest, LoginResponse> workflow) =>
        {
            var request = new LoginRequest { Email = email, Password = password };
            var result = await workflow.Perform(request);

            if (!result.Success)
            {
                var errorMessage = result.Errors.FirstOrDefault()?.Message ?? "An error occurred";
                return Results.Content(
                    component.RenderError(errorMessage),
                    "text/html");
            }

            return Results.Content(
                component.RenderSuccess(result.Model?.Token ?? "No token received"),
                "text/html");
        })
        .DisableAntiforgery();
    }
}
