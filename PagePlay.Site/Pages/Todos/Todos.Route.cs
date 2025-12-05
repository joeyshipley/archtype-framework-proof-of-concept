using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageInteraction : IEndpoint {}

public class TodosPageEndpoints(
    IPageLayout _layout,
    TodosPage _page,
    IFrameworkOrchestrator _framework,
    IEnumerable<ITodosPageInteraction> _interactions,
    ILogger<TodosPageEndpoints> _logger
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "todos";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async () =>
        {
            try
            {
                // Framework handles data loading and metadata injection
                var components = new IServerComponent[] { _page };
                var renderedComponents = await _framework.RenderComponentsAsync(components);
                var bodyContent = renderedComponents[_page.ComponentId];

                var page = await _layout.RenderAsync("Todos", bodyContent);
                return Results.Content(page, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load todos page for current user");
                var bodyContent = _page.RenderError("Failed to load todos");
                var page = await _layout.RenderAsync("Todos", bodyContent);
                return Results.Content(page, "text/html");
            }
        })
        .RequireAuthenticatedUser();

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
