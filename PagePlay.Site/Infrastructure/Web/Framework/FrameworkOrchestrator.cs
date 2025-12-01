namespace PagePlay.Site.Infrastructure.Web.Framework;

using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Mutations;

/// <summary>
/// Orchestrates framework operations: data loading, component rendering, OOB updates.
/// </summary>
public interface IFrameworkOrchestrator
{
    /// <summary>
    /// Loads data for all components and renders them.
    /// Used for initial page load.
    /// </summary>
    Task<Dictionary<string, string>> RenderComponentsAsync(IEnumerable<IServerComponent> components);

    /// <summary>
    /// Handles mutation response by re-rendering affected components.
    /// Used for interaction responses.
    /// Returns HTML string of OOB updates (empty string if no components affected).
    /// </summary>
    Task<string> RenderMutationResponseAsync(
        DataMutations mutations,
        string? componentContextJson
    );
}

public class FrameworkOrchestrator(
    IDataLoader _dataLoader,
    IComponentContextParser _contextParser,
    IComponentFactory _componentFactory
) : IFrameworkOrchestrator
{
    public async Task<Dictionary<string, string>> RenderComponentsAsync(
        IEnumerable<IServerComponent> components)
    {
        var componentList = components.ToList();

        // 1. Collect all required domain context types
        var requiredContextTypes = componentList
            .Select(c => c.Dependencies.DomainContextType)
            .Where(t => t != null)
            .Distinct()
            .ToArray();

        // 2. Load all domains in parallel
        var dataContext = await buildFluentChain(requiredContextTypes).Load();

        // 3. Render all components
        var renderedComponents = new Dictionary<string, string>();
        foreach (var component in componentList)
        {
            var html = component.Render(dataContext);
            renderedComponents[component.ComponentId] = html;
        }

        return renderedComponents;
    }

    public async Task<string> RenderMutationResponseAsync(
        DataMutations mutations,
        string? componentContextJson)
    {
        // 1. Parse component context from client
        var pageComponents = _contextParser.Parse(componentContextJson);

        // 2. Find components affected by mutation
        var affectedComponents = (mutations != null)
            ? pageComponents
                .Where(c => mutations.Domains.Contains(c.Domain))
                .ToList()
            : [];

        if (affectedComponents.Count == 0)
            return string.Empty; // No components to update

        // 3. Map affected component domain names to context types
        var affectedContextTypes = affectedComponents
            .Select(c =>
            {
                var component = _componentFactory.Create(c.ComponentType);
                return component?.Dependencies.DomainContextType;
            })
            .Where(t => t != null)
            .Distinct()
            .ToArray();

        // 4. Re-fetch mutated domains
        var dataContext = await buildFluentChain(affectedContextTypes).Load();

        // 5. Re-render affected components
        var updates = new List<string>();
        foreach (var componentInfo in affectedComponents)
        {
            var component = _componentFactory.Create(componentInfo.ComponentType);
            if (component == null) continue;

            var html = component.Render(dataContext);

            // Insert hx-swap-oob attribute into the component's root element
            // The component already has id="..." so we just need to add the OOB attribute
            var oobHtml = html.Replace($"id=\"{componentInfo.Id}\"", $"id=\"{componentInfo.Id}\" hx-swap-oob=\"true\"");

            updates.Add(oobHtml);
        }

        return string.Join("\n", updates);
    }

    private IDataLoaderBuilder buildFluentChain(Type[] contextTypes)
    {
        if (contextTypes.Length == 0)
            throw new InvalidOperationException("Cannot build fluent chain with no context types");

        IDataLoaderBuilder builder = null!;

        foreach (var contextType in contextTypes)
        {
            // Reflection to call With<T>() for each type
            var withMethod = builder == null
                ? typeof(IDataLoader).GetMethod("With")
                : typeof(IDataLoaderBuilder).GetMethod("With");

            var genericMethod = withMethod!.MakeGenericMethod(contextType);

            if (builder == null)
            {
                // First call: dataLoader.With<T>()
                builder = (IDataLoaderBuilder)genericMethod.Invoke(_dataLoader, null)!;
            }
            else
            {
                // Subsequent calls: builder.With<T>()
                builder = (IDataLoaderBuilder)genericMethod.Invoke(builder, null)!;
            }
        }

        return builder;
    }
}
