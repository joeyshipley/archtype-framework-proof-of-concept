using PagePlay.Site.Application.Todos.ListTodos;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageInteraction : IEndpoint {}

public class TodosPageEndpoints(
    IPageLayout _layout,
    ITodosPageView _page,
    IEnumerable<ITodosPageInteraction> _interactions,
    IWelcomeWidget _welcomeWidget,
    IFrameworkOrchestrator _framework
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "todos";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async (
            IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse> listWorkflow
        ) =>
        {
            // Define page components
            var components = new IServerComponent[] { _welcomeWidget };

            // Framework loads data and renders components
            var renderedComponents = await _framework.RenderComponentsAsync(components);
            var welcomeHtml = renderedComponents[_welcomeWidget.ComponentId];

            // Render todos (existing pattern for now)
            var result = await listWorkflow.Perform(new ListTodosWorkflowRequest());
            var todosHtml = !result.Success
                ? _page.RenderError("Failed to load todos")
                : _page.RenderPage(result.Model.Todos);

            // Combine everything in layout
            var page = _layout.Render("Todos", todosHtml, welcomeHtml);
            return Results.Content(page, "text/html");
        })
        .RequireAuthenticatedUser();

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
