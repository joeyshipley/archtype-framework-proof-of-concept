# Performance Metrics Considerations

## What to Measure

**Request-level:**
- Duration distributions (p50, p95, p99 percentiles)
- Status code distribution (2xx, 4xx, 5xx)
- Request rate (requests/sec)

**Database:**
- Query count per request (detect N+1 queries)
- Query duration (identify slow queries > 100ms)
- Connection pool usage

**External Services:**
- API call latency
- Timeout and error rates

## Metrics Types

**Histogram**: Request duration, query duration (tracks distribution, enables percentiles)
**Counter**: Total requests, total errors (monotonically increasing)
**Gauge**: Active connections, memory usage (point-in-time values)

## Key Concerns

**Performance Overhead:**
- Metrics collection adds 1-5ms per request
- Sample high-traffic endpoints if needed (e.g., 1% sampling)
- Aggregate before export

**Cardinality Explosion:**
- ❌ BAD: Metrics per user ID or dynamic URL path (`/todos/12345`)
- ✅ GOOD: Metrics per route pattern (`/todos/{id}`), status code class, HTTP method
- High cardinality = memory explosion

## Implementation Phases

**Phase 1 (Current):**
- ✅ Request duration logged in `RequestLoggingMiddleware`
- Consider: Log slow requests (> 500ms) at WARN level

**Phase 2 (Next):**
- Add EF Core `IDbCommandInterceptor` to log slow queries (> 100ms)
- Track query count per request
- Log slow requests at WARN level for alerting

**Phase 3 (Production):**
- Integrate APM tool (Application Insights, Prometheus, DataDog)
- Export metrics with proper histogram buckets
- Set up alerting on p99 latency, error rates

## Technology Options

**APM/Monitoring:**
- Application Insights (Azure native, auto-instruments EF Core)
- Prometheus + Grafana (self-hosted, flexible)
- DataDog, New Relic (SaaS, comprehensive)

**Libraries:**
- `prometheus-net` - Prometheus metrics for .NET
- `App.Metrics` - Vendor-agnostic metrics library
- Application Insights SDK - Azure integration

## Current Architecture Integration

- `RequestLoggingMiddleware` already tracks duration and correlation IDs
- `Repository` class is ideal location for DB query instrumentation
- `ILogRecorder<T>` can be extended for structured metric logging
- Serilog can pipe to various sinks (Application Insights, Seq, ELK)

## Open Questions

- Which APM/monitoring tool for production?
- Sampling strategy for high-traffic endpoints?
- Alerting thresholds (p95 > 500ms? Error rate > 1%?)
