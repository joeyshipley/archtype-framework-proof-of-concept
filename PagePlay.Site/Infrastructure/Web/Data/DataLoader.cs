namespace PagePlay.Site.Infrastructure.Web.Data;

using Microsoft.Extensions.Logging;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.Web.Components;

/// <summary>
/// Loads data for specified domains in parallel.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Fetches data for all specified domains in parallel.
    /// Returns unified DataContext with all domain data.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when user is not authenticated or domain configuration is invalid</exception>
    /// <exception cref="DataLoadException">Thrown when domain data fetching fails</exception>
    Task<IDataContext> LoadDomainsAsync(IEnumerable<string> domainNames);
}

public class DataLoader(
    IEnumerable<IDataDomain> _domains,
    IUserIdentityService _userIdentity,
    ILogger<DataLoader> _logger
) : IDataLoader
{
    public async Task<IDataContext> LoadDomainsAsync(IEnumerable<string> domainNames)
    {
        var userId = _userIdentity.GetCurrentUserId();
        if (!userId.HasValue)
        {
            _logger.LogError("Attempted to load domain data without authenticated user");
            throw new InvalidOperationException("User must be authenticated to load domain data");
        }

        var domainList = domainNames.Distinct().ToList();
        var dataContext = new DataContext();

        _logger.LogDebug("Loading domains: {Domains} for user {UserId}", string.Join(", ", domainList), userId.Value);

        // Find matching domains
        var domainsToLoad = _domains
            .Where(d => domainList.Contains(d.Name))
            .ToList();

        // Fetch all typed domains in parallel
        foreach (var domain in domainsToLoad)
        {
            try
            {
                var domainType = domain.GetType();
                var typedInterface = domainType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataDomain<>));

                if (typedInterface == null)
                {
                    _logger.LogError("Domain '{DomainName}' does not implement IDataDomain<TContext>", domain.Name);
                    throw new InvalidOperationException($"Domain '{domain.Name}' must implement IDataDomain<TContext>");
                }

                // Fetch typed context
                var contextType = typedInterface.GetGenericArguments()[0];
                var fetchTypedMethod = domainType.GetMethod("FetchTypedAsync");

                if (fetchTypedMethod == null)
                {
                    _logger.LogError("Domain '{DomainName}' does not implement FetchTypedAsync method", domain.Name);
                    throw new InvalidOperationException($"Domain '{domain.Name}' does not implement FetchTypedAsync");
                }

                var typedTask = (Task)fetchTypedMethod.Invoke(domain, new object[] { userId.Value });
                await typedTask;
                var typedResult = typedTask.GetType().GetProperty("Result")?.GetValue(typedTask);

                if (typedResult == null)
                {
                    _logger.LogError("Domain '{DomainName}' returned null from FetchTypedAsync for user {UserId}", domain.Name, userId.Value);
                    throw new DataLoadException($"Domain '{domain.Name}' returned null from FetchTypedAsync");
                }

                // Add typed context
                var addTypedMethod = typeof(DataContext).GetMethod("AddTypedDomain")
                    ?.MakeGenericMethod(contextType);
                addTypedMethod?.Invoke(dataContext, new[] { domain.Name, typedResult });

                _logger.LogDebug("Successfully loaded domain '{DomainName}' for user {UserId}", domain.Name, userId.Value);
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not DataLoadException)
            {
                _logger.LogError(ex, "Failed to load domain '{DomainName}' for user {UserId}", domain.Name, userId.Value);
                throw new DataLoadException($"Failed to load domain '{domain.Name}': {ex.Message}", ex);
            }
        }

        _logger.LogDebug("Successfully loaded {Count} domains for user {UserId}", domainsToLoad.Count, userId.Value);
        return dataContext;
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
