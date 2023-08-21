using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(PlayerMoves), nameof(PlayerMoves.EnableMoveInGame))]
    public class PlayerMoves_EnableMoveInGame
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(PlayerMoves.Moves moveEnum, bool boughtFromTrowzer)
        {
            ManualSingleton<IRandomizer>.instance.LocationChecked(PlayerMoveConverter.GetLocationIdFromMove(moveEnum));
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerMoves), "RegisterMove")]
    public class PlayerMoves_RegisterMove
    {
        [HarmonyPostfix]
        public static void NeverReplace(PlayerMoves.Move move, PlayerMoves.Moves moveEnum, string devName)
        {
            var moveItemId = PlayerMoveConverter.GetItemIdFromMove(moveEnum);
            var hasReceivedMove = ManualSingleton<IRandomizer>.instance.GetReceivedAbilities().Any(itemId => itemId == moveItemId);
            SavegameManager.instance.EnableMove(moveEnum, hasReceivedMove, false);
            if (hasReceivedMove)
            {
                ManualSingleton<ILogger>.instance.Info("PMP: Enabling move: " + devName);
                move.EnableInGame();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerMoves), nameof(PlayerMoves.HasLearnedMove))]
    public class PlayerMoves_HasLearnedMove
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(PlayerMoves.Moves moveEnum, ref bool __result)
        {
            // If we're in the Trowza interface, we want to check if we've sent the location
            // Otherwise, we just let it run like normal.
            if (Utilities.StackHasMethods("ShowShopItems"))
            {
                ManualSingleton<ILogger>.instance.Debug("PlayerMoves_HasLearnedMove: Shop item found: " + moveEnum);
                __result = ManualSingleton<IRandomizer>.instance.GetCheckedAbilityLocations().Contains(PlayerMoveConverter.GetLocationIdFromMove(moveEnum));
                return false;
            }
            else
            {
                ManualSingleton<ILogger>.instance.Debug("PlayerMoves_HasLearnedMove: NOT shop item 2: " + moveEnum);
                Utilities.PrintStack();
                __result = ManualSingleton<IRandomizer>.instance.GetCheckedAbilityLocations().Contains(PlayerMoveConverter.GetLocationIdFromMove(moveEnum));
                return false;
            }
        }
    }
}
