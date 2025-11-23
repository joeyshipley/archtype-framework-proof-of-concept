using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todo.ListTodo;

public class ListTodoEndpoint : ITodoEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.Register<ListTodoResponse>("/list", handle)
            .RequireAuthenticatedUser();

    private async Task<IResult> handle(
        ListTodoRequest request,
        IWorkflow<ListTodoRequest, ListTodoResponse> workflow
    ) => Respond.With(await workflow.Perform(request));
}
