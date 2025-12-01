using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Shared;

public interface IWelcomeWidget : IServerComponent
{
    string RenderUnauthenticated();
}

public class WelcomeWidget : IWelcomeWidget
{
    public string ComponentId => "welcome-widget";

    // New typed API - compile-time safe, no magic strings!
    public DataDependencies Dependencies => DataDependencies
        .From<TodosListProvider, TodosListDomainView>();

    public string Render(IDataContext data)
    {
        // Typed access - no magic strings!
        var todosData = data.Get<TodosListDomainView>();
        var count = todosData.OpenCount;

        // language=html
        return $$"""
        <div id="{{ComponentId}}"
             data-component="WelcomeWidget"
             data-domain="todos"
             class="welcome-widget">
            <p>Welcome, you have {{count}} open Todos to look at.</p>
        </div>
        """;
    }

    public string RenderUnauthenticated()
    {
        // language=html
        return $$"""
        <div id="{{ComponentId}}"
             class="welcome-widget">
            <p>Welcome</p>
        </div>
        """;
    }
}
