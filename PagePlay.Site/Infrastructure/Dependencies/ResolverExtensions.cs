using System.Reflection;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class ResolverExtensions
{
    public static void AutoRegister<TInterface>(
        this IServiceCollection services,
        ServiceLifetime lifetime
    )
        where TInterface : class
    {
        var implementations = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => 
                t is { IsClass: true, IsAbstract: false }
                && typeof(TInterface).IsAssignableFrom(t)
            );

        foreach (var implementation in implementations)
        {
            var descriptor = new ServiceDescriptor(
                typeof(TInterface),
                implementation,
                lifetime
            );
            services.Add(descriptor);
        }
    }

    public static void AutoRegisterByNamingPattern(
        this IServiceCollection services,
        string pattern,
        ServiceLifetime lifetime
    )
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false }
                && t.Name.Contains(pattern)
            );

        foreach (var type in types)
        {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{type.Name}");

            if (interfaceType != null)
            {
                var descriptor = new ServiceDescriptor(interfaceType, type, lifetime);
                services.Add(descriptor);
            }
        }
    }

    public static void AutoRegisterWorkflows(
        this IServiceCollection services,
        ServiceLifetime lifetime
    )
    {
        var workflowTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false }
                && t.Name.EndsWith("Workflow")
            );

        foreach (var workflowType in workflowTypes)
        {
            var workflowInterface = workflowType.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(Application.IWorkflow<,>)
                );

            if (workflowInterface != null)
            {
                var descriptor = new ServiceDescriptor(workflowInterface, workflowType, lifetime);
                services.Add(descriptor);
            }
        }
    }    
}