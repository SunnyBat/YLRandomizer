using System.IO;
using System.Text;

namespace YLRandomizer.Logging
{
    public class FileLogger : DirectedLogger
    {
        FileStream outputStream;
        public FileLogger(string path)
        {
            outputStream = File.OpenWrite(path);
        }

        public override void Log(string message, LogLevel logLevel)
        {
            if (outputStream != null)
            {
                try
                {
                    var toWrite = Encoding.UTF8.GetBytes(message + "\r\n");
                    outputStream.Write(toWrite, 0, toWrite.Length);
                    Flush();
                }
                catch { }
            }
        }

        public override void Flush()
        {
            outputStream.Flush();
        }
    }
}
