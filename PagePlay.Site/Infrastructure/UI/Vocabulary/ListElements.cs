namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// List style - Semantic intent for different list types.
/// Maps to HTML list elements (ul, ol) and styling.
/// </summary>
public enum ListStyle
{
    Unordered,  // Bullets (default) - <ul>
    Ordered,    // Numbers - <ol>
    Plain       // No markers - <ul> without list-style
}

/// <summary>
/// List - Container for list items.
/// Semantic container that enforces type-safe list structure.
/// </summary>
public record List : ComponentBase, IBodyContent
{
    public ListStyle Style { get; init; } = ListStyle.Unordered;
    public string Id { get; init; }

    public List()
    {
    }

    public List(params IComponent[] items)
    {
        foreach (var item in items)
            Add(item);
    }
}

/// <summary>
/// List item state - Semantic state for individual list items.
/// Controls visual styling based on item status (completed, disabled, error).
/// </summary>
public enum ListItemState
{
    Normal,
    Completed,
    Disabled,
    Error
}

/// <summary>
/// ListItem - Individual list item with semantic state.
/// Used within List containers to represent individual items with optional state styling.
/// </summary>
public record ListItem : ComponentBase
{
    public ListItemState State { get; init; } = ListItemState.Normal;
    public string Id { get; init; }

    public ListItem()
    {
    }

    public ListItem(params IComponent[] content)
    {
        foreach (var item in content)
            Add(item);
    }
}
