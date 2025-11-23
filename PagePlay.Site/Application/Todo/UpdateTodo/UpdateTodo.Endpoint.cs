using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todo.UpdateTodo;

public class UpdateTodoEndpoint : ITodoEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<UpdateTodoResponse>("/update", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        UpdateTodoRequest request,
        IWorkflow<UpdateTodoRequest, UpdateTodoResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
