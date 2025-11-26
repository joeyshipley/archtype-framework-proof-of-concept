using PagePlay.Site.Application.Accounts;
using PagePlay.Site.Infrastructure.Routing;
using PagePlay.Site.Pages;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Todos;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class ApiRoutingResolver
{
    public static void BindRouting(IServiceCollection services)
    {
        // TODO: remove these once we've converted to new way.
        services.AutoRegister<IEndpointRoutes>(ServiceLifetime.Scoped);
        services.AutoRegister<IAccountEndpoint>(ServiceLifetime.Scoped);

        // These are the new way.
        services.AutoRegister<IClientEndpoint>(ServiceLifetime.Scoped);
        // TODO: can we auto-register these based on assembly scanning?
        services.AutoRegister<ITodosPageInteraction>(ServiceLifetime.Scoped);
        services.AutoRegister<ILoginPageInteraction>(ServiceLifetime.Scoped);
    }
}