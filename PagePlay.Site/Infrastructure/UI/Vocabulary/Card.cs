namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Card - A bounded visual unit for grouping related content.
/// </summary>
public record Card : ComponentBase
{
    /// <summary>
    /// Optional header region for title and metadata.
    /// </summary>
    public Header? Header { get; init; }

    /// <summary>
    /// Required main content area.
    /// </summary>
    public required Body Body { get; init; }

    /// <summary>
    /// Optional footer region for actions.
    /// </summary>
    public Footer? Footer { get; init; }

    public Card()
    {
        // Required property enforcement happens via required keyword
    }
}
