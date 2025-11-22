using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Infrastructure.Database;

public class AppDbContext : DbContext
{
    private readonly ISettingsProvider? _settingsProvider;

    // Runtime constructor - used by your application via DI
    public AppDbContext(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    // Design-time constructor - used by EF Core CLI tools via AppDbContextFactory
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _settingsProvider = null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // If already configured (design-time via factory), skip
        if (optionsBuilder.IsConfigured)
            return;

        // Runtime configuration via ISettingsProvider
        if (_settingsProvider == null)
            throw new InvalidOperationException(
                "AppDbContext requires either DbContextOptions or ISettingsProvider.");

        optionsBuilder.UseNpgsql(_settingsProvider.Database.ConnectionString);
    }

    public DbSet<User> Users { get; set; }
}
