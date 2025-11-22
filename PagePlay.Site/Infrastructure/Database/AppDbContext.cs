using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts;
using PagePlay.Site.Application.Accounts._Domain.Models;

namespace PagePlay.Site.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
