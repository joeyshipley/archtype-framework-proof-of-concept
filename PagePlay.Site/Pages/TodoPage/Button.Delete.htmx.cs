namespace PagePlay.Site.Pages.TodoPage;

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
        string? swapStrategy = null
    )
    {
        return Button.Render(
            route: new RouteData
            {
                Endpoint = endpoint,
                ModelId = id,
                Target = target,
                SwapStrategy = swapStrategy ?? "morph:innerHTML"
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
