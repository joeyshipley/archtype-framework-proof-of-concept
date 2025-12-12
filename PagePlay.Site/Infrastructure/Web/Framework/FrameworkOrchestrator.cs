namespace PagePlay.Site.Infrastructure.Web.Framework;

using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Mutations;

/// <summary>
/// Orchestrates framework operations: data loading, view rendering, OOB updates.
/// </summary>
public interface IFrameworkOrchestrator
{
    /// <summary>
    /// Loads data for all views and renders them.
    /// Used for initial page load.
    /// </summary>
    Task<Dictionary<string, string>> RenderViewsAsync(IEnumerable<IView> views);

    /// <summary>
    /// Handles mutation response by re-rendering affected views.
    /// Used for interaction responses.
    /// Returns HTML string of OOB updates (empty string if no views affected).
    /// </summary>
    Task<string> RenderMutationResponseAsync(
        DataMutations mutations,
        string componentContextJson
    );
}

public class FrameworkOrchestrator(
    IDataLoader _dataLoader,
    IViewContextParser _contextParser,
    IViewFactory _viewFactory
) : IFrameworkOrchestrator
{
    public async Task<Dictionary<string, string>> RenderViewsAsync(
        IEnumerable<IView> views)
    {
        var viewList = views.ToList();

        // 1. Collect all required domain context types (excluding None dependencies)
        var requiredContextTypes = viewList
            .Where(v => v.Dependencies != DataDependencies.None)
            .Select(v => v.Dependencies.DomainContextType)
            .Where(t => t != null)
            .Distinct()
            .ToArray();

        // 2. Load all domains in parallel (or create empty context if no domains needed)
        var dataContext = requiredContextTypes.Length > 0
            ? await buildFluentChain(requiredContextTypes).Load()
            : DataContext.Empty();

        // 3. Render all views with automatic metadata injection
        var renderedViews = new Dictionary<string, string>();
        foreach (var view in viewList)
        {
            var html = renderViewWithMetadata(view, dataContext);
            renderedViews[view.ViewId] = html;
        }

        return renderedViews;
    }

    public async Task<string> RenderMutationResponseAsync(
        DataMutations mutations,
        string viewContextJson)
    {
        // 1. Parse view context from client
        var pageViews = _contextParser.Parse(viewContextJson);

        // 2. Find views affected by mutation
        var affectedViews = (mutations != null)
            ? pageViews
                .Where(v => mutations.Domains.Contains(v.Domain))
                .ToList()
            : [];

        if (affectedViews.Count == 0)
            return string.Empty; // No views to update

        // 3. Map affected view domain names to context types
        var affectedContextTypes = affectedViews
            .Select(v =>
            {
                var view = _viewFactory.Create(v.ViewType);
                return view?.Dependencies.DomainContextType;
            })
            .Where(t => t != null)
            .Distinct()
            .ToArray();

        // 4. Re-fetch mutated domains (or create empty context if no domains)
        var dataContext = affectedContextTypes.Length > 0
            ? await buildFluentChain(affectedContextTypes).Load()
            : DataContext.Empty();

        // 5. Re-render affected views with OOB attribute
        var updates = new List<string>();
        foreach (var viewInfo in affectedViews)
        {
            var view = _viewFactory.Create(viewInfo.ViewType);
            if (view == null) continue;

            // Render raw view HTML first
            var rawHtml = view.Render(dataContext);

            // Inject OOB attribute BEFORE metadata (ensures pattern match succeeds)
            var oobHtml = rawHtml.Replace($"id=\"{viewInfo.Id}\"", $"id=\"{viewInfo.Id}\" hx-swap-oob=\"true\"");

            // Then inject metadata attributes (for next interaction's view context)
            var finalHtml = injectMetadataAttributes(oobHtml, view);

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
    /// Renders a view and automatically injects view tracking metadata.
    /// For views with Dependencies.None, no metadata is added (static pages).
    /// For views with data dependencies, adds data-view and data-domain attributes.
    /// </summary>
    private string renderViewWithMetadata(IView view, IDataContext data)
    {
        var html = view.Render(data);
        return injectMetadataAttributes(html, view);
    }

    /// <summary>
    /// Injects data-view and data-domain attributes into the view's root element.
    /// For static views (Dependencies.None), returns HTML unchanged.
    /// </summary>
    private string injectMetadataAttributes(string html, IView view)
    {
        // No metadata needed for static views
        if (view.Dependencies == DataDependencies.None)
            return html;

        // Inject data-view and data-domain attributes into root element
        // The view renders with id="...", we inject tracking attributes after it
        var viewTypeName = view.GetType().Name;
        var domainName = view.Dependencies.Domain;

        var replacement = $"id=\"{view.ViewId}\" data-view=\"{viewTypeName}\" data-domain=\"{domainName}\"";
        return html.Replace($"id=\"{view.ViewId}\"", replacement);
    }
}
