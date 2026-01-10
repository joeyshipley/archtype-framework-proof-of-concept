using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Html;
using PagePlay.Site.Infrastructure.Web.Http;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Register.Interactions;

public class CreateAccountInteraction(
    IRegisterPageView page,
    IResponseManager responseManager,
    IFrameworkOrchestrator framework
) : PageInteractionBase<RegisterRequest, RegisterResponse, IRegisterPageView>(page, framework),
    IRegisterPageInteraction
{
    protected override string RouteBase => RegisterPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "create";
    protected override bool RequireAuth => false;

    protected override Task<IResult> OnSuccess(RegisterResponse response)
    {
        responseManager.SetRedirectHeader("/login");
        return Task.FromResult(Results.Ok());
    }

    protected override IResult RenderError(string message)
    {
        var errorHtml = Page.RenderErrorNotification(message);
        return BuildOobOnly(HtmlFragment.InjectOob(errorHtml));
    }
}
