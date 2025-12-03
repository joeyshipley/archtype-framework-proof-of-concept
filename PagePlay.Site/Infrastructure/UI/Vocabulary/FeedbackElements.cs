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
    public AlertTone Tone { get; init; }
    public bool Dismissible { get; init; }
    public string Id { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public Alert(string message, AlertTone tone = AlertTone.Neutral)
    {
        _message = message;
        Tone = tone;
    }

    // Fluent builder methods

    /// <summary>Sets dismissible state. Returns new instance (immutable).</summary>
    public Alert WithDismissible(bool dismissible) => this with { Dismissible = dismissible };

    /// <summary>Sets element ID. Returns new instance (immutable).</summary>
    public Alert WithId(string id) => this with { Id = id };
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
    public EmptyStateSize Size { get; init; } = EmptyStateSize.Medium;
    public string ActionLabel { get; init; }
    public string ActionUrl { get; init; }

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public EmptyState(string message)
    {
        _message = message;
    }
}
