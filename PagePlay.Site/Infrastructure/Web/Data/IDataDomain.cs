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
}

/// <summary>
/// Typed interface for domains that provide strongly-typed context.
/// All domains must implement this with their specific context type.
/// </summary>
/// <typeparam name="TContext">The typed context this domain provides</typeparam>
public interface IDataDomain<TContext> : IDataDomain where TContext : class, new()
{
    /// <summary>
    /// Fetches domain data as a strongly-typed context.
    /// Called during initial page render and when domain is mutated.
    /// </summary>
    Task<TContext> FetchTypedAsync(long userId);
}
