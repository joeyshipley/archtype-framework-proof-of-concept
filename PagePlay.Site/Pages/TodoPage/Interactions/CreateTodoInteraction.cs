using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Pages.TodoPage.Interactions;

public class CreateTodoInteraction(TodosPage _page) : IPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(TodosPageEndpoints.ROUTE_BASE, "creates"), 
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
