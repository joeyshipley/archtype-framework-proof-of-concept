using PagePlay.Site.Application.Todos.ListTodos;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Todos;

public interface ITodosPageInteraction : IEndpoint {}

public class TodosPageEndpoints(
    IPageLayout _layout,
    ITodosPageView _page,
    IEnumerable<ITodosPageInteraction> _interactions
) : IClientEndpoint
{
    public const string PAGE_ROUTE = "todos";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(PAGE_ROUTE, async (
            IWorkflow<ListTodosWorkflowRequest, ListTodosWorkflowResponse> listWorkflow
        ) =>
        {
            var result = await listWorkflow.Perform(new ListTodosWorkflowRequest());
            var bodyContent = !result.Success
                ? _page.RenderError("Failed to load todos")
                : _page.RenderPage(result.Model.Todos);

            var page = _layout.Render("Todos", bodyContent);
            return Results.Content(page, "text/html");
        })
        .RequireAuthenticatedUser();

        foreach (var interaction in _interactions)
            interaction.Map(endpoints);
    }
}
