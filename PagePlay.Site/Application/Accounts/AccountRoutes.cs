using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Application.Accounts;

public interface IAccountEndpoint : IEndpoint {}

public class AccountRoutes(IEnumerable<IAccountEndpoint> _endpoints) : IEndpointRoutes
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("v0/api/account")
            .WithTags("Accounts");
        
        foreach (var endpoint in _endpoints)
            endpoint.Map(group);
    }
}

