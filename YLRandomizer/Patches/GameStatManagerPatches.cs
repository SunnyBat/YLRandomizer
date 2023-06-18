using HarmonyLib;
using ParadoxNotion;
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
                ManualSingleton<ILogger>.instance.Debug($"GameStatManager_HasConditionBeenMet.Postfix(): {condition.Stat}, {condition.Value}, {condition.ComparisonOperator})");
                // Capital B lift elevator to office (final boss area) check
                // We need to use pagies received instead of locations checked
                __result = OperationTools.Compare(ManualSingleton<IRandomizer>.instance.GetReceivedPagiesCount(), condition.Value, condition.ComparisonOperator);
                // TODO Do we want to do this here, or do we want to override GameStats.TryGetValue() instead? Overriding TryGetValue might have unintended consequences elsewhere
                return false;
            }
            return true;
        }
    }
}
