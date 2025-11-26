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

        // TODO: Set authentication cookie/session with loginResult.Model.Token

        context.Response.Headers.Append("HX-Redirect", "/todos");
        return Results.Ok();
    }
}
