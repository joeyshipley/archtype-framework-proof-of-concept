using PagePlay.Site.Application.Todos.Domain.Models;
using PagePlay.Site.Infrastructure.Web.Html;
using PagePlay.Site.Pages.Shared.Elements;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageView
{
    string RenderPage(List<TodoListEntry> todos);
    string RenderCreateForm();
    string RenderTodoList(List<TodoListEntry> todos);
    string RenderTodoItem(TodoListEntry todo);
    string RenderError(string error);
    string RenderErrorNotification(string error);
    string RenderDeleteErrorWithNotification(long todoId, string error);
}

public class TodosPage : ITodosPageView
{
    // language=html
    public string RenderPage(List<TodoListEntry> todos) =>
    $$"""
    <div class="todo-page">
        <h1>My Todos</h1>
        <div id="notifications"></div>
        {{RenderCreateForm()}}
        <div id="todo-list">
            {{RenderTodoList(todos)}}
        </div>
    </div>
    """;

    // TODO: look into this input after-request inline javascript.
    // This is breaking our no JS policy, even though it's minimal.
    // language=html
    public string RenderCreateForm() =>
    $$"""
    <div class="todo-create-form">
        <form hx-post="/interaction/todos/create"
              hx-target="#todo-list-ul"
              hx-swap="afterbegin"
              hx-on::after-request="if(event.detail.successful) { this.reset(); document.querySelector('.todo-empty')?.remove(); }">
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
    public string RenderTodoList(List<TodoListEntry> todos)
    {
        var todosHtml = todos.Count == 0
            ? """<li class="todo-empty"><p>No todos yet. Add one above to get started!</p></li>"""
            : string.Join("\n", todos.Select(RenderTodoItem));

        return $$"""
        <ul class="todo-list" id="todo-list-ul">
            {{todosHtml}}
        </ul>
        """;
    }

    // language=html
    public string RenderTodoItem(TodoListEntry todo)
    {
        var completedClass = todo.IsCompleted ? "completed" : "";
        var checkedAttr = todo.IsCompleted ? "checked" : "";

        return $$"""
        <li class="todo-item {{completedClass}}" id="todo-{{todo.Id}}">
            <div class="todo-content">
                <form hx-post="/interaction/todos/toggle"
                      hx-target="#todo-list"
                      hx-swap="morph:innerHTML"
                      hx-ext="morph"
                      class="todo-toggle-form">
                    <input type="hidden" name="id" value="{{todo.Id}}" />
                    <input type="checkbox"
                           {{checkedAttr}}
                           onclick="event.preventDefault(); this.form.requestSubmit();"
                           class="todo-checkbox" />
                </form>
                <span class="todo-title">{{todo.Title.Safe()}}</span>
                {{Button.Render(
                    route: new()
                    {
                        Endpoint = "/interaction/todos/delete",
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
                    endpoint: "/interaction/todos/delete",
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
        {{error.Safe()}}
    </div>
    """;

    // language=html
    public string RenderErrorNotification(string error) =>
    $$"""
    <div id="notifications" hx-swap-oob="true">
        <div class="error" role="alert">
            {{error.Safe()}}
        </div>
    </div>
    """;

    // language=html
    public string RenderDeleteErrorWithNotification(long todoId, string error) =>
    $$"""
    <li class="todo-item todo-error" id="todo-{{todoId}}">
        <div class="todo-content">
            OH NOES!!!
        </div>
    </li>
    {{RenderErrorNotification(error)}}
    """;
}
