using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Pages;

namespace PagePlay.Site.Pages.Login.Interactions;

public class AuthenticateInteraction(LoginPage _page) : ILoginPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(LoginPageEndpoints.ROUTE_BASE, "authenticate"),
        handle
    );

    private async Task<IResult> handle(
        HttpContext context,
        [FromForm] LoginWorkflowRequest loginWorkflowRequest,
        IWorkflow<LoginWorkflowRequest, LoginWorkflowResponse> loginWorkflow
    )
    {
        var loginResult = await loginWorkflow.Perform(loginWorkflowRequest);

        if (!loginResult.Success)
            return Results.Content(_page.RenderError("Invalid email or password"), "text/html");

        // Set secure HTTP-only cookie with JWT token
        context.Response.Cookies.Append("auth_token", loginResult.Model.Token, new CookieOptions
        {
            HttpOnly = true,                        // Prevents JavaScript access (XSS protection)
            Secure = true,                          // Only sent over HTTPS
            SameSite = SameSiteMode.Strict,         // CSRF protection
            MaxAge = TimeSpan.FromMinutes(60),      // Match JWT expiration
            Path = "/"
        });

        context.Response.Headers.Append("HX-Redirect", "/todos");
        return Results.Ok();
    }
}
