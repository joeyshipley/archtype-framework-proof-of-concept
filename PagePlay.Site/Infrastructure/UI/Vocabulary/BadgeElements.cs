namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Badge tone - Semantic intent for different badge types.
/// Maps to visual styling for status indicators, counts, and labels.
/// </summary>
public enum BadgeTone
{
    Neutral,   // Default gray/muted
    Accent,    // Brand color (blue)
    Positive,  // Success/green
    Warning,   // Warning/yellow
    Critical   // Error/red
}

/// <summary>
/// Badge size - Controls visual size of the badge.
/// </summary>
public enum BadgeSize
{
    Small,     // Compact, for tight spaces
    Medium     // Default size
}

/// <summary>
/// Badge - Small label for counts, status indicators, and tags.
/// Used in card headers, navigation items, table cells, etc.
/// </summary>
public record Badge : IElement, IHeaderContent, IBodyContent, IFooterContent
{
    private readonly string _label;

    public string Label => _label;
    public BadgeTone ElementTone { get; init; } = BadgeTone.Neutral;
    public BadgeSize ElementSize { get; init; } = BadgeSize.Medium;
    public string ElementId { get; init; }

    public IEnumerable<IElement> Children => Enumerable.Empty<IElement>();

    public Badge(string label)
    {
        _label = label;
    }

    public Badge(string label, BadgeTone tone)
    {
        _label = label;
        ElementTone = tone;
    }

    // Fluent builder methods

    /// <summary>Sets the tone variant. Returns new instance (immutable).</summary>
    public Badge Tone(BadgeTone tone) => this with { ElementTone = tone };

    /// <summary>Sets the size variant. Returns new instance (immutable).</summary>
    public Badge Size(BadgeSize size) => this with { ElementSize = size };

    /// <summary>Sets element ID. Returns new instance (immutable).</summary>
    public Badge Id(string id) => this with { ElementId = id };
}
