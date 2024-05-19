using HarmonyLib;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(PlayerStateFallHitGround), nameof(PlayerStateFallHitGround.StateEnter))]
    public class PlayerStateFallHitGround_StateEnter
    {
        [HarmonyPostfix]
        public static void Postfix(CharState fromState, PlayerStateFallHitGround __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"PlayerStateFallHitGround_StateEnter(): {fromState.Name}");
            var mIsDeadfromFall = (bool)typeof(PlayerStateFallHitGround)
                .GetField("mIsDeadfromFall", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(__instance);
            if (mIsDeadfromFall)
            {
                ManualSingleton<IRandomizer>.instance.SendDeathLink("Hit the ground a little too fast");
            }
        }
    }
}
