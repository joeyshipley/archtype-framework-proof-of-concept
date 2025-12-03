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
    AfterEnd,
    None
}

/// <summary>
/// Button type - Semantic button purpose.
/// Maps to HTML button type attribute.
/// </summary>
public enum ButtonType
{
    Button,   // type="button" (default)
    Submit,   // type="submit"
    Reset     // type="reset"
}

/// <summary>
/// Button - Interactive element for user actions.
/// Can appear in Header, Footer, or Field slots.
/// </summary>
public record Button : IHeaderContent, IFooterContent, IFieldContent
{
    private readonly string _label;

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    // Visual properties
    public Importance Importance { get; }
    public string Label => _label;
    public bool Disabled { get; init; }
    public bool Loading { get; init; }

    // Button type (submit, reset, button)
    public ButtonType Type { get; init; } = ButtonType.Button;

    // Interactive properties (HTMX)
    public string Action { get; init; }
    public string Id { get; init; }
    public string Target { get; init; }
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

    // Fluent builder methods

    /// <summary>Sets disabled state. Returns new instance (immutable).</summary>
    public Button WithDisabled(bool disabled) => this with { Disabled = disabled };

    /// <summary>Sets loading state. Returns new instance (immutable).</summary>
    public Button WithLoading(bool loading) => this with { Loading = loading };

    /// <summary>Sets button type (submit, reset, button). Returns new instance (immutable).</summary>
    public Button WithType(ButtonType type) => this with { Type = type };

    /// <summary>Sets HTMX action URL. Returns new instance (immutable).</summary>
    public Button WithAction(string action) => this with { Action = action };

    /// <summary>Sets element ID. Returns new instance (immutable).</summary>
    public Button WithId(string id) => this with { Id = id };

    /// <summary>Sets HTMX target selector. Returns new instance (immutable).</summary>
    public Button WithTarget(string target) => this with { Target = target };

    /// <summary>Sets HTMX swap strategy. Returns new instance (immutable).</summary>
    public Button WithSwap(SwapStrategy swap) => this with { Swap = swap };

    /// <summary>Sets model ID for HTMX requests. Returns new instance (immutable).</summary>
    public Button WithModelId(long? modelId) => this with { ModelId = modelId };
}
