using System;
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

        public static void PrintFullErrorDetails(Exception e)
        {
            int errorIteration = 0;
            while (e != null && errorIteration < 100) // 100 is the max depth we should go, we're all but guaranteed to be infinitely looping at that point
            {
                ManualSingleton<ILogger>.instance.Error($"=== {errorIteration++} ===");
                ManualSingleton<ILogger>.instance.Error(e.Message);
                ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                if (e.InnerException != e)
                {
                    e = e.InnerException;
                }
                else
                {
                    e = null;
                }
            }
        }
    }
}
