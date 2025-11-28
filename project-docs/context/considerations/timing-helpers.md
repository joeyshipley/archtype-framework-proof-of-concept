# Timing Helpers for Performance Tracking

## Purpose

Measure operation duration for performance monitoring and optimization. Identifies slow code paths and helps track performance regressions.

## Use Cases

**Database operations:**
- Repository method execution time
- Query performance tracking
- Transaction duration

**External API calls:**
- HTTP client request duration
- Third-party service latency

**Business operations:**
- Complex calculations
- Bulk data processing
- Report generation

**Cache operations:**
- Cache hit/miss timing
- Cache warm-up duration

## Implementation Options

**Option 1: Disposable Timer (Recommended)**
```csharp
using (_log.Timed("DatabaseQuery"))
{
    // Operation to measure
}
// Automatically logs duration on dispose
```
- Pros: Clean syntax, exception-safe, automatic logging
- Cons: Requires implementing IDisposable helper

**Option 2: Manual Stopwatch**
```csharp
var sw = Stopwatch.StartNew();
try
{
    // Operation
}
finally
{
    _log.Debug("Operation completed in {Duration}ms", sw.ElapsedMilliseconds);
}
```
- Pros: Simple, no new abstractions
- Cons: Verbose, easy to forget logging, error-prone

**Option 3: Decorator Pattern**
```csharp
public class TimedRepository : IRepository
{
    private readonly IRepository _inner;
    private readonly ILogRecorder<TimedRepository> _log;

    public async Task<T> Get<T>(Specification<T> spec)
    {
        using (_log.Timed("Repository.Get"))
        {
            return await _inner.Get(spec);
        }
    }
}
```
- Pros: Transparent, no changes to business logic
- Cons: Boilerplate for each method, more complex

**Option 4: AOP/Interceptors**
```csharp
[Timed] // Attribute-based
public async Task<User> GetUser(long id) { ... }
```
- Pros: Very clean, minimal code changes
- Cons: Requires AOP framework (Castle DynamicProxy, etc.), complexity

## Recommended Implementation

**Create a simple timing helper:**
```csharp
public static class LogRecorderExtensions
{
    public static IDisposable Timed(
        this ILogRecorder log,
        string operationName)
    {
        return new TimedOperation(log, operationName);
    }
}

internal class TimedOperation : IDisposable
{
    private readonly ILogRecorder _log;
    private readonly string _operationName;
    private readonly Stopwatch _stopwatch;

    public TimedOperation(ILogRecorder log, string operationName)
    {
        _log = log;
        _operationName = operationName;
        _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        var duration = _stopwatch.ElapsedMilliseconds;

        // Log at appropriate level based on duration
        if (duration > 1000)
            _log.Warn("{Operation} took {Duration}ms", _operationName, duration);
        else if (duration > 100)
            _log.Info("{Operation} took {Duration}ms", _operationName, duration);
        else
            _log.Debug("{Operation} took {Duration}ms", _operationName, duration);
    }
}
```

**Usage:**
```csharp
public async Task<User> GetUser(long id)
{
    using (_log.Timed("GetUser"))
    {
        return await _repository.Get(new UserByIdSpec(id));
    }
}
```

## What to Measure

**High-value targets:**
- Database queries (especially potential N+1 queries)
- External API calls
- File I/O operations
- Complex calculations/algorithms
- Operations with SLAs

**Avoid over-measuring:**
- Simple property getters/setters
- In-memory operations (< 1ms)
- High-frequency operations (creates noise)

## Thresholds & Alerting

**Suggested thresholds:**
- < 10ms: DEBUG level (normal, low noise)
- 10-100ms: INFO level (visible but acceptable)
- 100-1000ms: WARN level (slow, investigate)
- \> 1000ms: ERROR level (unacceptable, alert)

**Context matters:**
- Adjust thresholds per operation type
- Database queries: > 100ms is slow
- External APIs: > 500ms might be normal
- Report generation: > 5s might be acceptable

## Integration with Metrics

Timing helpers complement performance metrics:
- **Logs**: Individual slow operations (debugging)
- **Metrics**: Aggregate performance (dashboards, alerting)

Consider:
- Emitting metrics alongside logs
- Structured logging for easy metric extraction
- Correlation IDs to trace slow request chains

## Open Questions

- Should timing helpers emit metrics or just logs?
- Default thresholds for different operation types?
- Sample high-frequency operations to reduce overhead?
