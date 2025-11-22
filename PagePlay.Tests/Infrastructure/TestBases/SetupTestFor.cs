using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.AutoSub;
using PagePlay.Site.Infrastructure.Dependencies;
using PagePlay.Tests.Infrastructure.Dependencies;

namespace PagePlay.Tests.Infrastructure.TestBases;

public class SetupTestFor<T> where T : class
{
    protected T SUT { get; private set; }
    protected AutoSubstitute Mocker { get; private set; }

    private IServiceCollection _services;

    protected SetupTestFor()
    {
        Mocker = new AutoSubstitute();
        SUT = Mocker.CreateInstance<T>();

        _services = new ServiceCollection();
        DependencyResolver.Bind(_services);
    }

    protected FakesInjectorBuilder Fakes() =>
        new FakesInjectorBuilder(_services);
}
