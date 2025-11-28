namespace PagePlay.Site.Infrastructure.Core.Application;

/// <summary>
/// Default implementation of ILogProvider that wraps Microsoft.Extensions.Logging.ILogger.
/// This allows the application to remain decoupled from specific logging infrastructure (Serilog, NLog, etc.).
/// The underlying ILogger can be configured to use any provider (Console, File, Serilog, Application Insights, etc.).
/// The generic type parameter T is used as the source context for filtering logs by class.
/// </summary>
public class LogProvider<T> : ILogProvider<T>
{
    private readonly ILogger<T> _logger;

    public LogProvider(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public void LogCritical(string message, params object[] args)
    {
        _logger.LogCritical(message, args);
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        _logger.LogCritical(exception, message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogTrace(string message, params object[] args)
    {
        _logger.LogTrace(message, args);
    }
}
