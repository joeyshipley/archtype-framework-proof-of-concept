using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Mutations;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class DeleteTodoInteraction(
    ITodosPageView page,
    IFrameworkOrchestrator _framework
) : PageInteractionBase<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse, ITodosPageView>(page),
      ITodosPageInteraction
{
    protected override string RouteBase => TodosPageEndpoints.PAGE_ROUTE;
    protected override string Action => "delete";

    // Declare what this interaction mutates
    protected virtual DataMutations Mutates => DataMutations.For("todos");

    protected override async Task<IResult> OnSuccess(DeleteTodoWorkflowResponse response)
    {
        // Get component context from request header
        var contextHeader = HttpContext.Request.Headers["X-Component-Context"].ToString();

        // Framework handles re-rendering affected components (returns HTML string)
        var oobHtml = await _framework.RenderMutationResponseAsync(Mutates, contextHeader);

        // For delete, we return empty content for the deleted item plus OOB updates
        return Results.Content(oobHtml, "text/html");
    }

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
