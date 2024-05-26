using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.Types;
using YLRandomizer.Data;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;
using static PlayerMoves;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(PlayerMoves), nameof(PlayerMoves.EnableMoveInGame))]
    public class PlayerMoves_EnableMoveInGame
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(PlayerMoves.Moves moveEnum, bool boughtFromTrowzer)
        {
            var locationId = ItemAndLocationIdConverter.GetLocationIdFromMove(moveEnum);
            ManualSingleton<IRandomizer>.instance.LocationChecked(locationId);
            IntermediateActionTracker.AddLocallyCheckedLocation(locationId);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerMoves), "RegisterMove")]
    public class PlayerMoves_RegisterMove
    {
        [HarmonyPostfix]
        public static void NeverReplace(PlayerMoves.Move move, PlayerMoves.Moves moveEnum, string devName, PlayerMoves __instance)
        {
            // EatMk3 is an unknown ability, possibly one that would've been unlocked in Galleon Galaxy but was cut for whatever reason
            var moveItemId = ItemAndLocationIdConverter.GetItemIdFromMove(moveEnum == Moves.BasicAttackAir ? Moves.BasicAttack : moveEnum); // Tail Twirl item is for both ground and air
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
        [HarmonyPostfix]
        public static void NeverReplace(PlayerMoves.Moves moveEnum)
        {
            ManualSingleton<ILogger>.instance.Debug("PlayerMoves_HasLearnedMove: " + moveEnum);
            Utilities.PrintStack();
        }
    }
}
