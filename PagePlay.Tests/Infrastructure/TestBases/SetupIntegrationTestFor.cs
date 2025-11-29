using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Dependencies;
using PagePlay.Tests.Infrastructure.Dependencies;
using PagePlay.Tests.Infrastructure.Logging;

namespace PagePlay.Tests.Infrastructure.TestBases;

public class SetupIntegrationTestFor<T> where T : class
{
    protected T SUT { get; private set; }

    private IServiceCollection _services;

    protected SetupIntegrationTestFor()
    {
        _services = new ServiceCollection();

        // Add test configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Security:PasswordPepper"] = "test-pepper",
                ["Security:Jwt:SecretKey"] = "test-secret-key-that-is-long-enough-for-jwt",
                ["Security:Jwt:Issuer"] = "test-issuer",
                ["Security:Jwt:Audience"] = "test-audience",
                ["Security:Jwt:ExpirationMinutes"] = "60",
                ["Database:ConnectionString"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        _services.AddSingleton<IConfiguration>(configuration);

        DependencyResolver.Bind(_services);

        // Override real logger with fake logger (must be after DependencyResolver.Bind)
        // Remove the real LogRecorder registration and replace with fake
        var logRecorderDescriptor = _services.FirstOrDefault(d =>
            d.ServiceType.IsGenericType &&
            d.ServiceType.GetGenericTypeDefinition() == typeof(ILogRecorder<>));
        if (logRecorderDescriptor != null)
        {
            _services.Remove(logRecorderDescriptor);
        }
        _services.AddSingleton(typeof(ILogRecorder<>), typeof(FakeLogRecorder<>));

        BuildSUT(_services);
    }

    protected FakesInjectorBuilder Fakes()
    {
        var fakesInjector = new FakesInjectorBuilder(_services, BuildSUT);
        return fakesInjector;
    }

    protected void BuildSUT(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        SUT = serviceProvider.GetRequiredService<T>();
    }
}
