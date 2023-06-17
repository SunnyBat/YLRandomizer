using HarmonyLib;
using YLRandomizer.Data;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.Validate))]
    public class SavegameManager_Validate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameMamager_Validate.Postfix()");
            // This will be called after a world is loaded.

            ArchipelagoDataHandler.UpdateCurrentGameStateToAP();
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectPagie))]
    public class SavegameManager_CollectPagie
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_CollectPagie.Postfix(): {index}, {status}");
            SavegameManager.instance.savegame.player.unspentPagies--;
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex, index));
        }
    }
}
