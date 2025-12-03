using PagePlay.Site.Infrastructure.UI;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;

namespace PagePlay.Site.Pages.StyleTest;

public interface IStyleTestPageView
{
    string RenderPage();
    string RenderRandomNumber(int number);
    string RenderError(string message);
}

public class StyleTestPage(IHtmlRenderer _renderer) : IStyleTestPageView
{
    public string RenderPage()
    {
        // Build the entire page using semantic types - no inline styles
        var page = new Page
        {
            new PageTitle("Closed-World UI - Walking Skeleton"),
            new Stack(For.Content,
                new Text("A vertical slice demonstrating semantic types → HTML + CSS with zero escape hatches.")
            ),
            new Grid(For.Cards, Columns.Auto,
                new Card
                {
                    Header = new Header(
                        new Text("Closed-World UI Demo")
                    ),
                    Body = new Body(
                        new Text("This card is built with semantic types. Developers declare WHAT things are, not HOW they look."),
                        new Text("No className. No inline styles. No escape hatches.")
                    ),
                    Footer = new Footer(
                        new Button(Importance.Secondary, "Learn More"),
                        new Button(Importance.Primary, "Get Started")
                    )
                },
                new Card
                {
                    Header = new Header(
                        new Text("Key Features")
                    ),
                    Body = new Body(
                        new Text("✓ Type system enforces validity"),
                        new Text("✓ Theme controls all appearance"),
                        new Text("✓ Invalid output is impossible"),
                        new Text("✓ Designer-owned styling")
                    )
                },
                new Card
                {
                    Header = new Header(
                        new Text("Button States")
                    ),
                    Body = new Body(
                        new Text("All button importance levels:")
                    ),
                    Footer = new Footer(
                        new Button(Importance.Primary, "Primary"),
                        new Button(Importance.Secondary, "Secondary"),
                        new Button(Importance.Tertiary, "Tertiary"),
                        new Button(Importance.Ghost, "Ghost")
                    )
                },
                new Card
                {
                    Header = new Header(
                        new Text("Interactive Button Test")
                    ),
                    Body = new Body(
                        new Text("Click the button to get a random number:")
                    ),
                    Footer = new Footer(
                        new Button(Importance.Primary, "Get Random Number")
                        {
                            Action = "/interaction/style-test/random",
                            Id = "random-number-button",
                            Target = "#random-result",
                            ModelId = 0
                        }
                    )
                }
            )
        };

        var pageHtml = _renderer.Render(page);

        // Add result container (not in vocabulary yet, so raw HTML for now)
        return pageHtml + "\n<div id=\"random-result\"></div>";
    }

    public string RenderRandomNumber(int number)
    {
        var content = new Stack(For.Content,
            new Text($"Random Number: {number}")
        );
        return _renderer.Render(content);
    }

    public string RenderError(string message)
    {
        var content = new Stack(For.Content,
            new Text($"Error: {message}")
        );
        return _renderer.Render(content);
    }
}
