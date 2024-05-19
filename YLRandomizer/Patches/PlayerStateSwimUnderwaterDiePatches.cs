using HarmonyLib;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(PlayerStateSwimUnderwaterDie), nameof(PlayerStateSwimUnderwaterDie.StateEnter))]
    public class PlayerStateSwimUnderwaterDie_StateEnter
    {
        [HarmonyPostfix]
        public static void Postfix(CharState fromState)
        {
            ManualSingleton<ILogger>.instance.Debug($"PlayerStateSwimUnderwaterDie_StateEnter(): {fromState.Name}");
            ManualSingleton<IRandomizer>.instance.SendDeathLink("Stayed underwater for too long");
        }
    }
}
