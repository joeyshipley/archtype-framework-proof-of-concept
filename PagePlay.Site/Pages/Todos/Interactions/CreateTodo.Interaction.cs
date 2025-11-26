using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Pages;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class CreateTodoInteraction(TodosPage _page) : ITodosPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(TodosPageEndpoints.ROUTE_BASE, "create"), 
        handle
    );

    private async Task<IResult> handle(
        [FromForm] CreateTodoWorkflowRequest createWorkflowRequest,
        IWorkflow<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse> createWorkflow
    )
    {
        var createResult = await createWorkflow.Perform(createWorkflowRequest);

        if (!createResult.Success)
            return Results.Content(_page.RenderErrorNotification("Failed to create todo"), "text/html");

        return Results.Content(_page.RenderTodoItem(createResult.Model.Todo), "text/html");
    }
}
