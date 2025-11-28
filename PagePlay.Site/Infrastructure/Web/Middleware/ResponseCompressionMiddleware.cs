using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace PagePlay.Site.Infrastructure.Web.Middleware;

/// <summary>
/// Configures and enables response compression using Brotli and Gzip algorithms.
/// Brotli provides ~15-20% better compression than Gzip with minimal performance overhead.
/// Compression is enabled for HTTPS since the application uses CSRF tokens and JWTs,
/// which mitigate BREACH attack vectors.
///
/// IMPORTANT: This middleware must be registered BEFORE UseStaticFiles() in the pipeline
/// to ensure static assets (CSS, JS, images) are also compressed.
/// </summary>
public static class ResponseCompressionMiddleware
{
    /// <summary>
    /// Registers response compression services with optimized configuration.
    /// Call this in Program.cs during service configuration.
    /// </summary>
    public static IServiceCollection AddResponseCompressionMiddleware(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            // Enable compression over HTTPS
            // Safe for this application due to CSRF token and JWT architecture
            options.EnableForHttps = true;

            // Add compression providers (order matters - Brotli preferred by modern browsers)
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();

            // Configure MIME types to compress
            // Includes default types plus application/json for API responses
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "application/json",
                "text/html",
                "text/css",
                "application/javascript",
                "text/javascript",
                "application/xml",
                "text/xml",
                "image/svg+xml"
            });
        });

        // Configure Brotli compression provider
        // CompressionLevel.Fastest balances compression ratio with CPU usage
        // Typical savings: 60-70% size reduction on text content
        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        // Configure Gzip compression provider
        // CompressionLevel.Optimal provides best compression for Gzip
        // Used as fallback for older browsers that don't support Brotli
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        return services;
    }

    /// <summary>
    /// Applies response compression middleware to the application pipeline.
    /// Call this in Program.cs BEFORE app.UseStaticFiles() for maximum effectiveness.
    /// </summary>
    public static IApplicationBuilder UseResponseCompressionMiddleware(this IApplicationBuilder app)
    {
        app.UseResponseCompression();
        return app;
    }
}
