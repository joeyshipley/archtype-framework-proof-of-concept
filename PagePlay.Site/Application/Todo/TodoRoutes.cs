using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Todo;

public interface ITodoEndpoint : IEndpoint {}

public class TodoRoutes(IEnumerable<ITodoEndpoint> _endpoints) : IEndpointRoutes
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("/api/todo")
            .WithTags("Todo");

        foreach (var endpoint in _endpoints)
            endpoint.Map(group);
    }
}
