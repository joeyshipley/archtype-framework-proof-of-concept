namespace PagePlay.Site.Infrastructure.Web.Data;

using Microsoft.Extensions.Logging;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.Web.Components;

/// <summary>
/// Loads data for specified data providers in parallel.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Begins fluent provider loading.
    /// Chain .With&lt;TContext&gt;() for each provider, then .Load() to execute.
    /// </summary>
    IDataLoaderBuilder With<TContext>() where TContext : class;

    /// <summary>
    /// Internal method called by builder.
    /// Do not call directly - use fluent API instead.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when user is not authenticated or domain configuration is invalid</exception>
    /// <exception cref="DataLoadException">Thrown when domain data fetching fails</exception>
    Task<IDataContext> GetDataProvidersInternal(params Type[] contextTypes);
}

public class DataLoader(
    IEnumerable<IDataProvider> _domains,
    IUserIdentityService _userIdentity,
    ILogger<DataLoader> _logger
) : IDataLoader
{
    public IDataLoaderBuilder With<TContext>() where TContext : class
    {
        var builder = new DataLoaderBuilder(this);
        return builder.With<TContext>();
    }

    // TODO: clean up veribage, it's no longer domain or domain context anymore.
    public async Task<IDataContext> GetDataProvidersInternal(params Type[] contextTypes)
    {
        var userId = _userIdentity.GetCurrentUserId();
        if (!userId.HasValue)
        {
            _logger.LogError("Attempted to load domain data without authenticated user");
            throw new InvalidOperationException("User must be authenticated to load domain data");
        }

        var contextTypeList = contextTypes.Distinct().ToList();
        var dataContext = new DataContext();

        _logger.LogDebug("Loading {Count} domain contexts for user {UserId}", contextTypeList.Count, userId.Value);

        // Map context types to domains
        var domainsToLoad = new List<(IDataProvider domain, Type contextType)>();

        foreach (var contextType in contextTypeList)
        {
            var domain = findDomainForContextType(contextType);
            if (domain == null)
            {
                _logger.LogError("No domain found that produces context type '{ContextType}'", contextType.Name);
                throw new InvalidOperationException($"No domain found that produces context type '{contextType.Name}'");
            }

            domainsToLoad.Add((domain, contextType));
        }

        // Fetch all typed domains in parallel
        foreach (var (domain, contextType) in domainsToLoad)
        {
            try
            {
                var domainType = domain.GetType();
                var fetchTypedMethod = domainType.GetMethod("FetchTyped");

                if (fetchTypedMethod == null)
                {
                    _logger.LogError("Domain '{DomainType}' does not implement FetchTypedAsync method", domainType.Name);
                    throw new InvalidOperationException($"Domain '{domainType.Name}' does not implement FetchTypedAsync");
                }

                var typedTask = (Task)fetchTypedMethod.Invoke(domain, new object[] { userId.Value });
                await typedTask;
                var typedResult = typedTask.GetType().GetProperty("Result")?.GetValue(typedTask);

                if (typedResult == null)
                {
                    _logger.LogError("Domain '{DomainType}' returned null from FetchTypedAsync for user {UserId}", domainType.Name, userId.Value);
                    throw new DataLoadException($"Domain '{domainType.Name}' returned null from FetchTypedAsync");
                }

                // Add typed context (keyed by context type, not string)
                var addDomainMethod = typeof(DataContext).GetMethod("AddDomain")
                    ?.MakeGenericMethod(contextType);
                addDomainMethod?.Invoke(dataContext, new[] { typedResult });

                _logger.LogDebug("Successfully loaded context '{ContextType}' for user {UserId}", contextType.Name, userId.Value);
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not DataLoadException)
            {
                _logger.LogError(ex, "Failed to load context '{ContextType}' for user {UserId}", contextType.Name, userId.Value);
                throw new DataLoadException($"Failed to load context '{contextType.Name}': {ex.Message}", ex);
            }
        }

        _logger.LogDebug("Successfully loaded {Count} domain contexts for user {UserId}", domainsToLoad.Count, userId.Value);
        return dataContext;
    }

    private IDataProvider findDomainForContextType(Type contextType)
    {
        return _domains.FirstOrDefault(d =>
        {
            var domainType = d.GetType();
            var typedInterface = domainType.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IDataProvider<>) &&
                    i.GetGenericArguments()[0] == contextType);

            return typedInterface != null;
        });
    }
}

/// <summary>
/// Exception thrown when domain data loading fails.
/// </summary>
public class DataLoadException : Exception
{
    public DataLoadException(string message) : base(message) { }
    public DataLoadException(string message, Exception innerException) : base(message, innerException) { }
}
