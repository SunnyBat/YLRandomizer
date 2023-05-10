using System;

namespace YLRandomizer.Logging
{
    public class UnityILoggerImpl : UnityEngine.ILogger
    {
        private ILogger _log;

        public UnityILoggerImpl(ILogger log)
        {
            _log = log;
        }

        public UnityEngine.ILogHandler logHandler { get; set; }
        public bool logEnabled { get; set; }
        public UnityEngine.LogType filterLogType { get; set; }

        public bool IsLogTypeAllowed(UnityEngine.LogType logType)
        {
            return logType != UnityEngine.LogType.Assert;
        }

        public void Log(UnityEngine.LogType logType, object message)
        {
            _log.Info(message?.ToString());
        }

        public void Log(UnityEngine.LogType logType, object message, UnityEngine.Object context)
        {
            _log.Info(message?.ToString());
        }

        public void Log(UnityEngine.LogType logType, string tag, object message)
        {
            _log.Info(message?.ToString());
        }

        public void Log(UnityEngine.LogType logType, string tag, object message, UnityEngine.Object context)
        {
            _log.Info(message?.ToString());
        }

        public void Log(object message)
        {
            _log.Info(message?.ToString());
        }

        public void Log(string tag, object message)
        {
            _log.Info(message?.ToString());
        }

        public void Log(string tag, object message, UnityEngine.Object context)
        {
            _log.Info(message?.ToString());
        }

        public void LogError(string tag, object message)
        {
            _log.Info(message?.ToString());
        }

        public void LogError(string tag, object message, UnityEngine.Object context)
        {
            _log.Info(message?.ToString());
        }

        public void LogException(Exception exception)
        {
            _log.Info(exception?.Message);
            _log.Info(exception?.StackTrace.ToString());
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            _log.Info(exception?.Message);
            _log.Info(exception?.StackTrace.ToString());
        }

        public void LogFormat(UnityEngine.LogType logType, string format, params object[] args)
        {
            if (!string.IsNullOrEmpty(format))
            {
                _log.Info(string.Format(format, args));
            }
        }

        public void LogFormat(UnityEngine.LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (!string.IsNullOrEmpty(format))
            {
                _log.Info(string.Format(format, args));
            }
        }

        public void LogWarning(string tag, object message)
        {
            _log.Info(message?.ToString());
        }

        public void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            _log.Info(message?.ToString());
        }
    }
}
