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
        string componentContextJson
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

        // 1. Collect all required domain context types (excluding None dependencies)
        var requiredContextTypes = componentList
            .Where(c => c.Dependencies != DataDependencies.None)
            .Select(c => c.Dependencies.DomainContextType)
            .Where(t => t != null)
            .Distinct()
            .ToArray();

        // 2. Load all domains in parallel (or create empty context if no domains needed)
        var dataContext = requiredContextTypes.Length > 0
            ? await buildFluentChain(requiredContextTypes).Load()
            : DataContext.Empty();

        // 3. Render all components with automatic metadata injection
        var renderedComponents = new Dictionary<string, string>();
        foreach (var component in componentList)
        {
            var html = renderComponentWithMetadata(component, dataContext);
            renderedComponents[component.ComponentId] = html;
        }

        return renderedComponents;
    }

    public async Task<string> RenderMutationResponseAsync(
        DataMutations mutations,
        string componentContextJson)
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

        // 4. Re-fetch mutated domains (or create empty context if no domains)
        var dataContext = affectedContextTypes.Length > 0
            ? await buildFluentChain(affectedContextTypes).Load()
            : DataContext.Empty();

        // 5. Re-render affected components with OOB attribute
        var updates = new List<string>();
        foreach (var componentInfo in affectedComponents)
        {
            var component = _componentFactory.Create(componentInfo.ComponentType);
            if (component == null) continue;

            // Render raw component HTML first
            var rawHtml = component.Render(dataContext);

            // Inject OOB attribute BEFORE metadata (ensures pattern match succeeds)
            var oobHtml = rawHtml.Replace($"id=\"{componentInfo.Id}\"", $"id=\"{componentInfo.Id}\" hx-swap-oob=\"true\"");

            // Then inject metadata attributes (for next interaction's component context)
            var finalHtml = injectMetadataAttributes(oobHtml, component);

            updates.Add(finalHtml);
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

    /// <summary>
    /// Renders a component and automatically injects component tracking metadata.
    /// For components with Dependencies.None, no metadata is added (static pages).
    /// For components with data dependencies, adds data-component and data-domain attributes.
    /// </summary>
    private string renderComponentWithMetadata(IServerComponent component, IDataContext data)
    {
        var html = component.Render(data);
        return injectMetadataAttributes(html, component);
    }

    /// <summary>
    /// Injects data-component and data-domain attributes into the component's root element.
    /// For static components (Dependencies.None), returns HTML unchanged.
    /// </summary>
    private string injectMetadataAttributes(string html, IServerComponent component)
    {
        // No metadata needed for static components
        if (component.Dependencies == DataDependencies.None)
            return html;

        // Inject data-component and data-domain attributes into root element
        // The component renders with id="...", we inject tracking attributes after it
        var componentTypeName = component.GetType().Name;
        var domainName = component.Dependencies.Domain;

        var replacement = $"id=\"{component.ComponentId}\" data-component=\"{componentTypeName}\" data-domain=\"{domainName}\"";
        return html.Replace($"id=\"{component.ComponentId}\"", replacement);
    }
}
