using FluentValidation;
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
        BindValidation(services);
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
        services.AddScoped<AppDbContext>(sp =>
        {
            var settingsProvider = sp.GetRequiredService<ISettingsProvider>();
            return new AppDbContext(settingsProvider);
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void BindValidation(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IRequest>();
    }

    private static void BindWorkflows(IServiceCollection services)
    {
        services.AutoRegisterWorkflows(ServiceLifetime.Scoped);
    }
}