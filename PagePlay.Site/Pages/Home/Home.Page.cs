using PagePlay.Site.Infrastructure.Web.Components;

namespace PagePlay.Site.Pages.Home;

public interface IHomePageView : IView
{
}

public class HomePage : IHomePageView
{
    public string ViewId => "home-page";

    public DataDependencies Dependencies => DataDependencies.None;

    public string Render(IDataContext data) =>
        """
        <section id="home-page">
            <h1>Hello Reader</h1>
        </section>
        """;
}
