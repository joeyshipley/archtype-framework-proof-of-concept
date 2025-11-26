using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PagePlay.Site.Infrastructure.Data;

/// <summary>
/// Design-time factory for EF Core CLI tools (migrations, scaffolding, etc.)
/// This is ONLY used by 'dotnet ef' commands, never at runtime.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration from the same sources as your app
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string from config (same as runtime)
        var connectionString = configuration.GetSection("Database:ConnectionString").Value
            ?? throw new InvalidOperationException(
                "Database:ConnectionString not found in configuration. " +
                "Ensure appsettings.json or environment variables are configured.");

        // Create DbContext with options (bypassing ISettingsProvider dependency)
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
