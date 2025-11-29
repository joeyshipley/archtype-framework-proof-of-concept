namespace PagePlay.Site.Infrastructure.Web.Data;

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
    Task<IDataContext> LoadDomainsAsync(IEnumerable<string> domainNames);
}

public class DataLoader(
    IEnumerable<IDataDomain> _domains,
    IUserIdentityService _userIdentity
) : IDataLoader
{
    public async Task<IDataContext> LoadDomainsAsync(IEnumerable<string> domainNames)
    {
        var userId = _userIdentity.GetCurrentUserId();
        if (!userId.HasValue)
            throw new InvalidOperationException("User must be authenticated to load domain data");

        var domainList = domainNames.Distinct().ToList();
        var dataContext = new DataContext();

        // Find matching domains
        var domainsToLoad = _domains
            .Where(d => domainList.Contains(d.Name))
            .ToList();

        // Fetch all domains in parallel
        foreach (var domain in domainsToLoad)
        {
            // Check if this is a typed domain
            var domainType = domain.GetType();
            var typedInterface = domainType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataDomain<>));

            if (typedInterface != null)
            {
                // Typed domain - fetch both typed and legacy contexts
                var contextType = typedInterface.GetGenericArguments()[0];
                var fetchTypedMethod = domainType.GetMethod("FetchTypedAsync");

                if (fetchTypedMethod != null)
                {
                    var typedTask = (Task)fetchTypedMethod.Invoke(domain, new object[] { userId.Value });
                    await typedTask;
                    var typedResult = typedTask.GetType().GetProperty("Result")?.GetValue(typedTask);

                    if (typedResult != null)
                    {
                        // Add typed context
                        var addTypedMethod = typeof(DataContext).GetMethod("AddTypedDomain")
                            ?.MakeGenericMethod(contextType);
                        addTypedMethod?.Invoke(dataContext, new[] { domain.Name, typedResult });
                    }
                }

                // Also fetch legacy context for backward compatibility
                var legacyContext = await domain.FetchAllAsync(userId.Value);
                dataContext.AddDomain(domain.Name, legacyContext);
            }
            else
            {
                // Legacy domain - only fetch legacy context
                var legacyContext = await domain.FetchAllAsync(userId.Value);
                dataContext.AddDomain(domain.Name, legacyContext);
            }
        }

        return dataContext;
    }
}
