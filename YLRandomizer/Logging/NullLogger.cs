namespace YLRandomizer.Logging
{
    public class NullLogger : DirectedLogger
    {
        public override void Log(string message, LogLevel logLevel)
        {
        }
    }
}
