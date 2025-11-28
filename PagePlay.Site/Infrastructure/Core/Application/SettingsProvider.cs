namespace PagePlay.Site.Infrastructure.Core.Application;

public interface ISettingsProvider
{
    SecuritySettings Security { get; }
    DatabaseSettings Database { get; }
    RateLimitingSettings RateLimiting { get; }
}

public class SettingsProvider(IConfiguration _configuration)
    : ISettingsProvider
{
    public SecuritySettings Security { get; } = LoadSecuritySettings(_configuration);
    public DatabaseSettings Database { get; } = LoadDatabaseSettings(_configuration);
    public RateLimitingSettings RateLimiting { get; } = LoadRateLimitingSettings(_configuration);

    private static SecuritySettings LoadSecuritySettings(IConfiguration configuration)
    {
        var settings = configuration.GetSection("Security").Get<SecuritySettings>();

        if (settings == null)
            throw new InvalidOperationException("Security settings are missing from configuration. Please ensure 'Security' section exists in appsettings.json.");

        if (string.IsNullOrWhiteSpace(settings.PasswordPepper))
            throw new InvalidOperationException("Security:PasswordPepper is required but not configured in appsettings.json.");

        if (settings.Jwt == null)
            throw new InvalidOperationException("Security:Jwt settings are missing from configuration.");

        if (string.IsNullOrWhiteSpace(settings.Jwt.SecretKey))
            throw new InvalidOperationException("Security:Jwt:SecretKey is required but not configured in appsettings.json.");

        if (settings.Jwt.SecretKey.Length < 32)
            throw new InvalidOperationException("Security:Jwt:SecretKey must be at least 32 characters long for security reasons.");

        if (string.IsNullOrWhiteSpace(settings.Jwt.Issuer))
            throw new InvalidOperationException("Security:Jwt:Issuer is required but not configured in appsettings.json.");

        if (string.IsNullOrWhiteSpace(settings.Jwt.Audience))
            throw new InvalidOperationException("Security:Jwt:Audience is required but not configured in appsettings.json.");

        if (settings.Jwt.ExpirationMinutes <= 0)
            throw new InvalidOperationException("Security:Jwt:ExpirationMinutes must be greater than 0.");

        return settings;
    }

    private static DatabaseSettings LoadDatabaseSettings(IConfiguration configuration)
    {
        var settings = configuration.GetSection("Database").Get<DatabaseSettings>();

        if (settings == null)
            throw new InvalidOperationException("Database settings are missing from configuration. Please ensure 'Database' section exists in appsettings.json.");

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new InvalidOperationException("Database:ConnectionString is required but not configured in appsettings.json.");

        return settings;
    }

    private static RateLimitingSettings LoadRateLimitingSettings(IConfiguration configuration)
    {
        // RateLimiting is optional - use defaults if not configured
        var settings = configuration.GetSection("RateLimiting").Get<RateLimitingSettings>()
            ?? new RateLimitingSettings();

        // Validate if configured
        if (settings.RequestsPerMinute <= 0)
            throw new InvalidOperationException("RateLimiting:RequestsPerMinute must be greater than 0 if configured.");

        return settings;
    }
}

public class SecuritySettings
{
    public string PasswordPepper { get; set; }
    public JwtSettings Jwt { get; set; }
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class RateLimitingSettings
{
    /// <summary>
    /// Maximum number of requests allowed per minute per user/IP.
    /// Default: 250 (generous for normal users, provides good protection against abuse)
    /// </summary>
    public int RequestsPerMinute { get; set; } = 250;

    /// <summary>
    /// Paths to exclude from rate limiting (e.g., health checks, static assets).
    /// Use forward slashes. Example: "/health", "/_content", "/css"
    /// </summary>
    public List<string> ExcludedPaths { get; set; } = new()
    {
        "/health",
        "/_content",
        "/_framework"
    };
}