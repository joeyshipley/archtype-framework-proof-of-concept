using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Html;
using PagePlay.Site.Infrastructure.Web.Http;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Login.Interactions;

public class AuthenticateInteraction(
    ILoginPageView page,
    ICookieManager cookieManager,
    IResponseManager responseManager,
    IFrameworkOrchestrator framework
) : PageInteractionBase<LoginWorkflowRequest, LoginWorkflowResponse, ILoginPageView>(page, framework),
    ILoginPageInteraction
{
    protected override string RouteBase => LoginPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "authenticate";
    protected override bool RequireAuth => false;

    protected override Task<IResult> OnSuccess(LoginWorkflowResponse response)
    {
        cookieManager.SetAuthCookie(response.Token);
        responseManager.SetRedirectHeader("/todos");
        return Task.FromResult(Results.Ok());
    }

    protected override IResult RenderError(string message)
    {
        // Need to return both: error notification + form preservation
        // Otherwise HTMX removes the form (triggering element) when it doesn't receive an update for it
        var errorNotification = HtmlFragment.WithOob("notifications", Page.RenderError(message));
        var formReset = HtmlFragment.InjectOob(Page.RenderLoginForm());

        var combinedResponse = $"{formReset}\n{errorNotification}";
        return Results.Content(combinedResponse, "text/html");
    }
}
