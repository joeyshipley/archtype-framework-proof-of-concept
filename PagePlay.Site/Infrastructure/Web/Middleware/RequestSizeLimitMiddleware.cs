using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Site.Infrastructure.Web.Middleware;

/// <summary>
/// Configures request size limits on the Kestrel web server to protect against
/// memory exhaustion attacks and various DoS vectors (header bombs, slowloris, etc.).
///
/// These limits are applied globally by default but can be overridden per-endpoint
/// using the [RequestSizeLimit] attribute for specific use cases like file uploads.
///
/// Configuration is loaded from appsettings.json under "RequestSizeLimits" section.
/// All values have sensible defaults suitable for typical web applications.
///
/// IMPORTANT: This must be configured during WebHost building, before the application
/// is built. It cannot be added as middleware in the pipeline like other middleware.
/// </summary>
public static class RequestSizeLimitMiddleware
{
    /// <summary>
    /// Configures Kestrel request size limits based on application settings.
    /// Call this on WebApplicationBuilder.WebHost before building the application.
    ///
    /// Example usage in Program.cs:
    /// <code>
    /// builder.ConfigureRequestSizeLimits();
    /// var app = builder.Build();
    /// </code>
    /// </summary>
    public static WebApplicationBuilder ConfigureRequestSizeLimits(this WebApplicationBuilder builder)
    {
        // Build a temporary service provider to access ISettingsProvider
        // This is necessary because we need settings before the app is built
        using var tempServiceProvider = builder.Services.BuildServiceProvider();
        var settingsProvider = tempServiceProvider.GetRequiredService<ISettingsProvider>();
        var limits = settingsProvider.RequestSizeLimits;

        builder.WebHost.ConfigureKestrel(options =>
        {
            // Maximum size of the request body (forms, JSON, file uploads)
            // Protects against memory exhaustion from large payloads
            // Can be overridden per-endpoint: [RequestSizeLimit(10_485_760)] for 10 MB
            options.Limits.MaxRequestBodySize = limits.MaxRequestBodySizeBytes;

            // Maximum size of the request line (HTTP method + URL + query string)
            // Protects against URL-based attacks and excessive query parameter payloads
            options.Limits.MaxRequestLineSize = limits.MaxRequestLineSizeBytes;

            // Maximum total size of all request headers combined
            // Protects against header bomb attacks (many or very large headers)
            options.Limits.MaxRequestHeadersTotalSize = limits.MaxRequestHeadersTotalSizeBytes;

            // Timeout for receiving request headers
            // Protects against slowloris attacks (slowly sending headers to hold connections)
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(limits.RequestHeadersTimeoutSeconds);
        });

        return builder;
    }
}
