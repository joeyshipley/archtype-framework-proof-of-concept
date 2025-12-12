namespace PagePlay.Site.Infrastructure.UI;

/// <summary>
/// Base marker interface for all UI elements in the Closed-World system.
/// Elements declare WHAT they are, not HOW they look.
/// </summary>
public interface IElement
{
    /// <summary>
    /// The child content of this element.
    /// </summary>
    IEnumerable<IElement> Children { get; }
}

/// <summary>
/// Marker interface for content that can appear in a Header slot.
/// </summary>
public interface IHeaderContent : IElement { }

/// <summary>
/// Marker interface for content that can appear in a Body slot.
/// </summary>
public interface IBodyContent : IElement { }

/// <summary>
/// Marker interface for content that can appear in a Footer slot.
/// </summary>
public interface IFooterContent : IElement { }

/// <summary>
/// Marker interface for content that can appear in form fields.
/// Includes inputs, labels, buttons, and other form elements.
/// </summary>
public interface IFieldContent : IElement { }

/// <summary>
/// Base class for elements with children.
/// Supports collection initializer syntax.
/// </summary>
public abstract record ElementBase : IElement, System.Collections.IEnumerable
{
    private readonly List<IElement> _children = new();

    public IEnumerable<IElement> Children => _children;

    public void Add(IElement element) => _children.Add(element);

    // Required for collection initializer syntax
    public System.Collections.IEnumerator GetEnumerator() => _children.GetEnumerator();
}
