using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todo.CreateTodo;

public class CreateTodoEndpoint : ITodoEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<CreateTodoResponse>("/create", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        CreateTodoRequest request,
        IWorkflow<CreateTodoRequest, CreateTodoResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
