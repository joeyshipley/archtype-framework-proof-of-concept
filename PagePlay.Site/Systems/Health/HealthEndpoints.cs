using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Routing;

namespace PagePlay.Site.Systems.Health;

public class HealthEndpoints(IRepository _repository) : IEndpointRoutes
{
    public void MapRoutes(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/systems")
            .WithTags("Systems");

        group.MapGet("/health", () =>
        {
            return Results.Ok(new { status = "healthy" });
        })
        .WithName("Health")
        .WithDescription("Basic liveness check - returns 200 if the application is running")
        .Produces<object>(StatusCodes.Status200OK);

        group.MapGet("/health/ready", async () =>
        {
            var isHealthy = await _repository.Health();

            if (isHealthy)
            {
                return Results.Ok(new { status = "ready", database = "connected" });
            }

            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        })
        .WithName("HealthReady")
        .WithDescription("Readiness check - verifies database connectivity")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status503ServiceUnavailable);
    }
}
