namespace PagePlay.Site.Infrastructure.Core.Application;

/// <summary>
/// Abstraction for application logging.
/// Protects the application from direct dependencies on logging infrastructure (Serilog, ILogger, etc.).
/// Supports structured logging with properties for better observability.
/// The generic type parameter T is used as the source context for filtering logs by class.
/// </summary>
public interface ILogProvider<T>
{
    /// <summary>
    /// Logs informational messages for general application flow.
    /// Use for: successful operations, state transitions, important milestones.
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs warning messages for abnormal or unexpected events that don't prevent operation.
    /// Use for: deprecated API usage, poor use of API, recoverable errors, temporary failures.
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs error messages for failures and exceptions that prevent specific operations.
    /// Use for: caught exceptions, failed operations, data validation failures.
    /// </summary>
    void LogError(string message, params object[] args);

    /// <summary>
    /// Logs error messages with exception details.
    /// Use for: caught exceptions with stack traces, unexpected errors.
    /// </summary>
    void LogError(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs critical errors that cause application failure or data loss.
    /// Use for: unrecoverable errors, system failures, data corruption.
    /// </summary>
    void LogCritical(string message, params object[] args);

    /// <summary>
    /// Logs critical errors with exception details.
    /// Use for: unhandled exceptions, fatal errors, system crashes.
    /// </summary>
    void LogCritical(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs debug messages for detailed troubleshooting.
    /// Use for: variable values, control flow, detailed diagnostic information.
    /// Only logged when Debug level is enabled.
    /// </summary>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Logs trace messages for very detailed diagnostic information.
    /// Use for: method entry/exit, detailed execution flow.
    /// Only logged when Trace level is enabled (typically only in development).
    /// </summary>
    void LogTrace(string message, params object[] args);
}
