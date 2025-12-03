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
    public bool ElementDisabled { get; init; }
    public bool ElementLoading { get; init; }

    // Button type (submit, reset, button)
    public ButtonType ElementType { get; init; } = ButtonType.Button;

    // Interactive properties (HTMX)
    public string ElementAction { get; init; }
    public string ElementId { get; init; }
    public string ElementTarget { get; init; }
    public SwapStrategy ElementSwap { get; init; } = SwapStrategy.InnerHTML;
    public long? ElementModelId { get; init; }

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
    public Button Disabled(bool disabled) => this with { ElementDisabled = disabled };

    /// <summary>Sets loading state. Returns new instance (immutable).</summary>
    public Button Loading(bool loading) => this with { ElementLoading = loading };

    /// <summary>Sets button type (submit, reset, button). Returns new instance (immutable).</summary>
    public Button Type(ButtonType type) => this with { ElementType = type };

    /// <summary>Sets HTMX action URL. Returns new instance (immutable).</summary>
    public Button Action(string action) => this with { ElementAction = action };

    /// <summary>Sets element ID. Returns new instance (immutable).</summary>
    public Button Id(string id) => this with { ElementId = id };

    /// <summary>Sets HTMX target selector. Returns new instance (immutable).</summary>
    public Button Target(string target) => this with { ElementTarget = target };

    /// <summary>Sets HTMX swap strategy. Returns new instance (immutable).</summary>
    public Button Swap(SwapStrategy swap) => this with { ElementSwap = swap };

    /// <summary>Sets model ID for HTMX requests. Returns new instance (immutable).</summary>
    public Button ModelId(long? modelId) => this with { ElementModelId = modelId };
}
