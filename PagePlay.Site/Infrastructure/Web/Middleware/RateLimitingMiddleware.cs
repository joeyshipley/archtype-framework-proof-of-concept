using System.Collections.Concurrent;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Infrastructure.Web.Middleware;

/// <summary>
/// Middleware that applies rate limiting to all requests.
/// Uses in-memory storage with automatic cleanup of expired entries.
/// Configuration is loaded from appsettings.json under "RateLimiting" section.
/// Rate limits are applied per-user (using JWT UserId claim) for authenticated requests,
/// and per-IP address for anonymous requests.
/// </summary>
public class RateLimitingMiddleware(
    RequestDelegate _next,
    ISettingsProvider _settingsProvider,
    IUserIdentityService _userIdentityService,
    ILogRecorder<RateLimitingMiddleware> _logger)
{
    // In-memory storage: Key = partition key (user/IP), Value = rate limiter state
    private static readonly ConcurrentDictionary<string, RateLimiterState> _limiters = new();

    // Background cleanup to remove expired entries
    private static readonly Timer _cleanupTimer = new(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for excluded paths
        if (IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Determine the partition key (who to rate limit)
        var partitionKey = GetPartitionKey(context);

        // Get or create rate limiter for this partition
        var limiter = _limiters.GetOrAdd(partitionKey, _ => new RateLimiterState());

        // Check if request is allowed
        if (!limiter.TryAcquire(_settingsProvider.RateLimiting.RequestsPerMinute))
        {
            _logger.Warn(
                "Rate limit exceeded for {PartitionKey}. Path: {Path}",
                partitionKey,
                context.Request.Path
            );

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = "60";
            context.Response.Headers["X-RateLimit-Limit"] = _settingsProvider.RateLimiting.RequestsPerMinute.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";

            await context.Response.WriteAsync("Too many requests. Please slow down and try again later.");
            return;
        }

        // Add rate limit headers to response
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-RateLimit-Limit"] = _settingsProvider.RateLimiting.RequestsPerMinute.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = limiter.GetRemainingRequests(_settingsProvider.RateLimiting.RequestsPerMinute).ToString();
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private string GetPartitionKey(HttpContext context)
    {
        // For authenticated users, key by user ID from JWT claims
        if (_userIdentityService.IsAuthenticated(context.User))
        {
            var userId = _userIdentityService.GetUserIdString(context.User);
            if (!string.IsNullOrEmpty(userId))
                return $"user:{userId}";
        }

        // For anonymous users, key by IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
        return $"ip:{ipAddress}";
    }

    private bool IsExcludedPath(PathString path)
    {
        foreach (var excludedPath in _settingsProvider.RateLimiting.ExcludedPaths)
        {
            if (path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private static void CleanupExpiredEntries(object? state)
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _limiters
            .Where(kvp => (now - kvp.Value.WindowStart).TotalMinutes > 2) // Keep for 2 minutes after window
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _limiters.TryRemove(key, out _);
        }
    }
}

/// <summary>
/// Internal state for a single rate limiter partition (user or IP).
/// Uses sliding window algorithm for smooth rate limiting.
/// </summary>
internal class RateLimiterState
{
    private readonly object _lock = new();
    private DateTime _windowStart = DateTime.UtcNow;
    private int _requestCount = 0;

    public bool TryAcquire(int limit)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var elapsed = now - _windowStart;

            // Reset window if more than 1 minute has passed
            if (elapsed.TotalMinutes >= 1)
            {
                _windowStart = now;
                _requestCount = 0;
            }

            // Check if under limit
            if (_requestCount >= limit)
                return false;

            // Allow request
            _requestCount++;
            return true;
        }
    }

    public int GetRemainingRequests(int limit)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var elapsed = now - _windowStart;

            // Reset window if expired
            if (elapsed.TotalMinutes >= 1)
            {
                return limit;
            }

            return Math.Max(0, limit - _requestCount);
        }
    }

    public DateTime WindowStart => _windowStart;
}
