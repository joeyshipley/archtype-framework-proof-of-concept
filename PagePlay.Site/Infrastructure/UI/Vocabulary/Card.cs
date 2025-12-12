namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Card - A bounded visual unit for grouping related content.
/// Uses direct content builder pattern - slots hidden as implementation detail.
/// </summary>
public record Card : ElementBase
{
    /// <summary>
    /// Internal header slot storage. Use .Header() builder method to set content.
    /// </summary>
    internal Header _headerSlot { get; init; }

    /// <summary>
    /// Internal body slot storage. Use .Body() builder method to set content.
    /// </summary>
    internal Body _bodySlot { get; init; }

    /// <summary>
    /// Internal footer slot storage. Use .Footer() builder method to set content.
    /// </summary>
    internal Footer _footerSlot { get; init; }

    /// <summary>
    /// Sets header content. Creates Header slot internally. Returns new instance (immutable).
    /// </summary>
    public Card Header(params IHeaderContent[] content)
    {
        var header = new Header(content);
        return this with { _headerSlot = header };
    }

    /// <summary>
    /// Sets body content. Creates Body slot internally. Returns new instance (immutable).
    /// </summary>
    public Card Body(params IBodyContent[] content)
    {
        var body = new Body(content);
        return this with { _bodySlot = body };
    }

    /// <summary>
    /// Sets footer content. Creates Footer slot internally. Returns new instance (immutable).
    /// </summary>
    public Card Footer(params IFooterContent[] content)
    {
        var footer = new Footer(content);
        return this with { _footerSlot = footer };
    }
}
