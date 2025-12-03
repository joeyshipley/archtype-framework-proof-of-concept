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
/// Button - Interactive element for user actions.
/// Can appear in Header or Footer slots.
/// </summary>
public record Button : IHeaderContent, IFooterContent
{
    private readonly string _label;

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public Importance Importance { get; }
    public string Label => _label;
    public bool Disabled { get; init; }
    public bool Loading { get; init; }

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
