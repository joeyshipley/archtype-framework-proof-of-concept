namespace PagePlay.Site.Infrastructure.Core.Application;

/// <summary>
/// Abstraction for application logging.
/// Protects the application from direct dependencies on logging infrastructure (Serilog, ILogger, etc.).
/// Supports structured logging with properties for better observability.
/// The generic type parameter T is used as the source context for filtering logs by class.
/// </summary>
public interface ILogRecorder<T>
{
    /// <summary>
    /// Logs informational messages for general application flow.
    /// Use for: successful operations, state transitions, important milestones.
    /// </summary>
    void Info(string message, params object[] args);

    /// <summary>
    /// Logs warning messages for abnormal or unexpected events that don't prevent operation.
    /// Use for: deprecated API usage, poor use of API, recoverable errors, temporary failures.
    /// </summary>
    void Warn(string message, params object[] args);

    /// <summary>
    /// Logs error messages for failures and exceptions that prevent specific operations.
    /// Use for: caught exceptions, failed operations, data validation failures.
    /// </summary>
    void Error(string message, params object[] args);

    /// <summary>
    /// Logs error messages with exception details.
    /// Use for: caught exceptions with stack traces, unexpected errors.
    /// </summary>
    void Error(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs critical errors that cause application failure or data loss.
    /// Use for: unrecoverable errors, system failures, data corruption.
    /// </summary>
    void Critical(string message, params object[] args);

    /// <summary>
    /// Logs critical errors with exception details.
    /// Use for: unhandled exceptions, fatal errors, system crashes.
    /// </summary>
    void Critical(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs debug messages for detailed troubleshooting.
    /// Use for: variable values, control flow, detailed diagnostic information.
    /// Only logged when Debug level is enabled.
    /// </summary>
    void Debug(string message, params object[] args);

    /// <summary>
    /// Logs trace messages for very detailed diagnostic information.
    /// Use for: method entry/exit, detailed execution flow.
    /// Only logged when Trace level is enabled (typically only in development).
    /// </summary>
    void Trace(string message, params object[] args);
}
