namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Page - Root container for a complete view.
/// Provides standard page wrapper and max-width constraint.
/// </summary>
public record Page : ComponentBase
{
    public string Id { get; init; }

    public Page() { }

    public Page(params IComponent[] content)
    {
        foreach (var item in content)
            Add(item);
    }
}

/// <summary>
/// Section - Major division within a page.
/// Groups related content with appropriate spacing.
/// </summary>
public record Section : ComponentBase, IBodyContent
{
    public string Id { get; init; }

    public Section() { }

    public Section(params IComponent[] content)
    {
        foreach (var item in content)
            Add(item);
    }

    // Fluent builder methods

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Section WithId(string id) => this with { Id = id };

    /// <summary>Adds child components. Returns this instance (mutable for children).</summary>
    public Section WithChildren(params IComponent[] children)
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
public record PageTitle : IComponent, IBodyContent
{
    private readonly string _title;

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

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
public record SectionTitle : IComponent, IBodyContent
{
    private readonly string _title;

    public IEnumerable<IComponent> Children => Enumerable.Empty<IComponent>();

    public string Title => _title;

    public SectionTitle(string title)
    {
        _title = title;
    }
}
