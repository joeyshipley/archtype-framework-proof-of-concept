using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Pages;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Pages.Todos.Interactions;

public class ToggleTodoInteraction(ITodosPageView _page) : ITodosPageInteraction
{
    public void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost(
        PageInteraction.GetRoute(TodosPageEndpoints.ROUTE_BASE, "toggle"),
        handle
    ).RequireAuthenticatedUser();

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
