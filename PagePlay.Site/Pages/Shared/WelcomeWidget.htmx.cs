using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Shared;

public interface IWelcomeWidget : IServerComponent { }

public class WelcomeWidget : IWelcomeWidget
{
    public string ComponentId => "welcome-widget";

    public DataDependencies Dependencies => DataDependencies
        .From("todos")
        .Require<int>("openCount");

    public string Render(IDataContext data)
    {
        var count = data.Get<int>("todos", "openCount");

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
}
