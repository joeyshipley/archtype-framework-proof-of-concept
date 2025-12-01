namespace PagePlay.Site.Infrastructure.Web.Components;

using PagePlay.Site.Infrastructure.Web.Data;

/// <summary>
/// Server-rendered component that declares data dependencies
/// and renders HTML from pre-loaded data.
/// </summary>
public interface IServerComponent
{
    /// <summary>
    /// Unique identifier for this component instance.
    /// Used as DOM element ID and for OOB targeting.
    /// </summary>
    string ComponentId { get; }

    /// <summary>
    /// Data dependencies this component needs to render.
    /// </summary>
    DataDependencies Dependencies { get; }

    /// <summary>
    /// Renders HTML using pre-loaded data.
    /// This method must be pure (no I/O, no side effects).
    /// </summary>
    string Render(IDataContext data);
}

/// <summary>
/// Declares which domain(s) a component depends on using strongly-typed domain references.
/// </summary>
public class DataDependencies
{
    public string Domain { get; private set; } = string.Empty;
    public Type? DomainContextType { get; private set; }

    public static DataDependencies From<TDomain, TContext>()
        where TDomain : IDataProvider<TContext>
        where TContext : class, new()
    {
        // Use the domain type name as the domain identifier
        // Domain classes should match their Name property (e.g., TodosDomain.Name = "todos")
        var domainTypeName = typeof(TDomain).Name;

        // Convert "TodosDomain" -> "todos" (remove "Domain" suffix and lowercase first char)
        var domainName = domainTypeName.EndsWith("Domain")
            ? domainTypeName.Substring(0, domainTypeName.Length - 6)
            : domainTypeName;
        domainName = char.ToLower(domainName[0]) + domainName.Substring(1);

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
