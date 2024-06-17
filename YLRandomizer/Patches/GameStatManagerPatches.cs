using HarmonyLib;
using ParadoxNotion;
using System;
using System.Collections.Generic;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(GameStatManager), nameof(GameStatManager.HasConditionBeenMet))]
    public class GameStatManager_HasConditionBeenMet
    {
        private const int MAX_DYNAMIC_CALL_COUNT = 25;

        private static readonly Dictionary<EGameStats, int> _statCallCounts = new Dictionary<EGameStats, int>();
        private static readonly EGameStats[] _statsToNeverPrint = new EGameStats[]
        {
            EGameStats.PagiesCollected, // Looks like this is used for a bunch of tome entrances, perhaps text triggers, final boss check, etc -- not printed too much, but unnecessary a lot of the time
            EGameStats.PagiesCollectedGlacier,
            EGameStats.HubBIceSwitchDone,
            EGameStats.ShownTutFirstButterflyEaten,
            EGameStats.ButterfliesEaten,
            EGameStats.ButterfliesWalkedInto,
            EGameStats.ShownTutAllPagiesInAWorld,
            EGameStats.CollectedAllPagiesInAWorld,
            EGameStats.ShownTutEnoughQuillies,
            EGameStats.ShownTutFirstHealthExtender,
            EGameStats.ShownTutFirstArcadeToken,
            EGameStats.ShownTutFirstHealthExtender,
            EGameStats.ShownTutFirstEnergyExtender,
            EGameStats.ShownTutFirstCasinoChip,
            EGameStats.HealthExtendersCollected,
            EGameStats.EnergyExtendersCollected,
            EGameStats.MainSwitchToHubBPressed,
            EGameStats.HubLairTrowzerIntroDone,
            EGameStats.PagiesCollectedJungle // TODO Does this do anything? For example, would it perevent Trowza from unlocking the Reptile Roll location check?
        };
        private static readonly EGameStats[] _statsToAlwaysPrint = new EGameStats[]
        {
        };
        private static readonly EGameStats[] _trowzaFreeAbilityStatChecks = new EGameStats[]
        {
            EGameStats.PagiesCollectedGlacier,
            EGameStats.PagiesCollectedSwamp,
            EGameStats.PagiesCollectedCasino,
            EGameStats.PagiesCollectedSpace
        };
        private static readonly EGameStats[] _quizStats = new EGameStats[]
        {
            EGameStats.PreQuizCutsceneShown,
            EGameStats.Quiz01Completed,
            EGameStats.Quiz02Completed,
            EGameStats.Quiz03Completed
        };

        // TODO Unacache this if we disconnect
        private static int? _cachedRequiredPagieCount = null;
        private static bool? _cachedQuizConfiguration = null;

        [HarmonyPrefix]
        public static bool SometimesReplace(GameStatCondition condition, GameStatManager __instance, ref bool __result)
        {
            var callCount = _statCallCounts.GetValueSafe(condition.Stat);
            if (condition.Stat == EGameStats.JungleVisited && condition.Value == 1 && condition.ComparisonOperator == CompareMethod.GreaterOrEqualTo) // To activate plinth for Tribalstack expansion ASAP
            {
                __result = true;
                return false;
            }
            else if (condition.Stat == EGameStats.PagiesCollected && condition.Value == Constants.DEFAULT_REQUIRED_PAGIES_FOR_CAPITAL_B)
            {
                // Capital B lift elevator to office (final boss area) check
                ManualSingleton<ILogger>.instance.Debug($"GameStatManager_HasConditionBeenMet.SometimesReplace(): {condition.Stat}, {condition.Value}, {condition.ComparisonOperator})");
                int requiredPagieCount = Constants.DEFAULT_REQUIRED_PAGIES_FOR_CAPITAL_B;
                if (_cachedRequiredPagieCount != null)
                {
                    requiredPagieCount = (int) _cachedRequiredPagieCount;
                }
                else if (ArchipelagoDataHandler.TryGetSlotData(Constants.CONFIGURATION_NAME_CAPITAL_B_PAGIE_COUNT, out long outPagies))
                {
                    requiredPagieCount = (int) outPagies;
                    _cachedRequiredPagieCount = requiredPagieCount;
                }
                __result = OperationTools.Compare(ManualSingleton<IRandomizer>.instance.GetReceivedPagiesCount(), requiredPagieCount, condition.ComparisonOperator);
                return false;
            }
            else if (_trowzaFreeAbilityStatChecks.Contains(condition.Stat))
            {
                // This will check two things:
                // 1. The vanilla condition, which is "Did you collect a Pagie from the world that this
                //    Trowzer ability unlocks?"
                // 2. Did you actually talk to Trowzer?
                // If both of these are true, Trowzer will move to the next location. Otherwise, Trowzer
                // will stay at this current location. This forces the player to both talk to Trowzer
                // (which will open the world) and collect a pagie in the world (which will move Trowzer).
                // This is really only required for specific edge cases around things like "The player
                // received all their abilities and immediately booked it to Galleon Galaxy, which will
                // prevent the player from ever checking the Flappy Flight Trowzer location". This system
                // will account for this and force Trowzer to be checked in order.
                //
                // Trowzer will always appear in the free ability order, starting with Glide. Until you
                // talk to him and get the previous Trowzer ability, he will not show up at the next
                // location. More specifically, if you want Trowzer to show up at the Flappy Flight
                // location, the game checks whether he should show up at Glide, then Buddy Bubble, then
                // Camo Cloak, then finally Flappy Flight. If any of the locations before Flappy Flight
                // are showing Trowzer, then he will not show up at Flappy Flight.
                // 
                // We could make Trowzer show up in multiple places by tracking which room the player is
                // currently in, however this would be very difficult for the Flappy Flight location since
                // we'd have to somehow track whether we're checking the Camo Cloak Trowzer or the Flappy
                // Flight trowzer, since they're both in the same room. Thus, this seems like something
                // that we should not do, since it adds a lot of complexity for an edge case that is
                // unlikely to ever matter.
                // To add to the lower likelihood of this ever happening, the world that a particular
                // vanilla Trowzer ability grant access to is blocked by invisible walls until you talk to
                // Trowzer. This means players would have to intentionally skip both Trowzer and the world
                // to hit these scenarios.
                //
                // Theoretically Trowzer can immediately teleport if you get a pagie in the relevant world
                // and then talk to Trowzer, but even if that happens it's going to be more of a quirk of
                // the randomizer than a real issue, so not addressing it is fine.
                var moveId = ItemAndLocationIdConverter.GetLocationIdFromEGameStat(condition.Stat);
                var hasBeenChecked = ManualSingleton<IRandomizer>.instance.GetAllCheckedLocations().Contains(moveId);
                if (__result)
                {
                    IntermediateActionTracker.RemoveLocallyCheckedLocation(moveId);
                }
                else if (IntermediateActionTracker.HasLocationBeenCheckedLocally(moveId))
                {
                    hasBeenChecked = true;
                }
                var hasCollectedPagie = true;
                if (ArchipelagoDataHandler.TryGetSlotData(Constants.CONFIGURATION_NAME_WORLDORDER, out Newtonsoft.Json.Linq.JArray worldOrderFromServer) && worldOrderFromServer.Count == 5)
                {
                    var randomizedWorldOrder = worldOrderFromServer.ToArray();
                    var newWorldOptionsId = randomizedWorldOrder[(int) condition.Stat - 18].ToString();
                    var newLogicalIndexFromRandomizedWorldOrder = Array.IndexOf(Constants.VanillaConfigurationOptionsWorldOrder, newWorldOptionsId);
                    if (newLogicalIndexFromRandomizedWorldOrder != -1)
                    {
                        var savegameManagerWorldIndex = Constants.WorldIndexOrder[newLogicalIndexFromRandomizedWorldOrder];
                        var worldData = SavegameManager.instance.savegame.worlds[savegameManagerWorldIndex];
                        hasCollectedPagie = OperationTools.Compare(worldData.pagieCount, condition.Value, condition.ComparisonOperator);
                    }
                    else
                    {
                        hasCollectedPagie = false;
                    }
                }
                else if (__instance.GameStats.TryGetValue(condition.Stat, out int originalValue))
                {
                    hasCollectedPagie = OperationTools.Compare(originalValue, condition.Value, condition.ComparisonOperator);
                }
                __result = _getResultForCondition(condition, hasBeenChecked && hasCollectedPagie);
                return false;
            }
            else if (_quizStats.Contains(condition.Stat))
            {
                bool shouldDisableQuizzes = false;
                if (_cachedQuizConfiguration != null)
                {
                    shouldDisableQuizzes = (bool) _cachedQuizConfiguration;
                }
                else if (ArchipelagoDataHandler.TryGetSlotData(Constants.CONFIGURATION_NAME_DISABLE_QUIZZES, out bool configResult))
                {
                    shouldDisableQuizzes = configResult;
                    _cachedQuizConfiguration = configResult;
                }

                if (shouldDisableQuizzes)
                {
                    // Will spam if quiz completed, don't print
                    __result = _getResultForCondition(condition, true);
                    return false;
                }
            }
            else if (!_statsToNeverPrint.Contains(condition.Stat) && (_statsToAlwaysPrint.Contains(condition.Stat) || callCount < MAX_DYNAMIC_CALL_COUNT))
            {
                ManualSingleton<ILogger>.instance.Debug($"GameStatManager_HasConditionBeenMet.SometimesReplace(): {condition.Stat}, {condition.Value}, {condition.ComparisonOperator})");
                _statCallCounts[condition.Stat] = callCount + 1;
                if (callCount + 1 >= MAX_DYNAMIC_CALL_COUNT)
                {
                    ManualSingleton<ILogger>.instance.Debug($"GameStatManager_HasConditionBeenMet.SometimesReplace(): STOPPING PRINTS: {condition.Stat})");
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the result value for the given condition that lines up with whether the value should be 0 or
        /// greater than 0. This will return true or false depending on the given condition's ComparisonOperator,
        /// value, and whether wantToBeGreaterThanZero is true or false.
        /// For example, if we want the result to be "greather than 0" and the condition is "EqualTo 0", this will
        /// return false -- since the condition is checking if the value == 0 and we want to say it's greater than
        /// 0, this returns false to indicate the value is not equal to 0.
        /// </summary>
        /// <param name="condition">The condition to parse and return a value based off of</param>
        /// <param name="wantToBeGreaterThanZero">True if we want the result to indicate that our condition's value
        ///     is greater than 0, or false to indicate it's less than or equal to 0.</param>
        /// <returns>True or false depending on the context</returns>
        private static bool _getResultForCondition(GameStatCondition condition, bool wantToBeGreaterThanZero)
        {
            var isCheckingForZero = (condition.ComparisonOperator == CompareMethod.EqualTo && condition.Value == 0)
                || (condition.ComparisonOperator == CompareMethod.LessOrEqualTo && condition.Value == 0)
                || (condition.ComparisonOperator == CompareMethod.LessThan && condition.Value == 1);
            return isCheckingForZero != wantToBeGreaterThanZero;
        }
    }

    [HarmonyPatch(typeof(GameStatManager), nameof(GameStatManager.GetCurrentValue), new Type[] { typeof(EGameStats) })]
    public class GameStatManager_GetCurrentValue
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(EGameStats statName)
        {
            ManualSingleton<ILogger>.instance.Debug($"GameStatManager_GetCurrentValue.SometimesReplace(): {statName}");
            return true;
        }
    }

    [HarmonyPatch(typeof(GameStatManager), nameof(GameStatManager.IsStatGreaterThanZero), new Type[] { typeof(EGameStats) })]
    public class GameStatManager_IsStatGreaterThanZero
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(EGameStats stat)
        {
            ManualSingleton<ILogger>.instance.Debug($"GameStatManager_IsStatGreaterThanZero.SometimesReplace(): {stat}");
            return true;
        }
    }//SetGameStat(EGameStats stat, int newValue)

    //[HarmonyPatch(typeof(GameStatManager), nameof(GameStatManager.SetGameStat), new Type[] { typeof(EGameStats), typeof(int) })]
    //public class GameStatManager_SetGameStat
    //{
    //    [HarmonyPrefix]
    //    public static void ModifyParameters(EGameStats stat, ref int newValue)
    //    {
    //        ManualSingleton<ILogger>.instance.Debug($"GameStatManager_SetGameStat.ModifyParameters(): {stat}, {newValue}");
    //        if (stat == EGameStats.CurrentLoadStartPoint && ArchipelagoDataHandler.TryGetSlotData(Constants.CONFIGURATION_NAME_WORLDORDER, out Newtonsoft.Json.Linq.JArray worldOrder))
    //        {
    //            // 9-13 = TT-GY, 14-18 = Expanded, vanilla world order
    //            if (newValue >= 9 && newValue <= 18)
    //            {
    //                var vanillaWorldIndex = (newValue - 9) % 5;
    //                var worldOptionsId = worldOrder[vanillaWorldIndex];
    //                var newerValue = Array.IndexOf(Constants.VanillaConfigurationOptionsWorldOrder, worldOptionsId) + 9;
    //                if (newValue >= 14) // Expanded world
    //                {
    //                    newerValue += 5;
    //                }
    //                newValue = newerValue;
    //                //switch (newValue)
    //                //{
    //                //    case 9: // Tribalstack Tropics
    //                //    case 14: // Tribalstack Tropics Expanded
    //                //    case 10: // Glitterglaze Glacier
    //                //    case 15: // Glitterglaze Glacier Expanded
    //                //    case 11: // Moodymaze Marsh
    //                //    case 16: // Moodymaze Marsh Expanded
    //                //    case 12: // Capital Cashino
    //                //    case 17: // Capital Cashino Expanded
    //                //    case 13: // Galleon Galaxy
    //                //    case 18: // Galleon Galaxy Expanded
    //                //}
    //            }
    //            else if (newValue >= 4 && newValue <= 8)
    //            {

    //            }
    //        }
    //    }
    //}
}
