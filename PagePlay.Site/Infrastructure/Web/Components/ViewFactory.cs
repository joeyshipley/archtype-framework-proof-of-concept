namespace PagePlay.Site.Infrastructure.Web.Components;

using PagePlay.Site.Pages.Shared;

/// <summary>
/// Creates view instances from type names.
/// Used when re-rendering views from client context.
/// </summary>
public interface IViewFactory
{
    /// <summary>
    /// Creates view instance by type name.
    /// Returns null if type not found or not a valid view.
    /// </summary>
    IView Create(string viewTypeName);
}

public class ViewFactory(IServiceScopeFactory _serviceScopeFactory) : IViewFactory
{
    // Auto-discover all IView concrete classes at startup
    private static readonly Dictionary<string, Type> _viewTypes = discoverViews();

    private static Dictionary<string, Type> discoverViews()
    {
        return typeof(IView).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IView).IsAssignableFrom(t))
            .ToDictionary(
                t => t.Name, // Use class name directly: "TodosPage", "WelcomeWidget"
                t => t
            );
    }

    public IView Create(string viewTypeName)
    {
        if (!_viewTypes.TryGetValue(viewTypeName, out var viewType))
            return null;

        // Create a new scope to resolve the view
        using var scope = _serviceScopeFactory.CreateScope();
        var instance = scope.ServiceProvider.GetService(viewType);
        return instance as IView;
    }
}
