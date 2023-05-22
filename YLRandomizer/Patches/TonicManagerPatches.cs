using HarmonyLib;
using System;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(TonicManager), "UnlockTonic", new Type[] { typeof(ETonics) })]
    public class TonicManager_UnlockTonic
    {
        [HarmonyPostfix]
        public static void Postfix(ETonics tonic, TonicManager __instance)
        {
            if (tonic == ETonics.Athlete) // Unlocked after beating CapitalB
            {
                ManualSingleton<IRandomizer>.instance.SetGameCompleted();
            }
        }
    }
}
