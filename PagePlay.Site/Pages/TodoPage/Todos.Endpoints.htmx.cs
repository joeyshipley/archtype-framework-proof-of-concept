using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Application.Todos.ListTodos;
using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.TodoPage;

public class TodosPageEndpoints : IClientEndpoint
{
    private const string ROUTE_BASE = "todos";
    
    public void Map(
        IEndpointRouteBuilder endpoints
    )
    {
        // TODO: DI the TodoPage?
        // Not sure that'll work at the level this is attached to program at this time.
        var page = new TodosPage();

        endpoints.MapGet(ROUTE_BASE, async (
            IAntiforgery antiforgery,
            HttpContext context,
            IWorkflow<ListTodosRequest, ListTodosResponse> listWorkflow
        ) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var request = new ListTodosRequest();
            var result = await listWorkflow.Perform(request);

            if (!result.Success)
            {
                var errorContent = page.RenderError("Failed to load todos");
                var errorPage = Layout.Render(errorContent, "Todos", tokens.RequestToken!);
                return htmxResult(errorPage);
            }

            var bodyContent = page.RenderPage(result.Model.Todos);
            var fullPage = Layout.Render(bodyContent, "Todos", tokens.RequestToken!);
            return htmxResult(fullPage);
        });

        endpoints.MapPost(interactionUrl("create"), async (
            [FromForm] CreateTodoRequest createRequest,
            IWorkflow<CreateTodoRequest, CreateTodoResponse> createWorkflow
        ) => 
        {
            var createResult = await createWorkflow.Perform(createRequest);

            if (!createResult.Success)
                return htmxResult(page.RenderErrorNotification("Failed to create todo"));

            return htmxResult(page.RenderTodoItem(createResult.Model.Todo));
        });

        endpoints.MapPost(interactionUrl("toggle"), async (
            [FromForm] ToggleTodoRequest toggleRequest,
            IWorkflow<ToggleTodoRequest, ToggleTodoResponse> toggleWorkflow
        ) =>
        {
            var toggleResult = await toggleWorkflow.Perform(toggleRequest);

            if (!toggleResult.Success)
                return htmxResult(page.RenderErrorNotification("Failed to toggle todo"));

            return htmxResult(page.RenderTodoList(toggleResult.Model.Todos));
        });

        endpoints.MapPost(interactionUrl("delete"), async (
            [FromForm] DeleteTodoRequest deleteRequest,
            IWorkflow<DeleteTodoRequest, DeleteTodoResponse> deleteWorkflow
        ) =>
        {
            var deleteResult = await deleteWorkflow.Perform(deleteRequest);

            // TODO: fetch the task from DB and send back a row with error state instead of using OH NOES!!! in html
            // This will be needed for better UX in other places.
            if (!deleteResult.Success)
                return htmxResult(page.RenderDeleteErrorWithNotification(deleteRequest.Id, "Failed to delete todo"));

            return htmxResult(string.Empty);
        });
    }
    
    private string interactionUrl(string path) => $"/interaction/{ ROUTE_BASE }/{ path.TrimStart('/') }";

    private IResult htmxResult(string content) => Results.Content(content, "text/html");
}
