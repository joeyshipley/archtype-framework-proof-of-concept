using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Todos.CreateTodo;
using PagePlay.Site.Application.Todos.DeleteTodo;
using PagePlay.Site.Application.Todos.ListTodos;
using PagePlay.Site.Application.Todos.ToggleTodo;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.TodoPage;

public static class TodosPageEndpoints
{
    private static IResult Html(string content) => Results.Content(content, "text/html");

    public static void MapTodoPageRoutes(this IEndpointRouteBuilder endpoints)
    {
        var page = new TodosPage();

        endpoints.MapGet("/todos", async (
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
                return Html(errorPage);
            }

            var bodyContent = page.RenderPage(result.Model.Todos);
            var fullPage = Layout.Render(bodyContent, "Todos", tokens.RequestToken!);
            return Html(fullPage);
        });

        endpoints.MapPost("/api/todos/create", async (
            [FromForm] CreateTodoRequest createRequest,
            IWorkflow<CreateTodoRequest, CreateTodoResponse> createWorkflow
        ) => 
        {
            var createResult = await createWorkflow.Perform(createRequest);

            if (!createResult.Success)
                return Html(page.RenderErrorNotification("Failed to create todo"));

            return Html(page.RenderTodoItem(createResult.Model.Todo));
        });

        endpoints.MapPost("/api/todos/toggle", async (
            [FromForm] ToggleTodoRequest toggleRequest,
            IWorkflow<ToggleTodoRequest, ToggleTodoResponse> toggleWorkflow
        ) =>
        {
            var toggleResult = await toggleWorkflow.Perform(toggleRequest);

            if (!toggleResult.Success)
                return Html(page.RenderErrorNotification("Failed to toggle todo"));

            return Html(page.RenderTodoList(toggleResult.Model.Todos));
        });

        endpoints.MapPost("/api/todos/delete", async (
            [FromForm] DeleteTodoRequest deleteRequest,
            IWorkflow<DeleteTodoRequest, DeleteTodoResponse> deleteWorkflow
        ) =>
        {
            var deleteResult = await deleteWorkflow.Perform(deleteRequest);

            if (!deleteResult.Success)
                return Html(page.RenderDeleteErrorWithNotification(deleteRequest.Id, "Failed to delete todo"));

            return Html(string.Empty);
        });
    }
}
