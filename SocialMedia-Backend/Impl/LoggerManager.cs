namespace SocialMedia_Backend.Impl;

public class LoggerManager : ILogger
{
    private static ILogger _logger;
    public LoggerManager(ILogger logger) => _logger = logger;

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.LogError(exception.Message);
    }

    //public void LogDebug(string message)
    //{
    //    logger.Debug(message);
    //}
    //public void LogError(string message)
    //{
    //    logger.Error(message);
    //}
    //public void LogInfo(string message)
    //{
    //    logger.Info(message);
    //}
    //public void LogWarn(string message)
    //{
    //    logger.Warn(message);
    //}
}