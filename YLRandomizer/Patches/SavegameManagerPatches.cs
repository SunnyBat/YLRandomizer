using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using YLRandomizer.Data;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
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
            // index is NOT the worldIndex. Sigh.
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetPlaycoinLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex));
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.GetArcadeTokenCount))]
    public class SavegameManager_GetArcadeTokenCount
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(ref int __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_GetArcadeTokenCount.SometimesReplace()");
            if (GameState.IsWithinLoadingScreenWindow()) // If we're loading the world, use vanilla world logic for spawning
            {
                ManualSingleton<ILogger>.instance.Debug($"Not replacing ({GameState.IsWithinLoadingScreenWindow()}, {Utilities.StackHasMethods("Start")})");
                return true;
            }

            var playCoinIndex = Constants.WorldIndexToLogicalIndexTranslations[DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex] - 1;
            var receivedPlayCoins = ManualSingleton<IRandomizer>.instance.GetReceivedPlayCoins();
            if (playCoinIndex >= 0 && playCoinIndex < receivedPlayCoins.Length)
            {
                __result = receivedPlayCoins[playCoinIndex]
                    ? 1
                    : 0;
                return false;
            }
            else
            {
                __result = 0;
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.CollectTransformToken))]
    public class SavegameManager_CollectTransformToken
    {
        [HarmonyPostfix]
        public static void Postfix(int index, Savegame.CollectionStatus status, SavegameManager __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_CollectTransformToken.Postfix()");

            // index is NOT the worldIndex. Sigh.
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetMollycoolLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex));
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.GetTransformationTokenCount))]
    public class SavegameManager_GetTransformationTokenCount
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(ref int __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_GetTransformationTokenCount.SometimesReplace()");
            Utilities.PrintStack();
            if (GameState.IsWithinLoadingScreenWindow()) // If we're loading the world, use vanilla world logic for spawning
            {
                ManualSingleton<ILogger>.instance.Debug($"Not replacing ({GameState.IsWithinLoadingScreenWindow()}, {Utilities.StackHasMethods("Start")})");
                return true;
            }

            var mollycoolIndex = Constants.WorldIndexToLogicalIndexTranslations[DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex] - 1;
            var receivedMollycools = ManualSingleton<IRandomizer>.instance.GetReceivedMollycools();
            if (mollycoolIndex >= 0 && mollycoolIndex < receivedMollycools.Length)
            {
                __result = receivedMollycools[mollycoolIndex]
                    ? 1
                    : 0;
                return false;
            }
            else
            {
                __result = 0;
                return false;
            }
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
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetHealthExtenderLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex));
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
            ManualSingleton<IRandomizer>.instance?.LocationChecked(ArchipelagoLocationConverter.GetEnergyExtenderLocationId(DestroyableMonoBehaviourSingleton<WorldInfo>.instance.worldIndex));
        }
    }
}
