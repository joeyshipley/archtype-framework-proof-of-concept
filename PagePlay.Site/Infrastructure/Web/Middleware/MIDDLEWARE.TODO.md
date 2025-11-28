# Middleware TODO List

Production-grade middleware components to implement as the application matures.

## High Priority (Must-have for production)

- [ ] **Request Logging / Observability**
  - Add structured logging for every request (path, method, status, duration)
  - Consider Serilog request logging or Application Insights
  - Essential for debugging production issues

- [x] **Rate Limiting**
  - ~~Implement rate limiting middleware using .NET 7+ built-in `UseRateLimiter()`~~
  - ~~Configure limits for authentication endpoints (`/login`, `/register`)~~
  - ~~Protect API endpoints from brute force and DoS attacks~~
  - ~~Consider per-IP and per-user rate limits~~
  - **STATUS**: ✅ Implemented with custom in-memory middleware
  - **IMPLEMENTATION**:
    - `RateLimitingMiddleware` applies globally to all routes (250 req/min per user/IP)
    - Per-user rate limiting for authenticated requests (by JWT UserId)
    - Per-IP rate limiting for anonymous requests
    - Configurable via `appsettings.json` with excluded paths (/health, static assets)
    - Automatic cleanup of expired entries, HTTP 429 responses with Retry-After headers
    - Note: Opted for custom middleware over built-in `UseRateLimiter()` for flexibility

- [x] **Security Headers**
  - ~~Implement security headers middleware (CSP, X-Frame-Options, X-Content-Type-Options, etc.)~~
  - ~~Consider using NWebsec library or custom middleware~~
  - ~~Provides defense-in-depth against XSS, clickjacking, MIME sniffing attacks~~
  - **STATUS**: Middleware created but requires refactoring inline scripts before production deployment
  - **BLOCKERS**:
    - `Pages/Todos/Todos.Page.htmx.cs:42` - `hx-on::after-request` inline handler
    - `Pages/Todos/Todos.Page.htmx.cs:87` - `onclick` inline handler
    - `Pages/Shared/Layout.htmx.cs:33-40` - inline `<script>` for CSRF token
  - **ACTION REQUIRED**: Move all inline JavaScript to external files before enabling in production

- [ ] **Health Checks**
  - Add health check endpoints (`/health`, `/health/ready`)
  - Include database connectivity checks
  - Required for Kubernetes, load balancers, and uptime monitoring

## Medium Priority (Should have)

- [ ] **Response Compression**
  - Enable Gzip/Brotli compression for bandwidth optimization
  - Must be added early in the pipeline
  - Easy performance win that reduces bandwidth costs

- [ ] **Correlation ID / Request Tracing**
  - Add unique ID per request for distributed tracing
  - Use Activity.Current or generate GUID
  - Essential for tracking requests across logs in distributed systems

## Low Priority (Nice to have)

- [ ] **Request Size Limits**
  - Configure explicit request body size limits beyond Kestrel defaults
  - Protect against memory exhaustion from large payloads
  - Usually configured in Kestrel options

## Notes

- Current authentication is properly handled by JWT bearer middleware and endpoint filters
- Session middleware is not needed (JWT is stateless)
- Age gate functionality should likely be an endpoint filter, not global middleware
- When implementing these, create actual middleware classes in this folder and remove the empty stub files
