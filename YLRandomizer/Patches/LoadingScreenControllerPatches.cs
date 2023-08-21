using HarmonyLib;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(LoadingScreenController), nameof(LoadingScreenController.LoadNextScene))]
    public class LoadingScreenController_LoadNextScene
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneName, LoadingScreenFade fadeMask)
        {
            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadNextScene.NeverReplace(): {sceneName}, {fadeMask}");
            GameState.SetLoadingScreenStarted();
        }
    }

    [HarmonyPatch(typeof(LoadingScreenController), nameof(LoadingScreenController.PlayOutroAnimation))]
    public class LoadingScreenController_PlayOutroAnimation
    {
        [HarmonyPrefix]
        public static void NeverReplace()
        {
            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_PlayOutroAnimation.NeverReplace()");
            GameState.SetLoadingScreenFinished();
        }
    }
}
