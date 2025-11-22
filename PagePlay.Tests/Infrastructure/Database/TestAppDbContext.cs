using Microsoft.EntityFrameworkCore;
using Moq;
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
        var mock = new Mock<ISettingsProvider>();
        mock.Setup(x => x.Database).Returns(new DatabaseSettings
        {
            ConnectionString = "unused-in-tests"
        });
        return mock.Object;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
        }
    }
}
