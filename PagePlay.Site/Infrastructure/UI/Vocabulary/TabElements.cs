namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// Tab style - Visual variant for tab appearance.
/// Maps to theme-controlled styling for different tab presentations.
/// </summary>
public enum TabStyle
{
    Underline,  // Default: active tab has colored underline (Flowbite common)
    Boxed,      // Active tab has bordered box around it
    Pill        // Active tab has pill-shaped background
}

/// <summary>
/// Tabs - Container for tabbed content with server-authoritative state.
/// The active tab is determined server-side, not client-side.
/// Supports both static rendering and HTMX tab switching.
/// </summary>
public record Tabs : ElementBase, IBodyContent
{
    public TabStyle ElementStyle { get; init; } = TabStyle.Underline;
    public string ElementId { get; init; }

    // Internal storage for tabs
    internal List<Tab> _tabs { get; init; } = new();

    public Tabs()
    {
    }

    public Tabs(params Tab[] tabs)
    {
        _tabs = tabs.ToList();
    }

    // Fluent builder methods

    /// <summary>Sets the tab style variant. Returns new instance (immutable).</summary>
    public Tabs Style(TabStyle style) => this with { ElementStyle = style };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Tabs Id(string id) => this with { ElementId = id };

    /// <summary>Adds a tab. Returns new instance (immutable).</summary>
    public Tabs Tab(Tab tab) => this with { _tabs = _tabs.Append(tab).ToList() };
}

/// <summary>
/// Tab - Individual tab with trigger (label) and content panel.
/// Active state is determined by the server, ensuring single source of truth.
/// </summary>
public record Tab : IElement
{
    private readonly string _label;

    public string Label => _label;
    public bool ElementActive { get; init; }
    public string ElementId { get; init; }

    // HTMX support for server-driven tab switching
    public string ElementAction { get; init; }
    public string ElementTarget { get; init; }
    public SwapStrategy ElementSwap { get; init; } = SwapStrategy.OuterHTML;

    // Content slot - what appears in the tab panel
    internal TabContent _content { get; init; }

    public IEnumerable<IElement> Children => _content?.Children ?? Enumerable.Empty<IElement>();

    public Tab(string label)
    {
        _label = label;
    }

    // Fluent builder methods

    /// <summary>Sets the active state. Returns new instance (immutable).</summary>
    public Tab Active(bool active = true) => this with { ElementActive = active };

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public Tab Id(string id) => this with { ElementId = id };

    /// <summary>Sets the HTMX action URL for tab switching. Returns new instance (immutable).</summary>
    public Tab Action(string action) => this with { ElementAction = action };

    /// <summary>Sets the HTMX target selector. Returns new instance (immutable).</summary>
    public Tab Target(string target) => this with { ElementTarget = target };

    /// <summary>Sets the HTMX swap strategy. Returns new instance (immutable).</summary>
    public Tab Swap(SwapStrategy swap) => this with { ElementSwap = swap };

    /// <summary>Sets the tab content. Returns new instance (immutable).</summary>
    public Tab Content(params IBodyContent[] content)
    {
        var tabContent = new TabContent(content);
        return this with { _content = tabContent };
    }
}

/// <summary>
/// TabContent - Internal content container for tab panels.
/// </summary>
internal record TabContent : ElementBase
{
    public TabContent()
    {
    }

    public TabContent(params IBodyContent[] content)
    {
        foreach (var item in content)
            Add(item);
    }
}
