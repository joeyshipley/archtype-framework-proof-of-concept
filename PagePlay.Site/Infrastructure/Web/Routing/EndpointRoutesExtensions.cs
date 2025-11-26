using System.Reflection;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Infrastructure.Web.Routing;

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

    public static RouteHandlerBuilder Register<TResponse>(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler) where TResponse : class =>
        endpoints.MapPost(pattern, handler).WithResponseOf<TResponse>();

    public static RouteHandlerBuilder WithResponseOf<T>(this RouteHandlerBuilder builder) where T : class =>
        builder
            .Produces<T>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

    public static RouteHandlerBuilder RequireAuthenticatedUser(this RouteHandlerBuilder builder) =>
        builder
            .RequireAuthorization()
            .AddEndpointFilter<PopulateAuthContextFilter>();
}