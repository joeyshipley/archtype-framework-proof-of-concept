namespace PagePlay.Site.Infrastructure.Web.Data;

/// <summary>
/// Non-generic interface for domain registration and discovery.
/// All domains must implement this for DI container scanning.
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
/// Typed interface for domains that provide strongly-typed context.
/// Enables compile-time safety for domain data access.
/// </summary>
/// <typeparam name="TContext">The typed context this domain provides</typeparam>
public interface IDataDomain<TContext> : IDataDomain where TContext : class, new()
{
    /// <summary>
    /// Fetches domain data as a strongly-typed context.
    /// Replaces dictionary-based access with type-safe properties.
    /// </summary>
    Task<TContext> FetchTypedAsync(long userId);
}

/// <summary>
/// Container for all data a domain provides (legacy string-based access).
/// For new code, prefer IDataDomain&lt;TContext&gt; with typed contexts.
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
