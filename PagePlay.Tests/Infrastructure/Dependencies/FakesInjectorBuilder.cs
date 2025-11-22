using Microsoft.Extensions.DependencyInjection;

namespace PagePlay.Tests.Infrastructure.Dependencies;

public class FakesInjectorBuilder(IServiceCollection _services)
{
    private readonly List<(Type InterfaceType, Type ImplementationType)> _fakes = new();

    public FakesInjectorBuilder Replace<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _fakes.Add((typeof(TInterface), typeof(TImplementation)));
        return this;
    }

    public void Use()
    {
        _services.BindWithFakes(_fakes);
    }
}
