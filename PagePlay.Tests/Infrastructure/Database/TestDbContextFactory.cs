using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Database;

namespace PagePlay.Tests.Infrastructure.Database;

public class TestDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly TestAppDbContext _context;

    public TestDbContextFactory()
    {
        _context = new TestAppDbContext();
    }

    public AppDbContext CreateDbContext()
    {
        return _context;
    }

    public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<AppDbContext>(_context);
    }
}
