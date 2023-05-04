
namespace YLRandomizer.Logging
{
    public abstract class DirectedLogger : ILogger
    {
        public abstract void Log(string message, LogLevel logLevel);

        public void Critical(string message)
        {
            Log(message, LogLevel.CRITICAL);
        }

        public void Debug(string message)
        {
            Log(message, LogLevel.DEBUG);
        }

        public void Error(string message)
        {
            Log(message, LogLevel.ERROR);
        }

        public void Info(string message)
        {
            Log(message, LogLevel.INFO);
        }

        public void Warning(string message)
        {
            Log(message, LogLevel.WARNING);
        }
    }
}
