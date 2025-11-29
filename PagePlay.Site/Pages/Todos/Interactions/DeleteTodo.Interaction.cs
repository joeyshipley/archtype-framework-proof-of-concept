using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Pages;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class DeleteTodoInteraction(ITodosPageView page)
    : PageInteractionBase<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string Action => "delete";

    protected override IResult OnSuccess(DeleteTodoWorkflowResponse response) =>
        Results.Content(string.Empty, "text/html");

    // TODO: fetch the task from DB and send back a row with error state instead of using OH NOES!!! in html
    // This will be needed for better UX in other places.
    protected override IResult OnError(IEnumerable<ResponseErrorEntry> errors)
    {
        // DeleteTodo needs the request ID to render the error state properly
        // For now, we'll use the generic error handling until the TODO above is addressed
        var errorMessage = errors.FirstOrDefault()?.Message ?? "Failed to delete todo";
        return Results.Content(Page.RenderErrorNotification(errorMessage), "text/html");
    }

    protected override IResult RenderError(string message) =>
        Results.Content(Page.RenderErrorNotification(message), "text/html");
}
