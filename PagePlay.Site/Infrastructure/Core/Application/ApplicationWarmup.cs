using Microsoft.EntityFrameworkCore;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Data;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Infrastructure.Core.Application;

/// <summary>
/// Provides warmup functionality to avoid cold start penalties on first request.
/// Warms up expensive services like password hashing and EF Core query compilation.
/// </summary>
public static class ApplicationWarmup
{
    /// <summary>
    /// Warms up critical services to avoid cold start penalties.
    /// Should be called after the application is built but before it starts listening for requests.
    /// </summary>
    /// <param name="services">The application's service provider</param>
    public static async Task WarmupAsync(this IServiceProvider services)
    {
        await Task.Run(async () =>
        {
            // Warm up BCrypt password hasher
            // BCrypt is intentionally slow for security, so first call has noticeable delay
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            _ = passwordHasher.VerifyPassword("warmup", "$2a$12$dummy.hash.for.warmup.only...................");

            // Warm up EF Core query compilation and database connection pool
            // First query triggers expression tree compilation and connection pool initialization
            using var scope = services.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            _ = await dbContext.Set<User>()
                .Where(u => u.Email == "warmup@example.com")
                .AsNoTracking()
                .FirstOrDefaultAsync();
        });
    }
}
