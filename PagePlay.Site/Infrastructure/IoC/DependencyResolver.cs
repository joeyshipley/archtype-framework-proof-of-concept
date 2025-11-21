using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Application.Accounts.ViewProfile;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Infrastructure.IoC;

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
        services.AddTransient<ISettingsProvider, SettingsProvider>();
    }

    private static void BindWorkflows(IServiceCollection services)
    {
        services.AddTransient<ILoginWorkflow, LoginWorkflow>();
        services.AddTransient<IViewProfileWorkflow, ViewProfileWorkflow>();
    }
}