using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Infrastructure.Database;

public class AppDbContext : DbContext
{
    private readonly ISettingsProvider _settingsProvider;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically discover and register all entities implementing IAggregateEntity
        var aggregateEntityTypes = typeof(AppDbContext).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IAggregateEntity).IsAssignableFrom(t));

        foreach (var entityType in aggregateEntityTypes)
        {
            modelBuilder.Entity(entityType);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAggregateEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
