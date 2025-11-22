using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts;
using PagePlay.Site.Application.Accounts._Domain.Models;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Infrastructure.Database;

public class AppDbContext : DbContext
{
    private readonly ISettingsProvider _settingsProvider;

    public AppDbContext(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_settingsProvider.Database.ConnectionString);
        }
    }

    public DbSet<User> Users { get; set; }
}
