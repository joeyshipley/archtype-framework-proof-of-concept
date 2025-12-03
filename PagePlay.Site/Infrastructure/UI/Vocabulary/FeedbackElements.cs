namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Alert tone - Semantic intent for different alert types.
/// Maps to visual styling for success, error, warning, and informational messages.
/// </summary>
public enum AlertTone
{
    Neutral,   // Informational (default)
    Positive,  // Success
    Warning,   // Warning/Caution
    Critical   // Error/Danger
}

/// <summary>
/// Alert - User feedback for errors, success, warnings, and informational messages.
/// Server returns alerts via workflow responses (validation errors, operation results).
/// </summary>
public record Alert : IComponent, IBodyContent
{
    private readonly string _message;

    public string Message => _message;
    public AlertTone ElementTone { get; init; }
    public bool ElementDismissible { get; init; }
    public string ElementId { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public Alert(string message, AlertTone tone = AlertTone.Neutral)
    {
        _message = message;
        ElementTone = tone;
    }

    // Fluent builder methods

    /// <summary>Sets dismissible state. Returns new instance (immutable).</summary>
    public Alert Dismissible(bool dismissible) => this with { ElementDismissible = dismissible };

    /// <summary>Sets element ID. Returns new instance (immutable).</summary>
    public Alert Id(string id) => this with { ElementId = id };
}

/// <summary>
/// Empty state size - Controls visual prominence of empty state messages.
/// </summary>
public enum EmptyStateSize
{
    Small,   // Brief inline message
    Medium,  // Default card-style
    Large    // Full-page with illustration
}

/// <summary>
/// EmptyState - Empty list or no-content messaging.
/// Displayed when data collections are empty (no todos, no search results, etc.).
/// </summary>
public record EmptyState : IComponent, IBodyContent
{
    private readonly string _message;

    public string Message => _message;
    public EmptyStateSize ElementSize { get; init; } = EmptyStateSize.Medium;
    public string ElementActionLabel { get; init; }
    public string ElementActionUrl { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public EmptyState(string message)
    {
        _message = message;
    }

    // Fluent builder methods

    /// <summary>Sets the size variant. Returns new instance (immutable).</summary>
    public EmptyState Size(EmptyStateSize size) => this with { ElementSize = size };

    /// <summary>Sets the action label. Returns new instance (immutable).</summary>
    public EmptyState ActionLabel(string actionLabel) => this with { ElementActionLabel = actionLabel };

    /// <summary>Sets the action URL. Returns new instance (immutable).</summary>
    public EmptyState ActionUrl(string actionUrl) => this with { ElementActionUrl = actionUrl };
}
