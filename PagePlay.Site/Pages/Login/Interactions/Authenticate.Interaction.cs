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
        // Only return error notification OOB - form stays as-is with user's values
        var errorHtml = Page.RenderErrorNotification(message);
        return BuildOobOnly(HtmlFragment.InjectOob(errorHtml));
    }
}
