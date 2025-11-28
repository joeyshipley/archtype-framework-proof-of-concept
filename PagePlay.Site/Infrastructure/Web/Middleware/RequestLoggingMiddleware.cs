using System.Diagnostics;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Infrastructure.Web.Middleware;

/// <summary>
/// Middleware that logs HTTP request and response information for observability.
/// Logs: method, path, query string, status code, duration, user ID, and correlation ID.
/// Does not log request/response bodies for performance reasons.
/// </summary>
public class RequestLoggingMiddleware(
    RequestDelegate _next,
    ILogRecorder<RequestLoggingMiddleware> _log)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, ICurrentUserContext currentUserContext)
    {
        // Generate or retrieve correlation ID
        var correlationId = GetOrCreateCorrelationId(context);

        // Start timing the request
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Call the next middleware
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log the completed request
            LogRequest(context, currentUserContext, correlationId, stopwatch.ElapsedMilliseconds);
        }
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if correlation ID exists in request headers
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId)
            && !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        // Generate new correlation ID
        var newCorrelationId = Guid.NewGuid().ToString();

        // Add to response headers so clients can track it
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = newCorrelationId;
            return Task.CompletedTask;
        });

        return newCorrelationId;
    }

    private void LogRequest(
        HttpContext context,
        ICurrentUserContext currentUserContext,
        string correlationId,
        long durationMs)
    {
        var request = context.Request;
        var response = context.Response;

        var logMessage = "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {Duration}ms | User: {UserId} | IP: {IpAddress} | CorrelationId: {CorrelationId}";

        var args = new object[]
        {
            request.Method,
            request.Path,
            request.QueryString.HasValue ? request.QueryString.Value : string.Empty,
            response.StatusCode,
            durationMs,
            currentUserContext.UserId?.ToString() ?? "anonymous",
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            correlationId
        };

        // Log at appropriate level based on status code
        if (response.StatusCode >= 500)
        {
            _log.Error(logMessage, args);
        }
        else if (response.StatusCode >= 400)
        {
            _log.Warn(logMessage, args);
        }
        else
        {
            _log.Info(logMessage, args);
        }
    }
}
