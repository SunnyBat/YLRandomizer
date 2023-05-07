using HarmonyLib;
using System;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.Validate))]
    public class SavegameManager_Validate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ManualSingleton<ILogger>.instance.Info($"SavegameMamager_Validate.Postfix()");
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
