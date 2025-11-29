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
/// </summary>
public class DataDependencies
{
    public string Domain { get; private set; } = string.Empty;
    public List<string> RequiredKeys { get; private set; } = new();

    public static DataDependencies From(string domain) => new() { Domain = domain };

    public DataDependencies Require<T>(string key)
    {
        RequiredKeys.Add(key);
        return this;
    }
}

/// <summary>
/// Provides access to all loaded domain data.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Gets data from a specific domain.
    /// </summary>
    T Get<T>(string domain, string key);

    /// <summary>
    /// Checks if a domain has been loaded.
    /// </summary>
    bool HasDomain(string domain);
}

public class DataContext : IDataContext
{
    private readonly Dictionary<string, DomainDataContext> _domains = new();

    public void AddDomain(string domainName, DomainDataContext domainData)
    {
        _domains[domainName] = domainData;
    }

    public T Get<T>(string domain, string key)
    {
        if (!_domains.ContainsKey(domain))
            throw new InvalidOperationException($"Domain '{domain}' not loaded");

        return _domains[domain].Get<T>(key);
    }

    public bool HasDomain(string domain) => _domains.ContainsKey(domain);
}
