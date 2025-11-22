using Microsoft.EntityFrameworkCore;
using NSubstitute;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database;

namespace PagePlay.Tests.Infrastructure.Database;

public class TestAppDbContext : AppDbContext
{
    public TestAppDbContext() : base(CreateMockSettingsProvider())
    {
    }

    private static ISettingsProvider CreateMockSettingsProvider()
    {
        var settingsProvider = Substitute.For<ISettingsProvider>();
        settingsProvider.Database.Returns(new DatabaseSettings
        {
            ConnectionString = "unused-in-tests"
        });
        return settingsProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
        }
    }
}
