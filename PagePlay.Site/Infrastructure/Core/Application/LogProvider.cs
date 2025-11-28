namespace PagePlay.Site.Infrastructure.Core.Application;

/// <summary>
/// Default implementation of ILogProvider that wraps Microsoft.Extensions.Logging.ILogger.
/// This allows the application to remain decoupled from specific logging infrastructure (Serilog, NLog, etc.).
/// The underlying ILogger can be configured to use any provider (Console, File, Serilog, Application Insights, etc.).
/// </summary>
public class LogProvider : ILogProvider
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, object> _properties;

    public LogProvider(ILogger<LogProvider> logger)
    {
        _logger = logger;
        _properties = new Dictionary<string, object>();
    }

    // Internal constructor for creating scoped loggers
    private LogProvider(ILogger logger, Dictionary<string, object> properties)
    {
        _logger = logger;
        _properties = new Dictionary<string, object>(properties);
    }

    public void LogInformation(string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogInformation(message, args);
            }
        }
        else
        {
            _logger.LogInformation(message, args);
        }
    }

    public void LogWarning(string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogWarning(message, args);
            }
        }
        else
        {
            _logger.LogWarning(message, args);
        }
    }

    public void LogError(string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogError(message, args);
            }
        }
        else
        {
            _logger.LogError(message, args);
        }
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogError(exception, message, args);
            }
        }
        else
        {
            _logger.LogError(exception, message, args);
        }
    }

    public void LogCritical(string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogCritical(message, args);
            }
        }
        else
        {
            _logger.LogCritical(message, args);
        }
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogCritical(exception, message, args);
            }
        }
        else
        {
            _logger.LogCritical(exception, message, args);
        }
    }

    public void LogDebug(string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogDebug(message, args);
            }
        }
        else
        {
            _logger.LogDebug(message, args);
        }
    }

    public void LogTrace(string message, params object[] args)
    {
        if (_properties.Count > 0)
        {
            using (_logger.BeginScope(_properties))
            {
                _logger.LogTrace(message, args);
            }
        }
        else
        {
            _logger.LogTrace(message, args);
        }
    }

    public ILogProvider ForContext<T>()
    {
        // Create a new logger with the type context
        var loggerFactory = _logger as ILoggerFactory
            ?? throw new InvalidOperationException("Cannot create typed logger - ILoggerFactory not available");

        var typedLogger = loggerFactory.CreateLogger<T>();
        return new LogProvider(typedLogger, _properties);
    }

    public ILogProvider WithProperty(string propertyName, object value)
    {
        // Create a new scoped logger with the additional property
        var newProperties = new Dictionary<string, object>(_properties)
        {
            [propertyName] = value
        };
        return new LogProvider(_logger, newProperties);
    }
}
