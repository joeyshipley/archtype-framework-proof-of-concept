using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class DependencyResolver
{
    public static void Bind(IServiceCollection services)
    {
        BindApplicationComponents(services);
        BindWorkflows(services);
        ApiRoutingResolver.BindRouting(services);
    }

    private static void BindApplicationComponents(IServiceCollection services)
    {
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }

    private static void BindWorkflows(IServiceCollection services)
    {
        services.AutoRegisterByNamingPattern("Workflow", ServiceLifetime.Scoped);
    }
}