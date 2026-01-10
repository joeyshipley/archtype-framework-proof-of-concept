namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Link - Navigation link element.
/// Renders as an anchor tag with optional styling variants.
/// </summary>
public record Link : IElement, IHeaderContent, IBodyContent, IFooterContent
{
    private readonly string _label;

    public string Label => _label;
    public string ElementHref { get; init; } = "#";
    public string ElementId { get; init; }
    public LinkStyle ElementStyle { get; init; } = LinkStyle.Default;

    public IEnumerable<IElement> Children => Enumerable.Empty<IElement>();

    public Link(string label)
    {
        _label = label;
    }

    public Link(string label, string href)
    {
        _label = label;
        ElementHref = href;
    }

    // Fluent builder methods

    /// <summary>Sets the href. Returns new instance (immutable).</summary>
    public Link Href(string href) => this with { ElementHref = href };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Link Id(string id) => this with { ElementId = id };

    /// <summary>Sets the style variant. Returns new instance (immutable).</summary>
    public Link Style(LinkStyle style) => this with { ElementStyle = style };
}

/// <summary>
/// Link style variants - controls visual appearance.
/// </summary>
public enum LinkStyle
{
    Default,        // Standard link (text with underline on hover)
    Button,         // Styled like a primary button
    ButtonSecondary,// Styled like a secondary button
    Ghost           // Subtle link (no underline, muted color)
}
