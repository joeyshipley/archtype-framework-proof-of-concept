using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Pages;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(TodosPage _page) : ITodosPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(TodosPageEndpoints.ROUTE_BASE, "toggle"), 
        handle
    );

    private async Task<IResult> handle(
        [FromForm] ToggleTodoWorkflowRequest toggleWorkflowRequest,
        IWorkflow<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse> toggleWorkflow
    )
    {
        var toggleResult = await toggleWorkflow.Perform(toggleWorkflowRequest);

        if (!toggleResult.Success)
            return Results.Content(_page.RenderErrorNotification("Failed to toggle todo"), "text/html");

        return Results.Content(_page.RenderTodoList(toggleResult.Model.Todos), "text/html");
    }
}
