namespace PagePlay.Site.Infrastructure.Web.Middleware;

/// <summary>
/// Middleware that adds security headers to HTTP responses.
/// Configured differently for development vs production environments.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public SecurityHeadersMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Safe in all environments - basic security headers
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        if (_env.IsDevelopment())
        {
            // Development: Fully permissive CSP for ease of development
            // Allows inline scripts, inline styles, eval, and all external sources
            context.Response.Headers["Content-Security-Policy"] =
                "default-src * 'unsafe-inline' 'unsafe-eval' data: blob:";
        }
        else
        {
            // Production: Strict CSP
            // NOTE: 'unsafe-inline' still required for styles (CSS-in-HTML from server)
            // Remaining blockers for full strictness:
            // 1. Pages/Todos/Todos.Page.htmx.cs:42 - hx-on::after-request inline handler
            // 2. Pages/Todos/Todos.Page.htmx.cs:87 - onclick inline handler
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' https://unpkg.com; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data:; " +
                "font-src 'self' data:; " +
                "connect-src 'self'; " +
                "frame-ancestors 'self'";

            // HSTS: Force HTTPS for 1 year (only in production)
            context.Response.Headers["Strict-Transport-Security"] =
                "max-age=31536000; includeSubDomains";
        }

        await _next(context);
    }
}
