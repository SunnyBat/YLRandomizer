using HarmonyLib;
using System.Reflection;
using YLRandomizer.Logging;
using YLRandomizer;
using System;

namespace Doorstop
{
    public class Entrypoint
    {
        public static void Start()
        {
            doSetup();
            doPatches();
        }

        private static void doSetup()
        {
            ManualSingleton<ILogger>.instance = new FileLogger("YLRandomizer.log");
        }

        private static void doPatches()
        {
            ManualSingleton<ILogger>.instance.Info("Patching game...");
            try
            {
                new Harmony("com.github.sunnybat.YLRandomizer").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                ManualSingleton<ILogger>.instance.Info(e.Message);
                ManualSingleton<ILogger>.instance.Info(e.StackTrace.ToString());
            }
            ManualSingleton<ILogger>.instance.Info("Finished patching game.");
        }
    }
}
