namespace YLRandomizer.Logging
{
    public interface ILogger
    {
        void Log(string message, LogLevel logLevel);
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Critical(string message);
        void Flush();
    }

    public enum LogLevel
    {
        /// <summary>
        /// Highest severity level. A significant error has occurred which has high impact on
        /// the application.
        /// </summary>
        CRITICAL = 1,
        /// <summary>
        /// An error that immediately and negatively impacts the application has occurred.
        /// </summary>
        ERROR = 2,
        /// <summary>
        /// A significant event has occurred. This generally does not immediately negatively
        /// impact the application, but may be a precursor for such events.
        /// </summary>
        WARNING = 3,
        /// <summary>
        /// An event has occurred, and may be of interest to someone like a developer, but is
        /// unlikely to be useful to an end user.
        /// </summary>
        INFO = 4,
        /// <summary>
        /// Excess info that does not need to be logged unless requested specifically by a
        /// developer.
        /// </summary>
        DEBUG = 5
    }
}
