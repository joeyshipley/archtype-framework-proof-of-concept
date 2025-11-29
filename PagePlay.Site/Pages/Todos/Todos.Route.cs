using Microsoft.Extensions.Logging;
using PagePlay.Site.Application.Todos.Domain;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageInteraction : IEndpoint {}

public class TodosPageEndpoints(
    IPageLayout _layout,
    ITodosPageView _page,
    IEnumerable<ITodosPageInteraction> _interactions,
    ILogger<TodosPageEndpoints> _logger
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "todos";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async (
            IDataLoader dataLoader
        ) =>
        {
            try
            {
                // Fetch todos via DataDomain (no magic strings!)
                var ctx = await dataLoader.With<TodosDomainContext>().Load();
                var todosData = ctx.Get<TodosDomainContext>();

                var bodyContent = _page.RenderPage(todosData.List);
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
