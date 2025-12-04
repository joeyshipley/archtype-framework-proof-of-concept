using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Workflows.ToggleTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;
using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator framework
) : PageInteractionBase<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse, ITodosPageView>(page, framework),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "toggle";
    protected override DataMutations Mutates => DataMutations.For(TodosListDomainView.DomainName);

    protected override async Task<IResult> OnSuccess(ToggleTodoWorkflowResponse response)
    {
        return await BuildOobResult();
    }

    protected override IResult RenderError(string message)
    {
        // Return empty main content (to prevent form replacement) + OOB notification
        var errorHtml = Page.RenderErrorNotification(message);
        var mainContent = ""; // Empty keeps form unchanged
        var oobNotification = HtmlFragment.InjectOob(errorHtml);
        return Results.Content(mainContent + oobNotification, "text/html");
    }
}
