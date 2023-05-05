using HarmonyLib;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    // Breaks the entire game if used... Cause is TBD
    //[HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.Validate))]
    //public class SavegameManager_Validate
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix()
    //    {
    //        ManualSingleton<ILogger>.instance.Info($"SavegameMamager_Validate.Postfix()");
    //        // TODO: Set unusedPagies to count from Archipelago
    //    }
    //}

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectPagie))]
    public class SavegameManager_CollectPagie
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status)
        {
            ManualSingleton<ILogger>.instance.Info($"SavegameManager_CollectPagie.Postfix(): Index={index} | Status={status}");
            //___player.unspentPagies--; // Enable when ready (aka Archipelago integrated)
            // TODO Send item to Archipelago
        }
    }
}
