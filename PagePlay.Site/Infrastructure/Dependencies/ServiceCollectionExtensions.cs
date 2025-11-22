using Microsoft.Extensions.DependencyInjection;

namespace PagePlay.Site.Infrastructure.Dependencies;

public static class ServiceCollectionExtensions
{
    public static void Replace<TInterface, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime
    )
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var existingDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TInterface));
        if (existingDescriptor != null)
        {
            services.Remove(existingDescriptor);
        }

        services.Add(new ServiceDescriptor(
            typeof(TInterface),
            typeof(TImplementation),
            lifetime
        ));
    }
}
