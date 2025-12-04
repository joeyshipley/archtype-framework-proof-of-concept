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
                new Card()
                    .Header(
                        new Text("Closed-World UI Demo")
                    )
                    .Body(
                        new Text("This card is built with semantic types. Developers declare WHAT things are, not HOW they look."),
                        new Text("No className. No inline styles. No escape hatches.")
                    )
                    .Footer(
                        new Button(Importance.Secondary, "Learn More"),
                        new Button(Importance.Primary, "Get Started")
                    ),
                new Card()
                    .Header(
                        new Text("Key Features")
                    )
                    .Body(
                        new Text("✓ Type system enforces validity"),
                        new Text("✓ Theme controls all appearance"),
                        new Text("✓ Invalid output is impossible"),
                        new Text("✓ Designer-owned styling")
                    ),
                new Card()
                    .Header(
                        new Text("Button States")
                    )
                    .Body(
                        new Text("All button importance levels:")
                    )
                    .Footer(
                        new Button(Importance.Primary, "Primary"),
                        new Button(Importance.Secondary, "Secondary"),
                        new Button(Importance.Tertiary, "Tertiary"),
                        new Button(Importance.Ghost, "Ghost")
                    ),
                new Card()
                    .Header(
                        new Text("Interactive Button Test")
                    )
                    .Body(
                        new Text("Click the button to get a random number:")
                    )
                    .Footer(
                        new Button(Importance.Primary, "Get Random Number")
                            .Action("/interaction/style-test/random")
                            .Id("random-number-button")
                            .ModelId(0)
                    ),
                // Result container for OOB updates
                new Card()
                    .Header(
                        new Text("Result")
                    )
                    .Body(
                        new Section()
                            .Id("random-result")
                            .Children(
                                new Text("Click the button above to see a random number appear here.")
                            )
                    )
            )
        };

        return _renderer.Render(page);
    }

    public string RenderRandomNumber(int number) =>
        _renderer.Render(
            new Section()
                .Id("random-result")
                .Children(
                    new Text($"Random Number: {number}")
                )
        );

    public string RenderError(string message) =>
        _renderer.Render(
            new Section()
                .Id("random-result")
                .Children(
                    new Alert($"Error: {message}", AlertTone.Critical)
                )
        );
}
