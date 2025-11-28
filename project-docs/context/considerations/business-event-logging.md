# Business Event Logging

## Purpose

Capture important business activities for audit trails, compliance, and business intelligence. Goes beyond technical request logging to track user actions and business outcomes.

## What to Log

**User Authentication:**
- Login attempts (success/failure, IP, timestamp)
- Logout events
- Password changes/resets
- Failed authentication attempts (potential security threats)
- JWT token issued/refreshed

**User Activity:**
- Account registration (email, timestamp)
- Profile updates
- Account deletion

**Business Operations:**
- Todo created/updated/deleted (who, what, when)
- Critical data changes with before/after values
- Admin actions (elevated privilege usage)

**Security Events:**
- Rate limit violations
- Suspicious activity patterns
- Permission denied attempts

## Implementation Approaches

**Option 1: Structured Logging (Current)**
```csharp
_log.Info("User registered: {Email} | UserId: {UserId} | IP: {IpAddress}",
    email, userId, ipAddress);
```
- Pros: Simple, uses existing ILogRecorder
- Cons: Mixed with technical logs, no strong typing

**Option 2: Dedicated Event Service**
```csharp
public interface IBusinessEventRecorder
{
    void UserRegistered(long userId, string email, string ipAddress);
    void LoginAttempt(string email, bool success, string ipAddress);
    void TodoCreated(long userId, long todoId);
}
```
- Pros: Strongly typed, clear intent, separate from technical logs
- Cons: More abstraction, additional interface

**Option 3: Domain Events**
```csharp
// Raised in domain layer, logged via event handler
public class UserRegisteredEvent : IDomainEvent
{
    public long UserId { get; init; }
    public string Email { get; init; }
}
```
- Pros: Follows DDD patterns, decoupled, extensible
- Cons: More complex, overkill for simple apps

## Recommended Approach

**Start simple (Option 1):**
- Use existing `ILogRecorder<T>` with structured parameters
- Create consistent log message formats for each event type
- Use INFO level for successful events, WARN for suspicious activity

**Future (Option 2):**
- Introduce `IBusinessEventRecorder` when business events become numerous
- Keeps business logic separate from technical logging
- Easier to route to separate storage or analytics systems

## Data to Include

**Standard fields for all events:**
- UserId (or "anonymous")
- Event type/name
- Timestamp (automatic via logger)
- Correlation ID (from RequestLoggingMiddleware)
- IP address
- User agent (for authentication events)

**Event-specific data:**
- Entity IDs (TodoId, etc.)
- Before/after values for updates
- Failure reasons for errors

## Storage Considerations

**Same as application logs:**
- Simple to implement
- Good for development and small scale
- Use structured logging for easy querying

**Separate storage:**
- Dedicated audit database table
- Write-only append log for compliance
- Separate retention policies (audit logs kept longer)

## Security & Compliance

- Never log passwords or sensitive tokens
- Consider PII regulations (GDPR, CCPA)
- Redact sensitive data (credit cards, SSNs)
- Implement log retention policies
- Ensure logs are tamper-proof for compliance

## Open Questions

- Separate audit log storage or same as application logs?
- Retention policy for business events (1 year? 7 years for compliance?)
- Do we need real-time event streaming to analytics?
- Which events require before/after value tracking?
