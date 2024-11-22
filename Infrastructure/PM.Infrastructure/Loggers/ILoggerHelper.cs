namespace PM.Infrastructure.Loggers
{
    public interface ILoggerHelper<T>
    {
        public void LogInfo(string message);
        public void LogError(Exception ex, string message);
        public void LogDebug(string message);
        public void LogWarning(string message);
    }
}