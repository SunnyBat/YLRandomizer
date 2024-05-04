using HarmonyLib;
using System;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(PlayerDeathManager), nameof(PlayerDeathManager.StartPostDeathSequence))]
    public class PlayerDeathManager_StartPostDeathSequence
    {
        private static readonly TimeSpan MinTimeBetweenDeathLinks = TimeSpan.FromSeconds(5);
        private static DateTime _lastTimeSent = DateTime.MinValue;
        [HarmonyPrefix]
        public static void Prefix()
        {
            ManualSingleton<ILogger>.instance.Debug("PlayerDeathManager_StartPostDeathSequence()");
            Utilities.PrintStack();
            if (DateTime.Now - _lastTimeSent > MinTimeBetweenDeathLinks)
            {
                ManualSingleton<IRandomizer>.instance.SendDeathLink("Dropped to zero health");
            }
            else
            {
                ManualSingleton<ILogger>.instance.Debug("PlayerDeathManager_StartPostDeathSequence: Too short time between sends");
            }
        }
    }
}
