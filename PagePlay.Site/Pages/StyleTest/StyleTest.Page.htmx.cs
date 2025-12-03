using PagePlay.Site.Infrastructure.Web.Html;

namespace PagePlay.Site.Pages.StyleTest;

public interface IStyleTestPageView
{
    string RenderPage();
    string RenderError(string error);
    string RenderSuccess(string message);
}

public class StyleTestPage : IStyleTestPageView
{
    // language=html
    public string RenderPage() =>
    $$"""
    <div>
        <h1>Style Test</h1>
        {{RenderSomeComponentLikeThing()}}
    </div>
    """;

    // language=html
    public string RenderSomeComponentLikeThing() =>
    $$"""
    <div>
        <h2>Component-Like Thing</h2>
        <p>This is a component-like thing inside the style test page.</p>
    </div>
    """;

    // language=html
    public string RenderError(string error) =>
    $$"""
    <div class="error" role="alert">
        {{error.Safe()}}
    </div>
    """;

    // language=html
    public string RenderSuccess(string message) =>
    $$"""
    <div class="success" role="alert">
        {{message.Safe()}}
    </div>
    """;
}
