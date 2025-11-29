namespace PagePlay.Site.Infrastructure.Web.Components;

using PagePlay.Site.Pages.Shared;

/// <summary>
/// Creates component instances from type names.
/// Used when re-rendering components from client context.
/// </summary>
public interface IComponentFactory
{
    /// <summary>
    /// Creates component instance by type name.
    /// Returns null if type not found or not a valid component.
    /// </summary>
    IServerComponent? Create(string componentTypeName);
}

public class ComponentFactory(IServiceScopeFactory _serviceScopeFactory) : IComponentFactory
{
    private static readonly Dictionary<string, Type> _componentTypes = new()
    {
        ["WelcomeWidget"] = typeof(IWelcomeWidget),
        ["AnalyticsStatsWidget"] = typeof(IAnalyticsStatsWidget)
    };

    public IServerComponent? Create(string componentTypeName)
    {
        if (!_componentTypes.TryGetValue(componentTypeName, out var componentType))
            return null;

        // Create a new scope to resolve the component
        using var scope = _serviceScopeFactory.CreateScope();
        var instance = scope.ServiceProvider.GetService(componentType);
        return instance as IServerComponent;
    }
}
