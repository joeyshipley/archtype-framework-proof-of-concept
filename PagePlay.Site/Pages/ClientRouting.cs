using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Pages;

public interface IClientEndpoint : IEndpoint {}

public class ClientInteractionRouting(
    IEnumerable<IClientEndpoint> _endpoints
) : IEndpointRoutes
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("")
            .WithTags("Client Interactions");
        
        foreach (var endpoint in _endpoints)
            endpoint.Map(group);
    }
}

