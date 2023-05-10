using BepInEx.Logging;
using System.IO;
using System.Text;

namespace YLRandomizer.Logging
{
    public class BepInExLogger : DirectedLogger
    {
        private ManualLogSource _logger;

        public BepInExLogger(string logSource)
        {
            _logger = Logger.CreateLogSource(logSource);
        }

        public override void Log(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.CRITICAL:
                    _logger.LogFatal(message);
                    break;
                case LogLevel.ERROR:
                    _logger.LogError(message);
                    break;
                case LogLevel.WARNING:
                    _logger.LogWarning(message);
                    break;
                case LogLevel.INFO:
                    _logger.LogInfo(message);
                    break;
                case LogLevel.DEBUG:
                    _logger.LogDebug(message);
                    break;
                default:
                    _logger.LogError(message);
                    break;
            }
        }

        public override void Flush()
        {
        }
    }
}
