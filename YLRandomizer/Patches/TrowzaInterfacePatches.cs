using HarmonyLib;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(TrowzaInterface), "HasMoveBeenUnlocked")]
    public class TrowzaInterface_HasMoveBeenUnlocked
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(TrowzaMoveInfo moveInfo, ref bool __result)
        {
            // This is specifically for the Trowza interface to display specific sets of moves per world.
            // Thus, we're not actually looking for received abilities, but rather sent locations.
            // Additionally, we don't actually want to check for whether we've unlocked the move itself,
            // but rather the prerequisite move to get into the next world. This is because having a
            // requirement to have unlocked the move to display it on the unlock interface is... problematic.
            var requiredMove = PlayerMoveConverter.GetRequiredAbilityForWorldId(moveInfo.world);
            __result = ManualSingleton<IRandomizer>.instance.GetCheckedAbilityLocations().Contains(PlayerMoveConverter.GetLocationIdFromMove(requiredMove));
            ManualSingleton<ILogger>.instance.Debug("TrowzaInterface_HasMoveBeenUnlocked: " + PlayerMoveConverter.GetLocationIdFromMove(requiredMove) + ": " + __result);
            return false;
        }
    }
}
