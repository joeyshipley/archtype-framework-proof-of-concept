using System.Web;
using PagePlay.Site.Application.Todos.ListTodos;

namespace PagePlay.Site.Pages.TodoPage;

public class TodoPage
{
    // language=html
    public string RenderPage(string antiforgeryToken, List<TodoItem> todos) =>
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
        <li class="todo-item {{completedClass}}" id="todo-{{todo.Id}}">
            <div class="todo-content">
                <form hx-post="/api/todos/toggle"
                      hx-target="#todo-list"
                      hx-swap="morph:innerHTML"
                      class="todo-toggle-form">
                    <input type="hidden" name="__RequestVerificationToken" value="{{antiforgeryToken}}" />
                    <input type="hidden" name="id" value="{{todo.Id}}" />
                    <input type="checkbox"
                           {{checkedAttr}}
                           onclick="event.preventDefault(); this.form.requestSubmit();"
                           class="todo-checkbox" />
                </form>
                <span class="todo-title">{{todo.Title}}</span>
                {{ButtonComponent(
                    content: $$"""<span class="delete-inner-container">Delete {{ todo.Title }}</span>""",
                    endpoint: "/api/todos/delete",
                    antiforgeryToken: antiforgeryToken,
                    cssClass: "todo-delete",
                    title: "Delete todo",
                    additionalData: new Dictionary<string, object>
                    {
                        { "id", todo.Id }
                    }
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

    // language=html
    private string ButtonComponent(
        string content,
        string endpoint,
        string antiforgeryToken,
        string? cssClass = null,
        string? title = null,
        string? targetSelector = "#todo-list",
        string? swapStrategy = "morph:innerHTML",
        Dictionary<string, object>? additionalData = null,
        string httpMethod = "post")
    {
        var classAttr = !string.IsNullOrEmpty(cssClass) ? $"class=\"{cssClass}\"" : "";
        var titleAttr = !string.IsNullOrEmpty(title) ? $"title=\"{title}\"" : "";

        // Automatically include antiforgery token
        var data = new Dictionary<string, object>
        {
            { "__RequestVerificationToken", antiforgeryToken }
        };

        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
                data[kvp.Key] = kvp.Value;
        }

        // Build hx-vals JSON
        var jsonPairs = data.Select(kvp =>
            kvp.Value is string
                ? $"\"{kvp.Key}\": \"{kvp.Value}\""
                : $"\"{kvp.Key}\": {kvp.Value}");
        var json = string.Join(", ", jsonPairs);
        var hxValsAttr = $"hx-vals='{{ {json} }}'";

        return $$"""
        <button hx-{{httpMethod}}="{{endpoint}}"
                hx-target="{{targetSelector}}"
                hx-swap="{{swapStrategy}}"
                {{hxValsAttr}}
                {{classAttr}}
                {{titleAttr}}>
            {{content}}
        </button>
        """;
    }
}
