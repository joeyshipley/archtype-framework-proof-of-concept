namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Visual and behavioral importance of a button.
/// Maps to theme-controlled appearance.
/// </summary>
public enum Importance
{
    Primary,
    Secondary,
    Tertiary,
    Ghost
}

/// <summary>
/// HTMX swap strategy - how content should be swapped in the DOM.
/// Maps to hx-swap attribute values.
/// </summary>
public enum SwapStrategy
{
    InnerHTML,
    OuterHTML,
    BeforeBegin,
    AfterBegin,
    BeforeEnd,
    AfterEnd
}

/// <summary>
/// Button - Interactive element for user actions.
/// Can appear in Header or Footer slots.
/// </summary>
public record Button : IHeaderContent, IFooterContent
{
    private readonly string _label;

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    // Visual properties
    public Importance Importance { get; }
    public string Label => _label;
    public bool Disabled { get; init; }
    public bool Loading { get; init; }

    // Interactive properties (HTMX)
    public string? Action { get; init; }
    public string? Id { get; init; }
    public string? Target { get; init; }
    public SwapStrategy Swap { get; init; } = SwapStrategy.InnerHTML;
    public long? ModelId { get; init; }

    public Button(Importance importance, string label)
    {
        Importance = importance;
        _label = label;
    }

    // Convenience: default to Secondary importance
    public Button(string label) : this(Importance.Secondary, label)
    {
    }
}
