using HarmonyLib;
using ParadoxNotion;
using System;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(GameStatManager), nameof(GameStatManager.HasConditionBeenMet))]
    public class GameStatManager_HasConditionBeenMet
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(GameStatCondition condition, ref bool __result)
        {
            if (condition.Stat == EGameStats.PagiesCollected && condition.Value == 100)
            {
                ManualSingleton<ILogger>.instance.Debug($"GameStatManager_HasConditionBeenMet.SometimesReplace(): {condition.Stat}, {condition.Value}, {condition.ComparisonOperator})");
                // Capital B lift elevator to office (final boss area) check
                // We need to use pagies received instead of locations checked
                __result = OperationTools.Compare(ManualSingleton<IRandomizer>.instance.GetReceivedPagiesCount(), condition.Value, condition.ComparisonOperator);
                // TODO Do we want to do this here, or do we want to override GameStats.TryGetValue() instead? Overriding TryGetValue might have unintended consequences elsewhere
                return false;
            }
            else if (condition.Stat == EGameStats.Quiz01Completed || condition.Stat == EGameStats.Quiz02Completed || condition.Stat == EGameStats.Quiz03Completed)
            {
                // Will spam if quiz completed, don't print
                // TODO Check AP configuration before disabling quizzes (needs to be implemented)
                __result = condition.Value != 0; // Works for "== 0" (returns false) and ">= 1" (returns true)
                return false;
            }
            return true;
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
    }
}
