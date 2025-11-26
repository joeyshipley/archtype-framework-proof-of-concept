using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Data;

namespace PagePlay.Tests.Infrastructure.Database;

public class TestAppDbContext : AppDbContext
{
    public TestAppDbContext() : base(createOptions())
    {
    }

    private static DbContextOptions<AppDbContext> createOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
        return optionsBuilder.Options;
    }
}
