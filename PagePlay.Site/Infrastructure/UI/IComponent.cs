namespace PagePlay.Site.Infrastructure.UI;

/// <summary>
/// Base marker interface for all UI components in the Closed-World system.
/// Components declare WHAT they are, not HOW they look.
/// </summary>
public interface IComponent
{
    /// <summary>
    /// The child content of this component.
    /// </summary>
    IEnumerable<IComponent> Children { get; }
}

/// <summary>
/// Marker interface for content that can appear in a Header slot.
/// </summary>
public interface IHeaderContent : IComponent { }

/// <summary>
/// Marker interface for content that can appear in a Body slot.
/// </summary>
public interface IBodyContent : IComponent { }

/// <summary>
/// Marker interface for content that can appear in a Footer slot.
/// </summary>
public interface IFooterContent : IComponent { }

/// <summary>
/// Base class for components with children.
/// Supports collection initializer syntax.
/// </summary>
public abstract record ComponentBase : IComponent, System.Collections.IEnumerable
{
    private readonly List<IComponent> _children = new();

    public IEnumerable<IComponent> Children => _children;

    public void Add(IComponent component) => _children.Add(component);

    // Required for collection initializer syntax
    public System.Collections.IEnumerator GetEnumerator() => _children.GetEnumerator();
}
