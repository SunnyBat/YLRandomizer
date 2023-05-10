using HarmonyLib;
using System.Reflection;
using YLRandomizer.Logging;
using YLRandomizer;
using System;
using YLRandomizer.Scripts;
using YLRandomizer.Data;

namespace Doorstop
{
    public class Entrypoint
    {
        private static bool _patched = false;

        public static void Start()
        {
            doSetup();
        }

        private static void doSetup()
        {
            ManualSingleton<IUserMessages>.instance = new UserMessages();
            ManualSingleton<ILogger>.instance = new FileLogger("YLRandomizer.err", "YLRandomizer.log");
            // Patch late so all of the Unity native methods are hooked up
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += unitySceneFirstLoad;
        }

        private static void unitySceneFirstLoad(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            doPatches();
            configureGUI();
            loadUnityExplorer();
            ArchipelagoHeartbeat.CreateNewHeartbeat();
        }

        private static void doPatches()
        {
            if (!_patched)
            {
                ManualSingleton<ILogger>.instance.Info("Patching game...");
                try
                {
                    new Harmony("com.github.sunnybat.YLRandomizer").PatchAll(Assembly.GetExecutingAssembly());
                    ManualSingleton<ILogger>.instance.Info("Finished patching game.");
                    _patched = true;
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= unitySceneFirstLoad;
                }
                catch (Exception e)
                {
                    ManualSingleton<ILogger>.instance.Error(e.Message);
                    ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    ManualSingleton<ILogger>.instance.Error("Error patching game. Future patches will be retried, but could break.");
                }
            }
        }

        private static void configureGUI()
        {
            // Create a game object that will be responsible to drawing the IMGUI in the Menu.
            var guiGameobject = new UnityEngine.GameObject();
            guiGameobject.AddComponent<ArchipelagoUI>();
            UnityEngine.GameObject.DontDestroyOnLoad(guiGameobject);
        }

        private static void loadUnityExplorer()
        {
            // Unity Explorer
            try
            {
                Assembly.LoadFrom("UniverseLib.Mono.dll");
                Assembly assembly = Assembly.LoadFrom("UnityExplorer.Mono.dll");
                Type type = assembly.GetType("UnityExplorer.ExplorerStandalone");
                type.GetMethod("CreateInstance", new Type[] { }).Invoke(null, null);
            }
            catch (Exception e)
            {
                ManualSingleton<ILogger>.instance.Error(e.Message);
                ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                ManualSingleton<ILogger>.instance.Error(e.InnerException.Message);
                ManualSingleton<ILogger>.instance.Error(e.InnerException.StackTrace);
            }
        }
    }
}
