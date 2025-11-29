using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(ITodosPageView page)
    : PageInteractionBase<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string Action => "toggle";

    protected override Task<IResult> OnSuccess(ToggleTodoWorkflowResponse response) =>
        Task.FromResult(Results.Content(Page.RenderTodoList(response.Todos), "text/html"));

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
