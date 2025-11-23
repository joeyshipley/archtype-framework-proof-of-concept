using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Infrastructure.Domain;

namespace PagePlay.Site.Infrastructure.Database;

public class AppDbContext : DbContext
{
    // Constructor used by both IDbContextFactory at runtime and IDesignTimeDbContextFactory for migrations
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically discover and register all entities implementing IEntity
        var entityTypes = typeof(AppDbContext).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IEntity).IsAssignableFrom(t));

        foreach (var entityType in entityTypes)
        {
            modelBuilder.Entity(entityType);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IEntity>();

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
