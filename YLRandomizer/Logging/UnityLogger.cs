namespace YLRandomizer.Logging
{
    public class UnityLogger : DirectedLogger
    {
        public override void Log(string message, LogLevel logLevel)
        {
            switch(logLevel)
            {
                case LogLevel.CRITICAL:
                case LogLevel.ERROR:
                    UnityEngine.Debug.LogError(message);
                    break;
                case LogLevel.WARNING:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                default:
                    UnityEngine.Debug.Log(message);
                    break;
            }
        }

        public override void Flush()
        {
        }
    }
}
