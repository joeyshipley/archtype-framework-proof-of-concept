using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(ITodosPageView page)
    : PageInteractionBase<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.ROUTE_BASE;
    protected override string Action => "toggle";

    protected override IResult OnSuccess(ToggleTodoWorkflowResponse response) =>
        Results.Content(Page.RenderTodoList(response.Todos), "text/html");

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
