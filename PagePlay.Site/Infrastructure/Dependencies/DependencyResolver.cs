using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Application.Accounts.Register;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Site.Application.StyleTest.GetRandomNumber;
using PagePlay.Site.Application.Todos.Perspectives;
using PagePlay.Site.Application.Todos.Perspectives.Analytics;
using PagePlay.Site.Application.Todos.Perspectives.List;
using PagePlay.Site.Application.Todos.Workflows.CreateTodo;
using PagePlay.Site.Application.Todos.Workflows.DeleteTodo;
using PagePlay.Site.Application.Todos.Workflows.ToggleTodo;
using PagePlay.Site.Application.Todos.Workflows.UpdateTodo;
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
        bindWorkflows(services);
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
        services.AddValidatorsFromAssemblyContaining<IWorkflowRequest>();
    }

    #endregion

    #region Workflows

    private static void bindWorkflows(IServiceCollection services)
    {
        bindAccountWorkflows(services);
        bindStyleTestWorkflows(services);
        bindTodoWorkflows(services);
    }

    private static void bindAccountWorkflows(IServiceCollection services)
    {
        services.AddScoped<IWorkflow<LoginWorkflowRequest, LoginWorkflowResponse>, LoginWorkflow>();
        services.AddScoped<IWorkflow<RegisterWorkflowRequest, RegisterWorkflowResponse>, RegisterWorkflow>();
        services.AddScoped<IWorkflow<ViewProfileWorkflowRequest, ViewProfileWorkflowResponse>, ViewProfileWorkflow>();
    }

    private static void bindStyleTestWorkflows(IServiceCollection services)
    {
        services.AddScoped<IWorkflow<GetRandomNumberWorkflowRequest, GetRandomNumberWorkflowResponse>, GetRandomNumberWorkflow>();
    }

    private static void bindTodoWorkflows(IServiceCollection services)
    {
        services.AddScoped<IWorkflow<CreateTodoWorkflowRequest, CreateTodoWorkflowResponse>, CreateTodoWorkflow>();
        services.AddScoped<IWorkflow<DeleteTodoWorkflowRequest, DeleteTodoWorkflowResponse>, DeleteTodoWorkflow>();
        services.AddScoped<IWorkflow<ToggleTodoWorkflowRequest, ToggleTodoWorkflowResponse>, ToggleTodoWorkflow>();
        services.AddScoped<IWorkflow<UpdateTodoWorkflowRequest, UpdateTodoWorkflowResponse>, UpdateTodoWorkflow>();
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
        services.AddScoped<IHomePageView, HomePage>();
        services.AddScoped<ILoginPageView, LoginPage>();
        services.AddScoped<IStyleTestPageView, StyleTestPage>();
        services.AddScoped<ITodosPageView, TodosPage>();
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
