namespace PagePlay.Site.Infrastructure.Core.Application;

public interface ISettingsProvider
{
    SecuritySettings Security { get; }
    DatabaseSettings Database { get; }
    RateLimitingSettings RateLimiting { get; }
    RequestSizeLimitSettings RequestSizeLimits { get; }
}

public class SettingsProvider(IConfiguration _configuration)
    : ISettingsProvider
{
    public SecuritySettings Security { get; } = LoadSecuritySettings(_configuration);
    public DatabaseSettings Database { get; } = LoadDatabaseSettings(_configuration);
    public RateLimitingSettings RateLimiting { get; } = LoadRateLimitingSettings(_configuration);
    public RequestSizeLimitSettings RequestSizeLimits { get; } = LoadRequestSizeLimitSettings(_configuration);

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

    private static RequestSizeLimitSettings LoadRequestSizeLimitSettings(IConfiguration configuration)
    {
        // RequestSizeLimits is optional - use defaults if not configured
        var settings = configuration.GetSection("RequestSizeLimits").Get<RequestSizeLimitSettings>()
            ?? new RequestSizeLimitSettings();

        // Validate if configured
        if (settings.MaxRequestBodySizeBytes <= 0)
            throw new InvalidOperationException("RequestSizeLimits:MaxRequestBodySizeBytes must be greater than 0 if configured.");

        if (settings.MaxRequestLineSizeBytes <= 0)
            throw new InvalidOperationException("RequestSizeLimits:MaxRequestLineSizeBytes must be greater than 0 if configured.");

        if (settings.MaxRequestHeadersTotalSizeBytes <= 0)
            throw new InvalidOperationException("RequestSizeLimits:MaxRequestHeadersTotalSizeBytes must be greater than 0 if configured.");

        if (settings.RequestHeadersTimeoutSeconds <= 0)
            throw new InvalidOperationException("RequestSizeLimits:RequestHeadersTimeoutSeconds must be greater than 0 if configured.");

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

public class RequestSizeLimitSettings
{
    /// <summary>
    /// Maximum size of the entire request body in bytes.
    /// Default: 5 MB (5,242,880 bytes) - reasonable for forms, JSON, and small file uploads.
    /// Can be overridden per-endpoint using [RequestSizeLimit] attribute for larger uploads.
    /// </summary>
    public long MaxRequestBodySizeBytes { get; set; } = 5_242_880; // 5 MB

    /// <summary>
    /// Maximum size of the request line (HTTP method + URL + query string) in bytes.
    /// Default: 16 KB (16,384 bytes) - accommodates complex URLs with query parameters.
    /// </summary>
    public int MaxRequestLineSizeBytes { get; set; } = 16_384; // 16 KB

    /// <summary>
    /// Maximum total size of all request headers combined in bytes.
    /// Default: 64 KB (65,536 bytes) - generous for cookies, auth tokens, and custom headers.
    /// </summary>
    public int MaxRequestHeadersTotalSizeBytes { get; set; } = 65_536; // 64 KB

    /// <summary>
    /// Maximum time in seconds to wait for request headers to be received.
    /// Default: 30 seconds - protects against slowloris attacks while allowing slow connections.
    /// </summary>
    public int RequestHeadersTimeoutSeconds { get; set; } = 30;
}