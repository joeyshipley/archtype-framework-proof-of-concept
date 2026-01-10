using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;
using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Home;

public interface IHomePageView : IView
{
}

public class HomePage(IHtmlRenderer _renderer) : IHomePageView
{
    public string ViewId => "home-page";

    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data)
    {
        var page = new Page(
            new PageTitle("Welcome to TODO"),
            new Stack(For.Sections,
                new Stack(For.Content,
                    new Text("This example uses ArchType, a framework for building web applications with server authority and semantic UIs.")
                ),
                new Row(For.Actions,
                    new Link("Style Test", "/style-test").Style(LinkStyle.ButtonSecondary),
                    new Link("Get Started", "/login").Style(LinkStyle.Button)
                )
            )
        ).Id(ViewId);

        return _renderer.Render(page);
    }
}
