using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Pages;
using PagePlay.Site.Pages.TodoPage;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class DeleteTodoInteraction(TodosPage _page) : IPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(TodosPageEndpoints.ROUTE_BASE, "delete"), 
        handle
    );

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
