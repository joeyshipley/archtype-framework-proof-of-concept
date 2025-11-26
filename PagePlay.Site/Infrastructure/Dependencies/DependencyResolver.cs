using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Database.Repositories;
using PagePlay.Site.Infrastructure.Security;
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
        ApiRoutingResolver.BindRouting(services);
    }

    private static void bindApplicationComponents(IServiceCollection services)
    {
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthContext, LoggedInAuthContext>();
        services.AddScoped(sp => (LoggedInAuthContext)sp.GetRequiredService<IAuthContext>());
        services.AddHttpContextAccessor();
        
        // TODO: remove this when cleaned up
        // Named HttpClient for calling internal APIs
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5200");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
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
        services.AddScoped<TodosPage>();
        services.AddScoped<LoginPage>();
    }
}