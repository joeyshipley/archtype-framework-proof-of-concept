using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todos.ListTodos;

public class ListTodosEndpoint : ITodoEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<ListTodosResponse>("/list", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        ListTodosRequest request,
        IWorkflow<ListTodosRequest, ListTodosResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
