namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Semantic purpose for layout spacing.
/// Maps to theme-controlled gap values - developers declare intent, not pixels.
/// </summary>
public enum For
{
    /// <summary>Tight grouping for related actions (buttons, toolbar items)</summary>
    Actions,

    /// <summary>Comfortable spacing for form fields</summary>
    Fields,

    /// <summary>Readable flow for paragraphs and prose</summary>
    Content,

    /// <summary>Consistent rhythm for list items and repeated elements</summary>
    Items,

    /// <summary>Generous separation for major page divisions</summary>
    Sections,

    /// <summary>Minimal gap for icon+label pairs</summary>
    Inline,

    /// <summary>Card grid layout spacing</summary>
    Cards
}

/// <summary>
/// Grid column count - semantic options, not arbitrary numbers.
/// </summary>
public enum Columns
{
    One,
    Two,
    Three,
    Four,
    /// <summary>Responsive: as many columns as will fit</summary>
    Auto
}

/// <summary>
/// Stack - Vertical arrangement with semantic purpose-based spacing.
/// </summary>
public record Stack : ComponentBase, IBodyContent
{
    public For Purpose { get; }

    public Stack(For purpose)
    {
        Purpose = purpose;
    }

    public Stack(For purpose, params IComponent[] content) : this(purpose)
    {
        foreach (var item in content)
            Add(item);
    }

    // Fluent builder methods

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public Stack WithChildren(params IComponent[] children)
    {
        foreach (var child in children)
            Add(child);
        return this;
    }
}

/// <summary>
/// Row - Horizontal arrangement with semantic purpose-based spacing.
/// </summary>
public record Row : ComponentBase, IBodyContent, IFooterContent
{
    public For Purpose { get; }

    public Row(For purpose)
    {
        Purpose = purpose;
    }

    public Row(For purpose, params IComponent[] content) : this(purpose)
    {
        foreach (var item in content)
            Add(item);
    }
}

/// <summary>
/// Grid - Two-dimensional layout with semantic purpose and column count.
/// </summary>
public record Grid : ComponentBase, IBodyContent
{
    public For Purpose { get; }
    public Columns Columns { get; }

    public Grid(For purpose, Columns columns = Columns.Auto)
    {
        Purpose = purpose;
        Columns = columns;
    }

    public Grid(For purpose, Columns columns, params IComponent[] content) : this(purpose, columns)
    {
        foreach (var item in content)
            Add(item);
    }
}
