using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todo.DeleteTodo;

public class DeleteTodoEndpoint : ITodoEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<DeleteTodoResponse>("/delete", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        DeleteTodoRequest request,
        IWorkflow<DeleteTodoRequest, DeleteTodoResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
