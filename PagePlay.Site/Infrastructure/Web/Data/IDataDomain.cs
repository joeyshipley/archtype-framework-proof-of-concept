namespace PagePlay.Site.Infrastructure.Web.Data;

/// <summary>
/// Represents a bounded context that provides related data.
/// Examples: "todos", "accounts", "notifications"
/// </summary>
public interface IDataDomain
{
    /// <summary>
    /// Unique name for this domain (e.g., "todos")
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Fetches ALL data this domain provides in one query.
    /// Called during initial page render and when domain is mutated.
    /// </summary>
    Task<DomainDataContext> FetchAllAsync(long userId);
}

/// <summary>
/// Container for all data a domain provides.
/// Components access data by key: context["openCount"]
/// </summary>
public class DomainDataContext
{
    private readonly Dictionary<string, object> _data = new();

    public object this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }

    public T Get<T>(string key) => (T)_data[key];

    public bool Contains(string key) => _data.ContainsKey(key);
}
