using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class CreateTodoInteraction(ITodosPageView page)
    : PageInteractionBase<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.ROUTE_BASE;
    protected override string Action => "create";

    protected override IResult OnSuccess(CreateTodoWorkflowResponse response) =>
        Results.Content(Page.RenderSuccessfulTodoCreation(response.Todo), "text/html");

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
