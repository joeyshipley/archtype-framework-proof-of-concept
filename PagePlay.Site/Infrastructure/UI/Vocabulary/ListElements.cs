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
    public ListStyle ElementStyle { get; init; } = ListStyle.Unordered;
    public string ElementId { get; init; }

    public List()
    {
    }

    public List(params IComponent[] items)
    {
        foreach (var item in items)
            Add(item);
    }

    // Fluent builder methods

    /// <summary>Sets the list style. Returns new instance (immutable).</summary>
    public List Style(ListStyle style) => this with { ElementStyle = style };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public List Id(string id) => this with { ElementId = id };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public new List Children(params IComponent[] children)
    {
        foreach (var child in children)
            Add(child);
        return this;
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
    public ListItemState ElementState { get; init; } = ListItemState.Normal;
    public string ElementId { get; init; }

    public ListItem()
    {
    }

    public ListItem(params IComponent[] content)
    {
        foreach (var item in content)
            Add(item);
    }

    // Fluent builder methods

    /// <summary>Sets the list item state. Returns new instance (immutable).</summary>
    public ListItem State(ListItemState state) => this with { ElementState = state };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public ListItem Id(string id) => this with { ElementId = id };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public new ListItem Children(params IComponent[] children)
    {
        foreach (var child in children)
            Add(child);
        return this;
    }
}
