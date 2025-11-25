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
                return Results.Content(errorPage, "text/html");
            }

            var bodyContent = page.RenderPage(tokens.RequestToken!, result.Model.Todos);
            var fullPage = Layout.Render(bodyContent, "Todos", tokens.RequestToken!);
            return Results.Content(fullPage, "text/html");
        });

        endpoints.MapPost("/api/todos/create", async (
            [FromForm] string title,
            IAntiforgery antiforgery,
            HttpContext context,
            IWorkflow<CreateTodoRequest, CreateTodoResponse> createWorkflow) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var createRequest = new CreateTodoRequest { Title = title };
            var createResult = await createWorkflow.Perform(createRequest);

            if (!createResult.Success)
                return Results.Content(page.RenderError("Failed to create todo"), "text/html");

            return Results.Content(
                page.RenderTodoItem(tokens.RequestToken!, createResult.Model.Todo),
                "text/html");
        });

        endpoints.MapPost("/api/todos/toggle", async (
            [FromForm] long id,
            IAntiforgery antiforgery,
            HttpContext context,
            IWorkflow<ToggleTodoRequest, ToggleTodoResponse> toggleWorkflow) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var toggleRequest = new ToggleTodoRequest { Id = id };
            var toggleResult = await toggleWorkflow.Perform(toggleRequest);

            if (!toggleResult.Success)
                return Results.Content(page.RenderError("Failed to toggle todo"), "text/html");

            return Results.Content(
                page.RenderTodoList(tokens.RequestToken!, toggleResult.Model.Todos),
                "text/html");
        });

        endpoints.MapPost("/api/todos/delete", async (
            [FromForm] long id,
            IWorkflow<DeleteTodoRequest, DeleteTodoResponse> deleteWorkflow) =>
        {
            var deleteRequest = new DeleteTodoRequest { Id = id };
            var deleteResult = await deleteWorkflow.Perform(deleteRequest);

            if (!deleteResult.Success)
                return Results.BadRequest();

            return Results.Ok();
        });
    }
}
