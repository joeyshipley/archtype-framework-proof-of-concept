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
        var fetchTasks = domainsToLoad.Select(domain =>
            domain.FetchAllAsync(userId.Value)
        );
        var results = await Task.WhenAll(fetchTasks);

        // Add to context
        for (int i = 0; i < domainsToLoad.Count; i++)
        {
            dataContext.AddDomain(domainsToLoad[i].Name, results[i]);
        }

        return dataContext;
    }
}
