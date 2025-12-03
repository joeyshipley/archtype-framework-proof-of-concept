using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.Analytics;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Web.Http;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Pages;
using PagePlay.Site.Pages.Home;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Shared;
using PagePlay.Site.Pages.Todos;
using PagePlay.Site.Pages.Todos.Components;

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
        services.AddSingleton(typeof(ILogRecorder<>), typeof(LogRecorder<>));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IUserIdentityService, UserIdentityService>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped(sp => (CurrentUserContext)sp.GetRequiredService<ICurrentUserContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICookieManager, CookieManager>();
        services.AddScoped<IResponseManager, ResponseManager>();
        services.AddScoped<IAntiforgeryTokenProvider, AntiforgeryTokenProvider>();
        services.AddScoped<IComponentContextParser, ComponentContextParser>();
        services.AddScoped<IDataLoader, DataLoader>();
        services.AddScoped<IComponentFactory, ComponentFactory>();
        services.AddScoped<IFrameworkOrchestrator, FrameworkOrchestrator>();

        // Closed-World UI
        services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
    }

    private static void bindData(IServiceCollection services)
    {
        services.AddDbContextFactory<AppDbContext>((sp, options) =>
        {
            var settingsProvider = sp.GetRequiredService<ISettingsProvider>();
            options.UseNpgsql(settingsProvider.Database.ConnectionString);
        });
        services.AddScoped<IRepository, Repository>();

        // Data Domains - Multiple domains can be registered for different contexts
        // Pattern: Separate domains by computational cost and usage frequency
        services.AddScoped<IDataProvider, TodosListProvider>();           // Basic CRUD operations
        services.AddScoped<IDataProvider, TodoAnalyticsProvider>();   // Analytics/reporting (expensive calculations)
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
        services.AddScoped<INavView, Nav>();
        services.AddScoped<IWelcomeWidget, WelcomeWidget>();
        services.AddScoped<IAnalyticsStatsWidget, AnalyticsStatsWidget>();

        // Components
        services.AddScoped<ITodoListComponent, TodoListComponent>();

        services.AutoRegisterPages(ServiceLifetime.Scoped);
        services.AutoRegister<IClientEndpoint>(ServiceLifetime.Scoped);
        services.AutoRegister<ITodosPageInteraction>(ServiceLifetime.Scoped);
        services.AutoRegister<ILoginPageInteraction>(ServiceLifetime.Scoped);
        services.AutoRegister<PagePlay.Site.Pages.StyleTest.IStyleTestPageInteraction>(ServiceLifetime.Scoped);
    }
}