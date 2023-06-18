using HarmonyLib;
using System.Linq;
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
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetPagieLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex, index));
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectArcadeToken))]
    public class SavegameManager_CollectArcadeToken
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status, SavegameManager __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_CollectArcadeToken.Postfix()");

            // TODO Do we even care about this? Doesn't look like it's actually used anywhere
            __instance.savegame.player.arcadeTokenCount = ManualSingleton<IRandomizer>.instance.GetReceivedPlayCoins().Length;
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetPlaycoinLocationId(index));
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.GetArcadeTokenCount))]
    public class SavegameManager_GetArcadeTokenCount
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(ref int __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_GetArcadeTokenCount.AlwaysReplace()");
            __result = ManualSingleton<IRandomizer>.instance.GetCheckedPlaycoinLocations()
                .Contains(ArchipelagoLocationConverter.GetPlaycoinLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex))
                ? 1
                : 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectTransformToken))]
    public class SavegameManager_CollectTransformToken
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status, SavegameManager __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_CollectTransformToken.Postfix()");

            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetMollycoolLocationId(index));
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.GetTransformationTokenCount))]
    public class SavegameManager_GetTransformationTokenCount
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(ref int __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_GetTransformationTokenCount.AlwaysReplace()");
            __result = ManualSingleton<IRandomizer>.instance.GetCheckedMollycoolLocations()
                .Contains(ArchipelagoLocationConverter.GetMollycoolLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex))
                ? 1
                : 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectHealthExtenderToken))]
    public class SavegameManager_CollectHealthExtenderToken
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status, SavegameManager __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_CollectHealthExtenderToken.Postfix()");

            __instance.savegame.player.healthExtenderTokenCount = ManualSingleton<IRandomizer>.instance.GetReceivedHealthExtenderCount();
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetHealthExtenderLocationId(index));
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectSpecialExtenderToken))]
    public class SavegameManager_CollectSpecialExtenderToken
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status, SavegameManager __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_CollectSpecialExtenderToken.Postfix()");

            // TODO Do we even care about this? Doesn't look like it's actually used anywhere
            __instance.savegame.player.specialExtenderTokenCount = ManualSingleton<IRandomizer>.instance.GetReceivedEnergyExtenderCount();
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetEnergyExtenderLocationId(index));
        }
    }
}
