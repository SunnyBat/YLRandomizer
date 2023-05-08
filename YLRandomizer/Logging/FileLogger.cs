using System.IO;
using System.Text;

namespace YLRandomizer.Logging
{
    public class FileLogger : DirectedLogger
    {
        FileStream logOutputStream;
        FileStream errorOutputStream;
        public FileLogger(string errorPath, string logPath = null)
        {
            File.Delete(errorPath);
            errorOutputStream = File.OpenWrite(errorPath);
            if (!string.IsNullOrEmpty(logPath))
            {
                File.Delete(logPath);
                logOutputStream = File.OpenWrite(logPath);
            }
        }

        public override void Log(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.CRITICAL:
                case LogLevel.ERROR:
                case LogLevel.WARNING:
                    if (errorOutputStream != null)
                    {
                        try
                        {
                            var toWrite = Encoding.UTF8.GetBytes(logLevel + ": " + message + "\r\n");
                            errorOutputStream.Write(toWrite, 0, toWrite.Length);
                        }
                        catch { }
                    }
                    break;
                default:
                    if (logOutputStream != null)
                    {
                        try
                        {
                            var toWrite = Encoding.UTF8.GetBytes(logLevel + ": " + message + "\r\n");
                            logOutputStream.Write(toWrite, 0, toWrite.Length);
                        }
                        catch { }
                    }
                    break;
            }
        }

        public override void Flush()
        {
            logOutputStream?.Flush();
            errorOutputStream?.Flush();
        }
    }
}
