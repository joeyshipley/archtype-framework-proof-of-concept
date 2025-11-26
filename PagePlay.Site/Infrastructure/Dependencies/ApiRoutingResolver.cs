using PagePlay.Site.Application.Accounts;
using PagePlay.Site.Infrastructure.Pages;
using PagePlay.Site.Infrastructure.Routing;
using PagePlay.Site.Pages;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class ApiRoutingResolver
{
    public static void BindRouting(IServiceCollection services)
    {
        services.AutoRegister<IEndpointRoutes>(ServiceLifetime.Scoped);
        services.AutoRegister<IAccountEndpoint>(ServiceLifetime.Scoped);

        services.AutoRegister<IClientEndpoint>(ServiceLifetime.Scoped);
        services.AutoRegister<IPageInteraction>(ServiceLifetime.Scoped);
    }
}