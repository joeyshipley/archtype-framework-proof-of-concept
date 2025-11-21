using System.Reflection;

namespace PagePlay.Site.Infrastructure.Routing;

public static class EndpointRoutesExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var endpointTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => 
                typeof(IEndpointRoutes).IsAssignableFrom(t) 
                && t is { IsInterface: false, IsAbstract: false }
            );

        using var scope = endpoints.ServiceProvider.CreateScope();
        foreach (var type in endpointTypes)
        {
            var instance = (IEndpointRoutes)ActivatorUtilities.CreateInstance(scope.ServiceProvider, type);
            instance.MapRoutes(endpoints);
        }

        return endpoints;
    }
    
    public static RouteHandlerBuilder WithApplicationErrorResponses(this RouteHandlerBuilder builder)
    {
        return builder
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}