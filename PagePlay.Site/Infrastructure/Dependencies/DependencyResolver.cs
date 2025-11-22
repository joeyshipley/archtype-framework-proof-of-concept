using PagePlay.Site.Application.Accounts.Domain.Repository;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class DependencyResolver
{
    public static void Bind(IServiceCollection services)
    {
        BindApplicationComponents(services);
        BindData(services);
        BindWorkflows(services);
        ApiRoutingResolver.BindRouting(services);
    }

    private static void BindApplicationComponents(IServiceCollection services)
    {
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }

    private static void BindData(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void BindWorkflows(IServiceCollection services)
    {
        services.AutoRegisterByNamingPattern("Workflow", ServiceLifetime.Scoped);
    }
}