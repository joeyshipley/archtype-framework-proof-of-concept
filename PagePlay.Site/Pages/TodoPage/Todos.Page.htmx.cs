using System.Web;
using PagePlay.Site.Application.Todos.Domain.Models;

namespace PagePlay.Site.Pages.TodoPage;

public class TodosPage
{
    // language=html
    public string RenderPage(string antiforgeryToken, List<TodoListEntry> todos) =>
    $$"""
    <div class="todo-page" hx-ext="morph">
        <h1>My Todos</h1>
        {{RenderCreateForm(antiforgeryToken)}}
        <div id="todo-list">
            {{RenderTodoList(antiforgeryToken, todos)}}
        </div>
    </div>
    """;

    // language=html
    public string RenderCreateForm(string antiforgeryToken) =>
    $$"""
    <div class="todo-create-form">
        <form hx-post="/api/todos/create"
              hx-target="#todo-list"
              hx-swap="morph:innerHTML"
              hx-on::after-request="if(event.detail.successful) this.reset()">
            <input type="hidden" name="__RequestVerificationToken" value="{{antiforgeryToken}}" />
            <div class="todo-input-group">
                <input id="title"
                       name="title"
                       type="text"
                       placeholder="What needs to be done?"
                       required
                       maxlength="200" />
                <button type="submit">Add Todo</button>
            </div>
        </form>
    </div>
    """;

    // language=html
    public string RenderTodoList(string antiforgeryToken, List<TodoListEntry> todos)
    {
        if (todos.Count == 0)
        {
            return """
            <div class="todo-empty">
                <p>No todos yet. Add one above to get started!</p>
            </div>
            """;
        }

        var todosHtml = string.Join("\n", todos.Select(todo => RenderTodoItem(antiforgeryToken, todo)));

        return $$"""
        <ul class="todo-list">
            {{todosHtml}}
        </ul>
        """;
    }

    // language=html
    public string RenderTodoItem(string antiforgeryToken, TodoListEntry todo)
    {
        var completedClass = todo.IsCompleted ? "completed" : "";
        var checkedAttr = todo.IsCompleted ? "checked" : "";

        return $$"""
        <li class="todo-item {{completedClass}}" id="todo-{{todo.Id}}">
            <div class="todo-content">
                <form hx-post="/api/todos/toggle"
                      hx-target="#todo-list"
                      hx-swap="morph:innerHTML"
                      class="todo-toggle-form">
                    <input type="hidden" name="id" value="{{todo.Id}}" />
                    <input type="checkbox"
                           {{checkedAttr}}
                           onclick="event.preventDefault(); this.form.requestSubmit();"
                           class="todo-checkbox" />
                </form>
                <span class="todo-title">{{todo.Title}}</span>
                {{Button.Render(
                    route: new()
                    {
                        Endpoint = "/api/todos/delete",
                        ModelId = todo.Id,
                        Target = $"#todo-{todo.Id}",
                        SwapStrategy = "outerHTML"
                    },
                    html: new()
                    {
                        ElementId = $"delete-todo-{todo.Id}",
                        Title = "Delete todo",
                        CssClass = "todo-delete"
                    },
                    content: $$"""X1"""
                )}}
                {{ButtonDelete.Render(
                    endpoint: "/api/todos/delete",
                    id: todo.Id,
                    tag: "todo",
                    target: $"#todo-{todo.Id}",
                    content: $$"""X2""",
                    swapStrategy: "outerHTML"
                )}}
                <hr />
            </div>
        </li>
        """;
    }

    // language=html
    public string RenderError(string error) =>
    $$"""
    <div class="error" role="alert">
        {{HttpUtility.HtmlEncode(error)}}
    </div>
    """;
}
