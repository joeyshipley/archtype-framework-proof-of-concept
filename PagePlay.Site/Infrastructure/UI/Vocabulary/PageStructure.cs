namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Page - Root container for a complete view.
/// Provides standard page wrapper and max-width constraint.
/// </summary>
public record Page : ElementBase
{
    public string ElementId { get; init; }

    public Page() { }

    public Page(params IElement[] content)
    {
        foreach (var item in content)
            Add(item);
    }

    // Fluent builder methods

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Page Id(string id) => this with { ElementId = id };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public new Page Children(params IElement[] children)
    {
        foreach (var child in children)
            Add(child);
        return this;
    }
}

/// <summary>
/// Section - Major division within a page.
/// Groups related content with appropriate spacing.
/// Implements IDropZone to support drag-and-drop interactions.
/// </summary>
public record Section : ElementBase, IBodyContent, IDropZone
{
    public string ElementId { get; init; }
    public string DropZoneName { get; init; }

    public Section() { }

    public Section(params IElement[] content)
    {
        foreach (var item in content)
            Add(item);
    }

    // Fluent builder methods

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Section Id(string id) => this with { ElementId = id };

    /// <summary>Makes this section a drop zone with the specified name. Returns new instance (immutable).</summary>
    public Section DropZone(string name) => this with { DropZoneName = name };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public new Section Children(params IElement[] children)
    {
        foreach (var child in children)
            Add(child);
        return this;
    }
}

/// <summary>
/// PageTitle - H1-level page identity.
/// Appears once per page, establishes primary context.
/// </summary>
public record PageTitle : IElement, IBodyContent
{
    private readonly string _title;

    public IEnumerable<IElement> Children => Enumerable.Empty<IElement>();

    public string Title => _title;

    public PageTitle(string title)
    {
        _title = title;
    }
}

/// <summary>
/// SectionTitle - H2-level section identity.
/// Establishes hierarchy within page sections.
/// </summary>
public record SectionTitle : IElement, IBodyContent
{
    private readonly string _title;

    public IEnumerable<IElement> Children => Enumerable.Empty<IElement>();

    public string Title => _title;

    public SectionTitle(string title)
    {
        _title = title;
    }
}
