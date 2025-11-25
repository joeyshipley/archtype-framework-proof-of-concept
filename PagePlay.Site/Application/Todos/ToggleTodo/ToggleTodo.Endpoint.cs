using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todos.ToggleTodo;

public class ToggleTodoEndpoint : ITodoEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<ToggleTodoResponse>("/toggle", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        ToggleTodoRequest request,
        IWorkflow<ToggleTodoRequest, ToggleTodoResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
