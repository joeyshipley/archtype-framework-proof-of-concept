namespace PagePlay.Site.Infrastructure.UI.Vocabulary;

/// <summary>
/// TopNav - Horizontal header bar with logo and actions.
/// Renders as a fixed-height header with logo on left, actions on right.
/// Used at the top of dashboard/admin layouts.
/// Implements IBodyContent so it can be shown inside cards for showcase purposes.
/// </summary>
public record TopNav : ElementBase, IElement, IBodyContent
{
    public string ElementId { get; init; }

    // Logo configuration (left side)
    public string ElementLogoText { get; init; }
    public string ElementLogoHref { get; init; } = "/";

    // Actions slot (right side)
    internal TopNavActions _actionsSlot { get; init; }

    public TopNav()
    {
    }

    public TopNav(string logoText)
    {
        ElementLogoText = logoText;
    }

    // Fluent builder methods

    /// <summary>Sets the element ID. Returns new instance (immutable).</summary>
    public TopNav Id(string id) => this with { ElementId = id };

    /// <summary>Sets the logo text and optional href. Returns new instance (immutable).</summary>
    public TopNav Logo(string text, string href = "/") => this with
    {
        ElementLogoText = text,
        ElementLogoHref = href
    };

    /// <summary>Sets the actions slot content (right side). Returns new instance (immutable).</summary>
    public TopNav Actions(params IElement[] content)
    {
        var actions = new TopNavActions();
        foreach (var item in content)
            actions.Add(item);
        return this with { _actionsSlot = actions };
    }
}

/// <summary>
/// TopNavActions - Internal container for right-side action items.
/// </summary>
internal record TopNavActions : ElementBase
{
    public TopNavActions()
    {
    }
}
