using HarmonyLib;
using System;
using UnityEngine.SceneManagement;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadScene), new Type[] { typeof(string) })]
    public class SceneManager_LoadScene_1
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneName)
        {
            ManualSingleton<ILogger>.instance.Debug($"SceneManager_LoadScene.NeverReplace(): {sceneName}");
        }
    }

    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadScene), new Type[] { typeof(string), typeof(LoadSceneMode) })]
    public class SceneManager_LoadScene_2
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneName, LoadSceneMode mode)
        {
            ManualSingleton<ILogger>.instance.Debug($"SceneManager_LoadScene.NeverReplace(): {sceneName}, {mode}");
        }
    }

    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadScene), new Type[] { typeof(int), typeof(LoadSceneMode) })]
    public class SceneManager_LoadScene_3
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneId, LoadSceneMode mode)
        {
            ManualSingleton<ILogger>.instance.Debug($"SceneManager_LoadScene.NeverReplace(): {sceneId}, {mode}");
        }
    }

    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadSceneAsync), new Type[] { typeof(string) })]
    public class SceneManager_LoadSceneAsync_1
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneName)
        {
            ManualSingleton<ILogger>.instance.Debug($"SceneManager_LoadSceneAsync.NeverReplace(): {sceneName}");
        }
    }

    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadSceneAsync), new Type[] { typeof(string), typeof(LoadSceneMode) })]
    public class SceneManager_LoadSceneAsync_2
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneName, LoadSceneMode mode)
        {
            ManualSingleton<ILogger>.instance.Debug($"SceneManager_LoadSceneAsync.NeverReplace(): {sceneName}, {mode}");
        }
    }

    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadSceneAsync), new Type[] { typeof(int), typeof(LoadSceneMode) })]
    public class SceneManager_LoadSceneAsync_3
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneId, LoadSceneMode mode)
        {
            ManualSingleton<ILogger>.instance.Debug($"SceneManager_LoadSceneAsync.NeverReplace(): {sceneId}, {mode}");
        }
    }
}
