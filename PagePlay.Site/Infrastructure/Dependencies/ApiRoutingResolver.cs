using PagePlay.Site.Application.Accounts;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class ApiRoutingResolver
{
    public static void BindRouting(IServiceCollection services)
    {
        services.AutoRegister<IEndpointRoutes>(ServiceLifetime.Scoped);

        services.AutoRegister<IAccountEndpoint>(ServiceLifetime.Scoped);
    }
}