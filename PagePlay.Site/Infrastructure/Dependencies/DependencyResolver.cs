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
        bindApplicationComponents(services);
        bindData(services);
        bindValidation(services);
        bindWorkflows(services);
        ApiRoutingResolver.BindRouting(services);
    }

    private static void bindApplicationComponents(IServiceCollection services)
    {
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }

    private static void bindData(IServiceCollection services)
    {
        services.AddScoped<AppDbContext>(sp =>
        {
            var settingsProvider = sp.GetRequiredService<ISettingsProvider>();
            return new AppDbContext(settingsProvider);
        });
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void bindValidation(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IRequest>();
    }

    private static void bindWorkflows(IServiceCollection services)
    {
        services.AutoRegisterWorkflows(ServiceLifetime.Scoped);
    }
}