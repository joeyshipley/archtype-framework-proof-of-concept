using PagePlay.Site.Application.Accounts;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Site.Infrastructure.Routing;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class ApiRoutingResolver
{
    public static void BindRouting(IServiceCollection services)
    {
        AccountRouting(services);
    }
        
    private static void AccountRouting(IServiceCollection services)
    {
        services.AddScoped<IEndpointRoutes, AccountRoutes>();
        
        services.AddScoped<IAccountEndpoint, LoginEndpoint>();
        services.AddScoped<IAccountEndpoint, ViewProfileEndpoint>();
    }
}