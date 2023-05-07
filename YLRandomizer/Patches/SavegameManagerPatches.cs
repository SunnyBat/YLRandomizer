using HarmonyLib;
using System;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    // Breaks the entire game if used... Cause is TBD
    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.Validate))]
    public class SavegameManager_Validate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            //ManualSingleton<ILogger>.instance.Info($"SavegameMamager_Validate.Postfix()");
            // TODO: Set unusedPagies to count from Archipelago
        }
    }

    // But this doesn't, yet it has the same signature sans the name... WTF?
    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.ResetGenericTokenCount))]
    public class SavegameManager_ResetGenericTokenCount
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ManualSingleton<ILogger>.instance.Info($"SavegameManager_ResetGenericTokenCount.Postfix()");
            // TODO: Set unusedPagies to count from Archipelago
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectPagie))]
    public class SavegameManager_CollectPagie
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status)
        {
            try
            {
                UnityEngine.Debug.Log("TEST");
            }
            catch (Exception e)
            {
                ManualSingleton<ILogger>.instance.Info(e.Message);
                ManualSingleton<ILogger>.instance.Info(e.StackTrace.ToString());
            }
            ManualSingleton<ILogger>.instance.Info($"SavegameManager_CollectPagie.Postfix(): Index={index} | Status={status}");
            //___player.unspentPagies--; // Enable when ready (aka Archipelago integrated)
            // TODO Send item to Archipelago
        }
    }
}
