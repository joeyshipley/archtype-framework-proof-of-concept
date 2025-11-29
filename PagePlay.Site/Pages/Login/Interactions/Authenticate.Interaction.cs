using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Web.Http;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Login.Interactions;

public class AuthenticateInteraction(
    ILoginPageView page,
    ICookieManager cookieManager,
    IResponseManager responseManager
) : PageInteractionBase<LoginWorkflowRequest, LoginWorkflowResponse, ILoginPageView>(page),
    ILoginPageInteraction
{
    protected override string RouteBase => LoginPageEndpoints.ROUTE_BASE;
    protected override string Action => "authenticate";
    protected override bool RequireAuth => false;

    protected override IResult OnSuccess(LoginWorkflowResponse response)
    {
        cookieManager.SetAuthCookie(response.Token);
        responseManager.SetRedirectHeader("/todos");
        return Results.Ok();
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderError(message), "text/html");
}
