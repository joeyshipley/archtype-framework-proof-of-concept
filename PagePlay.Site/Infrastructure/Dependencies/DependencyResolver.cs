using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Http;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Pages;
using PagePlay.Site.Pages.Home;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Shared;
using PagePlay.Site.Pages.Todos;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class DependencyResolver
{
    public static void Bind(IServiceCollection services)
    {
        bindApplicationComponents(services);
        bindData(services);
        bindValidation(services);
        bindWorkflows(services);
        bindClient(services);
    }

    private static void bindApplicationComponents(IServiceCollection services)
    {
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton(typeof(ILogProvider<>), typeof(LogProvider<>));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IUserIdentityService, UserIdentityService>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped(sp => (CurrentUserContext)sp.GetRequiredService<ICurrentUserContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICookieManager, CookieManager>();
        services.AddScoped<IResponseManager, ResponseManager>();
        services.AddScoped<IAntiforgeryTokenProvider, AntiforgeryTokenProvider>();
    }

    private static void bindData(IServiceCollection services)
    {
        services.AddDbContextFactory<AppDbContext>((sp, options) =>
        {
            var settingsProvider = sp.GetRequiredService<ISettingsProvider>();
            options.UseNpgsql(settingsProvider.Database.ConnectionString);
        });
        services.AddScoped<IRepository, Repository>();
    }

    private static void bindValidation(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IWorkflowRequest>();
    }

    private static void bindWorkflows(IServiceCollection services)
    {
        services.AutoRegisterWorkflows(ServiceLifetime.Scoped);
    }

    private static void bindClient(IServiceCollection services)
    {
        services.AddScoped<IPageLayout, Layout>();
        services.AutoRegisterPages(ServiceLifetime.Scoped);
        services.AutoRegister<IClientEndpoint>(ServiceLifetime.Scoped);
        services.AutoRegister<ITodosPageInteraction>(ServiceLifetime.Scoped);
        services.AutoRegister<ILoginPageInteraction>(ServiceLifetime.Scoped);
    }
}