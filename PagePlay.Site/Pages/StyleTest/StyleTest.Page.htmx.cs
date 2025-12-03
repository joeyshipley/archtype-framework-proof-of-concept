using PagePlay.Site.Infrastructure.UI;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.UI.Vocabulary;

namespace PagePlay.Site.Pages.StyleTest;

public interface IStyleTestPageView
{
    string RenderPage();
}

public class StyleTestPage(IHtmlRenderer _renderer) : IStyleTestPageView
{
    public string RenderPage()
    {
        // Demonstrate the Closed-World UI system
        var demoCard = new Card
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
        };

        var featuresCard = new Card
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
        };

        var statesCard = new Card
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
        };

        // language=html
        return $$"""
        <div style="max-width: 1200px; margin: 2rem auto; padding: 0 1rem;">
            <h1>Closed-World UI - Walking Skeleton</h1>
            <p style="margin-bottom: 2rem; color: #525252;">
                A vertical slice demonstrating semantic types → HTML + CSS with zero escape hatches.
            </p>

            <div style="display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));">
                {{_renderer.Render(demoCard)}}
                {{_renderer.Render(featuresCard)}}
                {{_renderer.Render(statesCard)}}
            </div>
        </div>
        """;
    }
}
