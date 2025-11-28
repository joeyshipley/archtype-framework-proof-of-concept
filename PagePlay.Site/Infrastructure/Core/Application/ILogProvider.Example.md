# ILogProvider<T> Usage Examples

This document demonstrates how to use the ILogProvider<T> abstraction throughout the application.

## Basic Usage

### 1. Constructor Injection

```csharp
public class MyService
{
    private readonly ILogProvider<MyService> _logger;

    public MyService(ILogProvider<MyService> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("Service is doing something");
        // Log output: [INFO] MyService: Service is doing something
        //                    ^^^^^^^^^  <- Automatically included via generic parameter
    }
}
```

### 2. Log Levels

```csharp
// Debug - Detailed diagnostic information (only in Development)
_logger.LogDebug("Processing item {ItemId}", itemId);

// Information - General application flow
_logger.LogInformation("User {UserId} logged in successfully", userId);

// Warning - Abnormal or unexpected events (not errors)
_logger.LogWarning("User {UserId} attempted to access restricted resource", userId);

// Error - Operation failures
_logger.LogError("Failed to process payment for order {OrderId}", orderId);

// Error with exception
try {
    // some operation
} catch (Exception ex) {
    _logger.LogError(ex, "Failed to update user profile for {UserId}", userId);
}

// Critical - Application failure or data loss
_logger.LogCritical("Database connection lost - application cannot continue");
```

## Advanced Usage

### 3. Structured Logging with Properties

```csharp
// Properties are captured in {braces} for structured logging
_logger.LogInformation(
    "User {UserId} created order {OrderId} with {ItemCount} items totaling {Total:C}",
    userId,
    orderId,
    itemCount,
    totalAmount
);

// This creates structured log entries that can be queried:
// - UserId = 123
// - OrderId = "ORD-456"
// - ItemCount = 5
// - Total = 99.99
```

### 4. Filtering Logs by Source Context

Since each logger includes its type via the generic parameter, you can easily filter logs:

```csharp
// In Serilog configuration (appsettings.json):
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "MyNamespace.OrderService": "Debug",  // Enable Debug logs for OrderService only
      "MyNamespace.PaymentService": "Warning"  // Only warnings and above for PaymentService
    }
  }
}
```

## Real-World Examples

### Example: Service Layer

```csharp
public class OrderService
{
    private readonly ILogProvider<OrderService> _logger;
    private readonly IRepository _repository;

    public OrderService(ILogProvider<OrderService> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<Order>> CreateOrderAsync(CreateOrderRequest request)
    {
        _logger.LogInformation(
            "Creating order for user {UserId} with {ItemCount} items",
            request.UserId,
            request.Items.Count
        );

        try
        {
            var order = new Order
            {
                UserId = request.UserId,
                Items = request.Items,
                Total = request.Items.Sum(i => i.Price)
            };

            await _repository.AddAsync(order);

            _logger.LogInformation(
                "Order {OrderId} created successfully for user {UserId}",
                order.Id,
                request.UserId
            );

            return Result.Success(order);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                "Order validation failed for user {UserId}: {ValidationErrors}",
                request.UserId,
                string.Join(", ", ex.Errors)
            );
            return Result.Failure<Order>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create order for user {UserId}",
                request.UserId
            );
            throw;
        }
    }
}
```

### Example: Middleware

```csharp
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogProvider<CustomMiddleware> _logger;

    public CustomMiddleware(RequestDelegate next, ILogProvider<CustomMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogDebug(
            "Processing {Method} request to {Path}",
            context.Request.Method,
            context.Request.Path
        );

        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation(
                "Completed {Method} {Path} with {StatusCode} in {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds
            );
        }
    }
}
```

## Configuration

Logging behavior is configured in `appsettings.json` under the `Serilog` section:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/pageplay-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Best Practices

1. **Use structured logging** - Always use properties instead of string interpolation:
   ```csharp
   // Good
   _logger.LogInformation("User {UserId} logged in", userId);

   // Bad
   _logger.LogInformation($"User {userId} logged in");
   ```

2. **Choose appropriate log levels**:
   - `Debug/Trace`: Only for development troubleshooting
   - `Information`: Normal application flow, important milestones
   - `Warning`: Abnormal situations that don't prevent operation
   - `Error`: Operation failures, caught exceptions
   - `Critical`: Application failures, data loss

3. **Log at boundaries**:
   - HTTP requests/responses
   - External API calls
   - Database operations
   - Authentication/authorization events
   - Business-critical operations

4. **Don't log sensitive data**:
   - Never log passwords, tokens, credit cards
   - Be careful with PII (personally identifiable information)
   - Use masking if needed: `_logger.LogInformation("Email: {Email}", MaskEmail(email))`

5. **Use structured properties for correlation**:
   - Add correlation data as parameters: `_logger.LogInformation("Processing {RequestId}", requestId)`
   - User IDs: `_logger.LogInformation("User {UserId} action", userId)`
   - Serilog will capture these as queryable properties

## Migrating from ILogger<T>

If you have existing code using `ILogger<T>`, replace it with `ILogProvider<T>`:

```csharp
// Before
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
}

// After
public class MyService
{
    private readonly ILogProvider<MyService> _logger;

    public MyService(ILogProvider<MyService> logger)
    {
        _logger = logger;
    }
}
```

The API is intentionally identical to ILogger<T> to make migration straightforward - just change the interface name!
