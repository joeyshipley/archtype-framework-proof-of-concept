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
    public SecuritySettings Security { get; } = _configuration
        .GetSection("Security")
        .Get<SecuritySettings>()
        ?? new SecuritySettings();

    public DatabaseSettings Database { get; } = _configuration
        .GetSection("Database")
        .Get<DatabaseSettings>()
        ?? new DatabaseSettings();

    public RateLimitingSettings RateLimiting { get; } = _configuration
        .GetSection("RateLimiting")
        .Get<RateLimitingSettings>()
        ?? new RateLimitingSettings();
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