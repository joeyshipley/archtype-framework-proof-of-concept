namespace PagePlay.Site.Infrastructure.Web.Data;

using PagePlay.Site.Infrastructure.Web.Components;

/// <summary>
/// Builder for fluent domain loading.
/// Collects context types, then loads all domains in parallel.
/// </summary>
public interface IDataLoaderBuilder
{
    /// <summary>
    /// Adds another domain context to the load operation.
    /// </summary>
    IDataLoaderBuilder With<TContext>() where TContext : class;

    /// <summary>
    /// Executes the load operation for all specified domains in parallel.
    /// Returns unified IDataContext with all domain data.
    /// </summary>
    Task<IDataContext> Load();
}

public class DataLoaderBuilder(IDataLoader _dataLoader) : IDataLoaderBuilder
{
    private readonly List<Type> _contextTypes = new();

    public IDataLoaderBuilder With<TContext>() where TContext : class
    {
        _contextTypes.Add(typeof(TContext));
        return this;
    }

    public async Task<IDataContext> Load()
    {
        // Delegate to actual data loader
        return await _dataLoader.GetDataProvidersInternal(_contextTypes.ToArray());
    }
}
