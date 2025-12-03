using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Todos.Components;

public interface ITodoListComponent : IServerComponent
{
}

public class TodoListComponent(ITodosPageView _page) : ITodoListComponent
{
    public string ComponentId => "todo-list-component";

    public DataDependencies Dependencies =>
        DataDependencies.From<TodosListProvider, TodosListDomainView>();

    public string Render(IDataContext data)
    {
        var todosData = data.Get<TodosListDomainView>();

        // Wrap existing page rendering in component container
        // language=html
        return $$"""
        <div id="{{ComponentId}}"
             data-component="TodoListComponent"
             data-domain="{{TodosListDomainView.DomainName}}">
            {{_page.RenderTodoList(todosData.List)}}
        </div>
        """;
    }
}
