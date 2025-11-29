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
/// Declares which domain(s) a component depends on.
/// Supports both legacy string-based and typed domain access.
/// </summary>
public class DataDependencies
{
    public string Domain { get; private set; } = string.Empty;
    public List<string> RequiredKeys { get; private set; } = new();
    public Type? DomainContextType { get; private set; }

    // Legacy string-based API (backward compatible)
    public static DataDependencies From(string domain) => new() { Domain = domain };

    public DataDependencies Require<T>(string key)
    {
        RequiredKeys.Add(key);
        return this;
    }

    // New typed API
    public static DataDependencies From<TDomain, TContext>()
        where TDomain : IDataDomain<TContext>
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
/// Provides access to all loaded domain data.
/// Supports both legacy string-based and typed domain access.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Gets data from a specific domain by key (legacy API).
    /// </summary>
    T Get<T>(string domain, string key);

    /// <summary>
    /// Gets typed domain context (new API).
    /// </summary>
    TContext GetDomain<TContext>(string domainName) where TContext : class;

    /// <summary>
    /// Checks if a domain has been loaded.
    /// </summary>
    bool HasDomain(string domain);
}

public class DataContext : IDataContext
{
    private readonly Dictionary<string, DomainDataContext> _domains = new();
    private readonly Dictionary<string, object> _typedDomains = new();

    public void AddDomain(string domainName, DomainDataContext domainData)
    {
        _domains[domainName] = domainData;
    }

    public void AddTypedDomain<TContext>(string domainName, TContext typedData) where TContext : class
    {
        _typedDomains[domainName] = typedData;
    }

    public T Get<T>(string domain, string key)
    {
        if (!_domains.ContainsKey(domain))
            throw new InvalidOperationException($"Domain '{domain}' not loaded");

        return _domains[domain].Get<T>(key);
    }

    public TContext GetDomain<TContext>(string domainName) where TContext : class
    {
        if (!_typedDomains.ContainsKey(domainName))
            throw new InvalidOperationException($"Typed domain '{domainName}' not loaded");

        return (TContext)_typedDomains[domainName];
    }

    public bool HasDomain(string domain) => _domains.ContainsKey(domain) || _typedDomains.ContainsKey(domain);
}
