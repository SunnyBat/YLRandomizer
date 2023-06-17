using System.Diagnostics;
using System.Linq;
using YLRandomizer.Logging;

namespace YLRandomizer
{
    public class Utilities
    {
        public static void PrintStack(LogLevel logLevel = LogLevel.DEBUG)
        {
            var stackFrames = new StackTrace().GetFrames();
            foreach (StackFrame frame in stackFrames)
            {
                // Access information about each stack frame
                string methodName = frame.GetMethod().Name;
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();

                // Do something with the stack frame information
                ManualSingleton<ILogger>.instance.Log($"\tMethod: {methodName}, File: {fileName}, Line: {lineNumber}", logLevel);
            }
        }

        public static bool StackHasMethods(params string[] methodNames)
        {
            var stackFrames = new StackTrace().GetFrames();
            foreach (StackFrame frame in stackFrames)
            {
                if (methodNames.Contains(frame.GetMethod().Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
