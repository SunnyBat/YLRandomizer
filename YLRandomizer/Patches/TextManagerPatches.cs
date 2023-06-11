using HarmonyLib;
using System.Diagnostics;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(TextManager), nameof(TextManager.GetStrings))]
    public class TextManager_GetStrings
    {
        [HarmonyPostfix]
        public static void Postfix(string table, ref string[] __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"TextManager_GetStrings.Postfix(): {table}");
            if (table == "Hub World")
            {
                //if (__result != null)
                //{
                //    for (int i = 0; i < __result.Length; i++)
                //    {
                //        ManualSingleton<ILogger>.instance.Info($"{table}: {i}: {__result[i]}");
                //    }
                //}
                //else
                //{
                //    ManualSingleton<ILogger>.instance.Warning($"{table}: result is null");
                //}
                var stackFrames = new StackTrace().GetFrames();
                foreach (StackFrame frame in stackFrames)
                {
                    // Access information about each stack frame
                    string methodName = frame.GetMethod().Name;
                    string fileName = frame.GetFileName();
                    int lineNumber = frame.GetFileLineNumber();

                    // Do something with the stack frame information
                    ManualSingleton<ILogger>.instance.Info($"Method: {methodName}, File: {fileName}, Line: {lineNumber}");
                }
            }
        }
    }
}
