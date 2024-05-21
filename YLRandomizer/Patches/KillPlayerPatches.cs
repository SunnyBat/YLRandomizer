using HarmonyLib;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(KillPlayer), "OnExecute")]
    public class KillPlayer_OnExecute
    {
        [HarmonyPostfix]
        public static void NeverReplace()
        {
            ManualSingleton<ILogger>.instance.Debug($"KillPlayer_OnExecute.NeverReplace()");
            Utilities.PrintStack();
        }
    }
}
