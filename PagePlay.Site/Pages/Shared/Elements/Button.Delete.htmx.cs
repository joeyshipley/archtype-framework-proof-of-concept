using RouteData = PagePlay.Site.Pages.Shared.Elements.RouteData;

namespace PagePlay.Site.Pages.Shared.Elements;

public static class ButtonDelete
{
    // language=html
    public static string Render(
        string endpoint,
        long id,
        string tag,
        string target,
        string content,
        string? cssClass = null,
        string? title = null,
        string? swapStrategy = null,
        string? hxExt = null
    )
    {
        return Button.Render(
            route: new RouteData
            {
                Endpoint = endpoint,
                ModelId = id,
                Target = target,
                SwapStrategy = swapStrategy ?? "outerHTML",
                HxExt = hxExt
            },
            html: new HtmlData
            {
                ElementId = $"delete-{tag}-{id}",
                Title = title ?? $"Delete {tag}",
                CssClass = cssClass ?? $"{tag}-delete"
            },
            content: content
        );
    }
}
