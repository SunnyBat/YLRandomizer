using HarmonyLib;
using System;
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
            // If we error out here this will be retried, and damage will be reapplied. This continues
            // even after the player's health reaches 0. Thus, if we repeatedly error out here, the
            // player is softlocked and must exit the world to fix it. Try/catch for safety.
            try
            {
                ManualSingleton<ILogger>.instance.Debug($"PlayerStateFallHitGround_StateEnter(): {fromState?.Name}");
                var mIsDeadFromFall = (bool)typeof(PlayerStateFallHitGround)
                    .GetField("mIsDeadFromFall", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(__instance);
                if (mIsDeadFromFall)
                {
                    ManualSingleton<ILogger>.instance.Debug("Triggering fall DeathLink (if applicable)");
                    ManualSingleton<IRandomizer>.instance.SendDeathLink("Hit the ground a little too fast");
                }
            }
            catch (Exception e)
            {
                Utilities.PrintFullErrorDetails(e);
            }
        }
    }
}
