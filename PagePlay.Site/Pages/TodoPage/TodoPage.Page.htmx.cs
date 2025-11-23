using System.Web;
using PagePlay.Site.Application.Todo.ListTodos;

namespace PagePlay.Site.Pages.TodoPage;

public class TodoPage
{
    // language=html
    public string RenderPage(string antiforgeryToken, List<TodoItem> todos) =>
    $$"""
    <div class="todo-page">
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
              hx-swap="innerHTML">
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
    public string RenderTodoList(string antiforgeryToken, List<TodoItem> todos)
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
    public string RenderTodoItem(string antiforgeryToken, TodoItem todo)
    {
        var completedClass = todo.IsCompleted ? "completed" : "";
        var checkedAttr = todo.IsCompleted ? "checked" : "";

        return $$"""
        <li class="todo-item {{completedClass}}" data-todo-id="{{todo.Id}}">
            <div class="todo-content">
                <form hx-post="/api/todos/toggle"
                      hx-target="#todo-list"
                      hx-swap="innerHTML"
                      class="todo-toggle-form">
                    <input type="hidden" name="__RequestVerificationToken" value="{{antiforgeryToken}}" />
                    <input type="hidden" name="id" value="{{todo.Id}}" />
                    <input type="checkbox"
                           {{checkedAttr}}
                           onchange="this.form.requestSubmit()"
                           class="todo-checkbox" />
                </form>
                <span class="todo-title">{{HttpUtility.HtmlEncode(todo.Title)}}</span>
                <button hx-post="/api/todos/delete"
                        hx-target="#todo-list"
                        hx-swap="innerHTML"
                        hx-vals='{"id": {{todo.Id}}, "__RequestVerificationToken": "{{antiforgeryToken}}"}'
                        class="todo-delete"
                        title="Delete todo">
                    ✕
                </button>
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
