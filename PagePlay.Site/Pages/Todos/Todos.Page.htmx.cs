using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Infrastructure.Web.Html;
using PagePlay.Site.Pages.Shared.Elements;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageView
{
    string RenderPage(List<TodoListEntry> todos);
    string RenderCreateForm();
    string RenderTodoList(List<TodoListEntry> todos);
    string RenderTodoItem(TodoListEntry todo);
    string RenderSuccessfulTodoCreation(TodoListEntry todo);
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

    // language=html
    private string renderCreateFormContent() =>
        HtmxForm.Render(
            new()
            {
                Action = "/interaction/todos/create",
                Target = "#todo-list-ul",
                SwapStrategy = "afterbegin"
            },
            $$"""
            <div class="todo-input-group">
                <input id="title"
                       name="title"
                       type="text"
                       placeholder="What needs to be done?"
                       required
                       maxlength="200" />
                <button type="submit">Add Todo</button>
            </div>
            """
        );

    // language=html
    public string RenderCreateForm() =>
    $$"""
    <div class="todo-create-form" id="todo-create-form">
        {{renderCreateFormContent()}}
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
        var checkboxIcon = todo.IsCompleted ? "☑" : "☐";

        return $$"""
        <li class="todo-item {{completedClass}}" id="todo-{{todo.Id}}">
            <div class="todo-content">
                {{HtmxForm.Render(
                    new()
                    {
                        Action = "/interaction/todos/toggle",
                        Target = "#todo-list",
                        CssClass = "todo-toggle-form"
                    },
                    $$"""
                    <input type="hidden" name="id" value="{{todo.Id}}" />
                    <button type="submit" class="todo-checkbox">{{checkboxIcon}}</button>
                    """
                )}}
                <span class="todo-title">{{todo.Title.Safe()}}</span>
                {{ButtonDelete.Render(
                    endpoint: "/interaction/todos/delete",
                    id: todo.Id,
                    tag: "todo",
                    target: $"#todo-{todo.Id}",
                    content: $$"""×""",
                    swapStrategy: "outerHTML"
                )}}
                <hr />
            </div>
        </li>
        """;
    }

    // language=html
    public string RenderSuccessfulTodoCreation(TodoListEntry todo) =>
    $$"""
    {{RenderTodoItem(todo)}}
    <div class="todo-create-form" id="todo-create-form" hx-swap-oob="true">
        {{renderCreateFormContent()}}
    </div>
    """;

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
