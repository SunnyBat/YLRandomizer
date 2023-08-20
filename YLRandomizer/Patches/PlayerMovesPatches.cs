using HarmonyLib;
using YLRandomizer.Data;
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
}
