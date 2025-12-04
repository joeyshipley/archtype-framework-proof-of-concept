using PagePlay.Site.Application.StyleTest.GetRandomNumber;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Html;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.StyleTest.Interactions;

public class GetRandomNumberInteraction(
    IStyleTestPageView page,
    IFrameworkOrchestrator framework
) : PageInteractionBase<GetRandomNumberWorkflowRequest, GetRandomNumberWorkflowResponse, IStyleTestPageView>(page, framework),
    IStyleTestPageInteraction
{
    protected override string RouteBase => StyleTestPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "random";
    protected override bool RequireAuth => false;

    protected override Task<IResult> OnSuccess(GetRandomNumberWorkflowResponse response)
    {
        // OOB-only pattern - update result section with random number
        var resultHtml = Page.RenderRandomNumber(response.Number);
        return Task.FromResult(BuildOobOnly(HtmlFragment.InjectOob(resultHtml)));
    }

    protected override IResult RenderError(string message)
    {
        // OOB-only pattern - update result section with error
        var errorHtml = Page.RenderError(message);
        return BuildOobOnly(HtmlFragment.InjectOob(errorHtml));
    }
}
