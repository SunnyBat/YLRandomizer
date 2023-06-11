using HarmonyLib;
using System.Collections.Generic;
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

            // === HANDLE EXISTING LOCATIONS ===
            // - Convert save to location list
            var locationIds = new List<long>();
            for (int i = 1; i < 7; i++) // Length 8; 0 and 7 are unused
            {
                var currentWorld = SavegameManager.instance.savegame.worlds[i];
                for (int j = 0; j < currentWorld.pagies.Length; j++)
                {
                    if (currentWorld.pagies[j] == Savegame.CollectionStatus.Collected)
                    {
                        locationIds.Add(ArchipelagoLocationConverter.GetLocationId(i, j));
                    }
                }
            }
            // - Send location list to Archipelago
            ManualSingleton<IRandomizer>.instance.LocationChecked(locationIds.ToArray());

            // === HANDLE NEW LOCATIONS ===
            // - Read all locations from Archipelago
            var foundLocations = ManualSingleton<IRandomizer>.instance.GetAllCheckedLocations();
            // - Set relevant pagies from Archipelago as found
            for (int i = 0; i < foundLocations.Length; i++)
            {
                var pagieData = ArchipelagoLocationConverter.GetPagieInfo(foundLocations[i]);
                SavegameManager.instance.savegame.worlds[pagieData.Item1].pagies[pagieData.Item2] = Savegame.CollectionStatus.Collected;
            }

            // === HANDLE RECEIVED ITEMS ===
            // - Read all items from Archipelago
            var receivedItems = ManualSingleton<IRandomizer>.instance.GetAllItems();
            // - Calculate pagies spent on worlds
            var spentPagies = 0;
            for (int i = 1; i < 7; i++)
            {
                if (SavegameManager.instance.savegame.worlds[i].worldUnlocked)
                {
                    switch (i)
                    {
                        case 2:
                            spentPagies++;
                            break;
                        case 3:
                            spentPagies += 12;
                            break;
                        case 4:
                            spentPagies += 10;
                            break;
                        case 5:
                            spentPagies += 3;
                            break;
                        case 6:
                            spentPagies += 7;
                            break;
                    }
                }
                if (SavegameManager.instance.savegame.worlds[i].worldExpanded)
                {
                    switch (i)
                    {
                        case 2:
                            spentPagies += 3;
                            break;
                        case 3:
                            spentPagies += 15;
                            break;
                        case 4:
                            spentPagies += 11;
                            break;
                        case 5:
                            spentPagies += 5;
                            break;
                        case 6:
                            spentPagies += 8;
                            break;
                    }
                }
            }
            // - Set unspent pagies based off of items from Archipelago and worlds unlocked
            var totalPagies = receivedItems.Length;
            SavegameManager.instance.savegame.player.unspentPagies = totalPagies - spentPagies;

            // == HANDLE GAME STATE ==
            var endGameTonicInfo = TonicManager.instance.UnlockRequirements.FindAll(req => req.Tonic == ETonics.Athlete);
            if (endGameTonicInfo.Count > 0 && endGameTonicInfo.All(req => GameStatManager.instance.HasConditionBeenMet(req.Condition)))
            {
                ManualSingleton<IRandomizer>.instance.SetGameCompleted();
            }
            else
            {
                ManualSingleton<IRandomizer>.instance.SetInGame();
            }
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

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.PerformUnlock))]
    public class SavegameManager_PerformUnlock
    {
        private static readonly int[] _worldIndexOrder = new int[] { 2, 5, 6, 4, 3 };

        [HarmonyPrefix]
        public static bool SometimesReplace(string unlockVarName, int worldIndex, int cost, WorldUnlockState.WorldUnlockType unlockType, ref bool __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_PerformUnlock.SometimesReplace(): {unlockVarName}, {worldIndex}, {cost}, {unlockType}");
            for (int i = 0; i < _worldIndexOrder.Length; i++)
            {
                var currentWorldIndex = _worldIndexOrder[i];
                if (currentWorldIndex == worldIndex)
                {
                    return true; // Assume we're good to continue, all worlds before this have been unlocked+expanded
                }
                else // Still stepping through worlds, need to check unlock+expansion of currentWorldIndex
                {
                    var currentWorldInfo = SavegameManager.instance.savegame.worlds[currentWorldIndex];
                    if (!currentWorldInfo.worldUnlocked || !currentWorldInfo.worldExpanded)
                    {
                        ManualSingleton<IUserMessages>.instance.AddMessage("WARN: Cannot unlock world right now.");
                        ManualSingleton<IUserMessages>.instance.AddMessage("Worlds must be unlocked and expanded in world order.");
                        __result = false;
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.HasUnspentPagies))]
    public class SavegameManager_HasUnspentPagies
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(int quantity, ref bool __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_HasUnspentPagies.SometimesReplace(): {quantity}");
            return true;
        }
    }

    [HarmonyPatch(typeof(SavegameManager), nameof(SavegameManager.GetUnspentPagies))]
    public class SavegameManager_GetUnspentPagies
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(ref int __result)
        {
            ManualSingleton<ILogger>.instance.Debug($"SavegameManager_GetUnspentPagies.SometimesReplace()");
            return true;
        }
    }
}
