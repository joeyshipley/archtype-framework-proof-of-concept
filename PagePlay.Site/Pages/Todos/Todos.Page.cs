using PagePlay.Site.Application.Todos.Models;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Infrastructure.UI;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageView : IView
{
    string RenderCreateForm();
    string RenderTodoItem(TodoListEntry todo);
    string RenderError(string error);
    string RenderErrorNotification(string error);
    string RenderDeleteErrorWithNotification(long todoId, string error);
}

public class TodosPage(IHtmlRenderer _renderer) : ITodosPageView
{
    public string ViewId => "todo-page";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todos = data.Get<TodosListDomainView>();
        var openTodos = todos.List.Where(t => !t.IsCompleted).ToList();
        var completedTodos = todos.List.Where(t => t.IsCompleted).ToList();

        return _renderer.Render(
            new Page(
                new PageTitle("My Todos"),
                new Section().Id("notifications"),
                new Grid(For.Sections, Columns.Three,
                    renderAddColumn(),
                    renderOpenColumn(openTodos),
                    renderCompletedColumn(completedTodos)
                )
            ).Id(ViewId)
        );
    }

    private Section renderOpenColumn(List<TodoListEntry> openTodos) =>
        new Section()
            .Id("open-todos")
            .Children(
                new SectionTitle("Open"),
                renderTodoListComponent(openTodos, "No open todos")
            );

    private Section renderAddColumn() =>
        new Section()
            .Id("todo-create-form")
            .Children(
                new SectionTitle("\u00A0"),
                renderCreateFormContent()
            );

    private Section renderCompletedColumn(List<TodoListEntry> completedTodos) =>
        new Section()
            .Id("completed-todos")
            .Children(
                new SectionTitle("Completed"),
                renderTodoListComponent(completedTodos, "No completed todos")
            );

    private Form renderCreateFormContent() =>
        new Form()
            .Action("/interaction/todos/create")
            .Children(
                new Stack(For.Items,
                    new Input()
                        .Name("title")
                        .Type(InputType.Text)
                        .Placeholder("What needs to be done?")
                        .Id("title"),
                    new Button(Importance.Primary, "Add")
                        .Type(ButtonType.Submit)
                )
            );

    public string RenderCreateForm() =>
        _renderer.Render(renderAddColumn());

    private IElement renderTodoListComponent(List<TodoListEntry> todos, string emptyMessage = "No todos")
    {
        if (todos.Count == 0)
        {
            return new EmptyState(emptyMessage)
            {
                ElementSize = EmptyStateSize.Small
            };
        }

        var list = new List()
            .Style(ListStyle.Plain);

        foreach (var todo in todos)
        {
            list.Add(renderTodoItemComponent(todo));
        }

        return list;
    }

    private ListItem renderTodoItemComponent(TodoListEntry todo) =>
        new ListItem()
            .State(todo.IsCompleted ? ListItemState.Completed : ListItemState.Normal)
            .Id($"todo-{todo.Id}")
            .Children(
                new Row(For.Items,
                    // Toggle form with checkbox button
                    new Form()
                        .Action("/interaction/todos/toggle")
                        .Children(
                            new Input()
                                .Name("id")
                                .Type(InputType.Hidden)
                                .Value(todo.Id.ToString()),
                            new Button(Importance.Ghost, todo.IsCompleted ? "☑" : "☐")
                                .Type(ButtonType.Submit)
                        ),
                    new Text(todo.Title),
                    // Delete button
                    new Button(Importance.Ghost, "×")
                        .Action("/interaction/todos/delete")
                        .ModelId(todo.Id)
                )
            );

    public string RenderTodoItem(TodoListEntry todo) =>
        _renderer.Render(renderTodoItemComponent(todo));

    public string RenderError(string error) =>
        _renderer.Render(
            new Alert(error, AlertTone.Critical)
        );

    public string RenderErrorNotification(string error) =>
        _renderer.Render(
            new Section()
                .Id("notifications")
                .Children(
                    new Alert(error, AlertTone.Critical)
                )
        );

    public string RenderDeleteErrorWithNotification(long todoId, string error)
    {
        var errorItem = new ListItem()
            .State(ListItemState.Error)
            .Id($"todo-{todoId}")
            .Children(
                new Text("OH NOES!!!")
            );

        return _renderer.Render(errorItem) + RenderErrorNotification(error);
    }
}
