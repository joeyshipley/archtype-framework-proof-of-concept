using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Site.Application.StyleTest.GetRandomNumber;
using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.Analytics;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Performers.CreateTodo;
using PagePlay.Site.Application.Todos.Performers.DeleteTodo;
using PagePlay.Site.Application.Todos.Performers.ToggleTodo;
using PagePlay.Site.Application.Todos.Performers.UpdateTodo;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.UI.Rendering;
using PagePlay.Site.Infrastructure.Web.Components;
using PagePlay.Site.Infrastructure.Web.Data;
using PagePlay.Site.Infrastructure.Web.Framework;
using PagePlay.Site.Infrastructure.Web.Http;
using PagePlay.Site.Pages;
using PagePlay.Site.Pages.Home;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Login.Interactions;
using PagePlay.Site.Pages.Shared;
using PagePlay.Site.Pages.StyleTest;
using PagePlay.Site.Pages.StyleTest.Interactions;
using PagePlay.Site.Pages.Todos;
using PagePlay.Site.Pages.Todos.Interactions;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class DependencyResolver
{
    public static void Bind(IServiceCollection services)
    {
        bindInfrastructure(services);
        bindData(services);
        bindValidation(services);
        bindPerformers(services);
        bindClient(services);
    }

    #region Infrastructure

    private static void bindInfrastructure(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        // Singletons
        services.AddSingleton<IHtmlRenderer, HtmlRenderer>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton<IUserIdentityService, UserIdentityService>();
        services.AddSingleton(typeof(ILogRecorder<>), typeof(LogRecorder<>));

        // Scoped
        services.AddScoped<IAntiforgeryTokenProvider, AntiforgeryTokenProvider>();
        services.AddScoped<ICookieManager, CookieManager>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped(sp => (CurrentUserContext)sp.GetRequiredService<ICurrentUserContext>());
        services.AddScoped<IDataLoader, DataLoader>();
        services.AddScoped<IFrameworkOrchestrator, FrameworkOrchestrator>();
        services.AddScoped<IResponseManager, ResponseManager>();
        services.AddScoped<IViewContextParser, ViewContextParser>();
        services.AddScoped<IViewFactory, ViewFactory>();
    }

    #endregion

    #region Data

    private static void bindData(IServiceCollection services)
    {
        services.AddDbContextFactory<AppDbContext>((sp, options) =>
        {
            var settingsProvider = sp.GetRequiredService<ISettingsProvider>();
            options.UseNpgsql(settingsProvider.Database.ConnectionString);
        });
        services.AddScoped<IRepository, Repository>();

        // Data Providers
        services.AddScoped<IDataProvider, TodoAnalyticsProvider>();
        services.AddScoped<IDataProvider, TodosListProvider>();
    }

    #endregion

    #region Validation

    private static void bindValidation(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IPerformerRequest>();
    }

    #endregion

    #region Performers

    private static void bindPerformers(IServiceCollection services)
    {
        bindAccountPerformers(services);
        bindStyleTestPerformers(services);
        bindTodoPerformers(services);
    }

    private static void bindAccountPerformers(IServiceCollection services)
    {
        services.AddScoped<IPerformer<LoginRequest, LoginResponse>, LoginPerformer>();
        services.AddScoped<IPerformer<RegisterRequest, RegisterResponse>, RegisterPerformer>();
        services.AddScoped<IPerformer<ViewProfileRequest, ViewProfileResponse>, ViewProfilePerformer>();
    }

    private static void bindStyleTestPerformers(IServiceCollection services)
    {
        services.AddScoped<IPerformer<GetRandomNumberRequest, GetRandomNumberResponse>, GetRandomNumberPerformer>();
    }

    private static void bindTodoPerformers(IServiceCollection services)
    {
        services.AddScoped<IPerformer<CreateTodoRequest, CreateTodoResponse>, CreateTodoPerformer>();
        services.AddScoped<IPerformer<DeleteTodoRequest, DeleteTodoResponse>, DeleteTodoPerformer>();
        services.AddScoped<IPerformer<ToggleTodoRequest, ToggleTodoResponse>, ToggleTodoPerformer>();
        services.AddScoped<IPerformer<UpdateTodoRequest, UpdateTodoResponse>, UpdateTodoPerformer>();
    }

    #endregion

    #region Client

    private static void bindClient(IServiceCollection services)
    {
        bindSharedComponents(services);
        bindPages(services);
        bindPageEndpoints(services);
        bindPageInteractions(services);
    }

    private static void bindSharedComponents(IServiceCollection services)
    {
        services.AddScoped<IAnalyticsStatsWidget, AnalyticsStatsWidget>();
        services.AddScoped<INavView, Nav>();
        services.AddScoped<IPageLayout, Layout>();
        services.AddScoped<IWelcomeWidget, WelcomeWidget>();
    }

    private static void bindPages(IServiceCollection services)
    {
        // Auto-discover and register all IView implementations by their specific interface
        var viewTypes = typeof(IView).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IView).IsAssignableFrom(t));

        foreach (var concreteType in viewTypes)
        {
            var viewInterface = concreteType.GetInterfaces()
                .FirstOrDefault(i => i != typeof(IView) && typeof(IView).IsAssignableFrom(i));

            if (viewInterface != null)
                services.AddScoped(viewInterface, concreteType);
        }
    }

    private static void bindPageEndpoints(IServiceCollection services)
    {
        services.AddScoped<IClientEndpoint, HomePageEndpoints>();
        services.AddScoped<IClientEndpoint, LoginPageEndpoints>();
        services.AddScoped<IClientEndpoint, StyleTestPageEndpoints>();
        services.AddScoped<IClientEndpoint, TodosPageEndpoints>();
    }

    private static void bindPageInteractions(IServiceCollection services)
    {
        // Login
        services.AddScoped<ILoginPageInteraction, AuthenticateInteraction>();

        // StyleTest
        services.AddScoped<IStyleTestPageInteraction, GetRandomNumberInteraction>();

        // Todos
        services.AddScoped<ITodosPageInteraction, CreateTodoInteraction>();
        services.AddScoped<ITodosPageInteraction, DeleteTodoInteraction>();
        services.AddScoped<ITodosPageInteraction, ToggleTodoInteraction>();
    }

    #endregion
}
