using PagePlay.Site.Application.StyleTest.GetRandomNumber;
using PagePlay.Site.Infrastructure.Web.Framework;
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

    protected override async Task<IResult> OnSuccess(GetRandomNumberWorkflowResponse response)
    {
        var content = Page.RenderRandomNumber(response.Number);
        return await BuildHtmlFragmentResult(content);
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderError(message), "text/html");
}
