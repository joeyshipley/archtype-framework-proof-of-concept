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
    IServerComponent Create(string componentTypeName);
}

public class ComponentFactory(IServiceScopeFactory _serviceScopeFactory) : IComponentFactory
{
    // Auto-discover all IServerComponent concrete classes at startup
    private static readonly Dictionary<string, Type> _componentTypes = discoverComponents();

    private static Dictionary<string, Type> discoverComponents()
    {
        return typeof(IServerComponent).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IServerComponent).IsAssignableFrom(t))
            .ToDictionary(
                t => t.Name, // Use class name directly: "TodosPage", "WelcomeWidget"
                t => t
            );
    }

    public IServerComponent Create(string componentTypeName)
    {
        if (!_componentTypes.TryGetValue(componentTypeName, out var componentType))
            return null;

        // Create a new scope to resolve the component
        using var scope = _serviceScopeFactory.CreateScope();
        var instance = scope.ServiceProvider.GetService(componentType);
        return instance as IServerComponent;
    }
}
