using PagePlay.Site.Infrastructure.UI;
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
            new PageTitle("PagePlay"),
            new Grid(For.Cards, Columns.Two,
                new Card()
                    .Header(
                        new Text("Card with Header & Footer")
                    )
                    .Body(
                        new Text("This card demonstrates the Flowbite-style border approach."),
                        new Text("Notice the subtle borders between header, body, and footer.")
                    )
                    .Footer(
                        new Button(Importance.Secondary, "Cancel"),
                        new Button(Importance.Primary, "Save")
                    ),
                new Card()
                    .Header(
                        new Text("Card with Header Only")
                    )
                    .Body(
                        new Text("This card has a header but no footer."),
                        new Text("The header border separates it from the body content.")
                    ),
                new Card()
                    .Body(
                        new Text("Card without Header or Footer"),
                        new Text("A simple content-only card with just a border around it.")
                    ),
                new Card()
                    .Body(
                        new Text("Another simple card to show the grid layout.")
                    )
                    .Footer(
                        new Button(Importance.Primary, "Action")
                    )
            )
        ).Id(ViewId);

        return _renderer.Render(page);
    }
}
