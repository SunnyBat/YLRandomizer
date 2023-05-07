using System;

namespace YLRandomizer.Logging
{
    public class BlendedLogger : DirectedLogger
    {
        private readonly ILogger[] _loggers;

        public BlendedLogger(params ILogger[] loggers)
        {
            if (loggers == null || loggers.Length == 0)
            {
                throw new ArgumentException("Must specify at least one logger");
            }
            this._loggers = loggers;
        }

        public override void Log(string message, LogLevel logLevel)
        {
            for (int i = 0; i < this._loggers.Length; i++)
            {
                this._loggers[i].Log(message, logLevel);
            }
        }

        public override void Flush()
        {
            for (int i = 0; i < this._loggers.Length; i++)
            {
                this._loggers[i].Flush();
            }
        }
    }
}
