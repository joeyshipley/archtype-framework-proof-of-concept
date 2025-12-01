using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator framework
) : PageInteractionBase<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse, ITodosPageView>(page, framework),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string RouteAction => "toggle";
    protected override DataMutations Mutates => DataMutations.For("todos");

    protected override async Task<IResult> OnSuccess(ToggleTodoWorkflowResponse response)
    {
        var content = Page.RenderTodoList(response.Todos);
        return await BuildHtmlFragmentResult(content);
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
