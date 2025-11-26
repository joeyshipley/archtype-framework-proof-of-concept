using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Application.Todos.ListTodos;
using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.TodoPage;

public class TodosPageEndpoints(IPageLayout _layout) : IClientEndpoint
{
    private const string ROUTE_BASE = "todos";
    
    public void Map(IEndpointRouteBuilder endpoints)
    {
        // TODO: DI the TodoPage?
        // Not sure that'll work at the level this is attached to program at this time.
        var page = new TodosPage();

        endpoints.MapGet(ROUTE_BASE, async (
            IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse> listWorkflow
        ) =>
        {
            var request = new ListTodosWorkflowRequest();
            var result = await listWorkflow.Perform(request);

            if (!result.Success)
            {
                var errorContent = page.RenderError("Failed to load todos");
                var errorPage = _layout.Render("Todos", errorContent);
                return htmxResult(errorPage);
            }

            var bodyContent = page.RenderPage(result.Model.Todos);
            var fullPage = _layout.Render("Todos", bodyContent);
            return htmxResult(fullPage);
        });

        endpoints.MapPost(interactionUrl("create"), async (
            [FromForm] CreateTodoWorkflowRequest createWorkflowRequest,
            IWorkflow<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse> createWorkflow
        ) => 
        {
            var createResult = await createWorkflow.Perform(createWorkflowRequest);

            if (!createResult.Success)
                return htmxResult(page.RenderErrorNotification("Failed to create todo"));

            return htmxResult(page.RenderTodoItem(createResult.Model.Todo));
        });

        endpoints.MapPost(interactionUrl("toggle"), async (
            [FromForm] ToggleTodoWorkflowRequest toggleWorkflowRequest,
            IWorkflow<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse> toggleWorkflow
        ) =>
        {
            var toggleResult = await toggleWorkflow.Perform(toggleWorkflowRequest);

            if (!toggleResult.Success)
                return htmxResult(page.RenderErrorNotification("Failed to toggle todo"));

            return htmxResult(page.RenderTodoList(toggleResult.Model.Todos));
        });

        endpoints.MapPost(interactionUrl("delete"), async (
            [FromForm] DeleteTodoWorkflowRequest deleteWorkflowRequest,
            IWorkflow<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse> deleteWorkflow
        ) =>
        {
            var deleteResult = await deleteWorkflow.Perform(deleteWorkflowRequest);

            // TODO: fetch the task from DB and send back a row with error state instead of using OH NOES!!! in html
            // This will be needed for better UX in other places.
            if (!deleteResult.Success)
                return htmxResult(page.RenderDeleteErrorWithNotification(deleteWorkflowRequest.Id, "Failed to delete todo"));

            return htmxResult(string.Empty);
        });
    }
    
    private string interactionUrl(string path) => $"/interaction/{ ROUTE_BASE }/{ path.TrimStart('/') }";

    private IResult htmxResult(string content) => Results.Content(content, "text/html");
}
