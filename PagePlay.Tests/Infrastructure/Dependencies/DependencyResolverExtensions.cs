using Microsoft.Extensions.DependencyInjection;
using PagePlay.Site.Infrastructure.Dependencies;

namespace PagePlay.Tests.Infrastructure.Dependencies;

public static class DependencyResolverExtensions
{
    public static void BindWithFakes(
        this IServiceCollection services,
        List<(Type TInterface, Type TImplementation)> fakes
    )
    {
        foreach (var (interfaceType, implementationType) in fakes)
        {
            var existingDescriptor = services.FirstOrDefault(d => d.ServiceType == interfaceType);
            var lifetime = existingDescriptor?.Lifetime ?? ServiceLifetime.Scoped;

            var replaceMethod = typeof(ServiceCollectionExtensions)
                .GetMethod(nameof(ServiceCollectionExtensions.Replace))!
                .MakeGenericMethod(interfaceType, implementationType);

            replaceMethod.Invoke(null, new object[] { services, lifetime });
        }
    }
}
