using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class DeleteTodoInteraction(ITodosPageView _page) : ITodosPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(TodosPageEndpoints.ROUTE_BASE, "delete"),
        handle
    ).RequireAuthenticatedUser();

    private async Task<IResult> handle(
        [FromForm] DeleteTodoWorkflowRequest deleteWorkflowRequest,
        IWorkflow<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse> deleteWorkflow
    )
    {
        var deleteResult = await deleteWorkflow.Perform(deleteWorkflowRequest);

        // TODO: fetch the task from DB and send back a row with error state instead of using OH NOES!!! in html
        // This will be needed for better UX in other places.
        if (!deleteResult.Success)
            return Results.Content(_page.RenderDeleteErrorWithNotification(deleteWorkflowRequest.Id, "Failed to delete todo"), "text/html");

        return Results.Content(string.Empty, "text/html");
    }
}
