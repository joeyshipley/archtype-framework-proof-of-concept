namespace PagePlay.Site.Infrastructure.Web.Components;

using PagePlay.Site.Infrastructure.Web.Data;

/// <summary>
/// Server-rendered view that declares data dependencies
/// and renders HTML from pre-loaded data.
/// </summary>
public interface IView
{
    /// <summary>
    /// Unique identifier for this view instance.
    /// Used as DOM element ID and for OOB targeting.
    /// </summary>
    string ViewId { get; }

    /// <summary>
    /// Data dependencies this view needs to render.
    /// </summary>
    DataDependencies Dependencies { get; }

    /// <summary>
    /// Renders HTML using pre-loaded data.
    /// This method must be pure (no I/O, no side effects).
    /// </summary>
    string Render(IDataContext data);
}

/// <summary>
/// Declares which domain(s) a view depends on using strongly-typed domain references.
/// </summary>
public class DataDependencies
{
    public string Domain { get; private set; } = string.Empty;
    public Type DomainContextType { get; private set; }

    /// <summary>
    /// Represents a view with no data dependencies (e.g., static pages like Login).
    /// Use this for pages that don't need to load domain data.
    /// </summary>
    public static readonly DataDependencies None = new()
    {
        Domain = string.Empty,
        DomainContextType = null
    };

    /// <summary>
    /// Creates data dependencies from a domain context type.
    /// Reads the domain name from the TContext.DomainName static field.
    /// </summary>
    /// <typeparam name="TContext">Domain view type that contains a public static DomainName field</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if TContext doesn't have a DomainName field or it's empty</exception>
    public static DataDependencies From<TContext>()
        where TContext : class, new()
    {
        // Read the DomainName constant from TContext
        var domainNameField = typeof(TContext).GetField("DomainName",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);

        if (domainNameField == null)
            throw new InvalidOperationException(
                $"{typeof(TContext).Name} must have a public static DomainName field");

        var domainName = domainNameField.GetValue(null) as string;

        if (string.IsNullOrEmpty(domainName))
            throw new InvalidOperationException(
                $"{typeof(TContext).Name}.DomainName cannot be null or empty");

        return new()
        {
            Domain = domainName,
            DomainContextType = typeof(TContext)
        };
    }
}

/// <summary>
/// Provides access to all loaded domain data via strongly-typed contexts.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Gets typed domain context by context type.
    /// No magic strings - the context type uniquely identifies the domain.
    /// </summary>
    TContext Get<TContext>() where TContext : class;

    /// <summary>
    /// Checks if a domain context has been loaded by type.
    /// </summary>
    bool HasDomain<TContext>() where TContext : class;
}

public class DataContext : IDataContext
{
    private readonly Dictionary<Type, object> _typedDomainsByContextType = new();

    /// <summary>
    /// Creates an empty data context for pages without data dependencies.
    /// Use this for static pages that don't need to load domain data.
    /// </summary>
    public static DataContext Empty() => new();

    public void AddDomain<TContext>(TContext typedData) where TContext : class
    {
        _typedDomainsByContextType[typeof(TContext)] = typedData;
    }

    public TContext Get<TContext>() where TContext : class
    {
        if (!_typedDomainsByContextType.TryGetValue(typeof(TContext), out var data))
            throw new InvalidOperationException($"Domain context '{typeof(TContext).Name}' not loaded");

        return (TContext)data;
    }

    public bool HasDomain<TContext>() where TContext : class =>
        _typedDomainsByContextType.ContainsKey(typeof(TContext));
}
