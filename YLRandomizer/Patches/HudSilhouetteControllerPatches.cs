using HarmonyLib;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{

    [HarmonyPatch(typeof(HudSilhouetteController), nameof(HudSilhouetteController.Animate))]
    public class HudSilhouetteController_Animate
    {
        [HarmonyPostfix]
        public static void NeverReplace(bool toBlack)
        {
            ManualSingleton<ILogger>.instance.Debug($"HudSilhouetteController_Animate.NeverReplace(): {toBlack}");
        }
    }
}
