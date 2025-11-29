using PagePlay.Site.Infrastructure.Core.Application;

namespace PagePlay.Tests.Infrastructure.Logging;

/// <summary>
/// Null implementation of ILogRecorder for testing.
/// Discards all log messages (no-op).
/// </summary>
public class FakeLogRecorder<T> : ILogRecorder<T>
{
    public void Info(string message, params object[] args) { }
    public void Warn(string message, params object[] args) { }
    public void Error(string message, params object[] args) { }
    public void Error(Exception exception, string message, params object[] args) { }
    public void Critical(string message, params object[] args) { }
    public void Critical(Exception exception, string message, params object[] args) { }
    public void Debug(string message, params object[] args) { }
    public void Trace(string message, params object[] args) { }
}
