﻿using HarmonyLib;
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
            var info = typeof(UnityEngine.Debug).GetField("s_Logger", BindingFlags.NonPublic | BindingFlags.Static);
            info.SetValue(null, new UnityEngine.Logger(new UnityILoggerImpl(ManualSingleton<ILogger>.instance)));
            try
            {
                // No idea if we need to hack this now or if we can put it in doPatchesLate but leaving it because it works 👉👉
                var logCallbackField = typeof(UnityEngine.Application).GetField("s_LogCallbackHandlerThreaded", BindingFlags.NonPublic | BindingFlags.Static);
                logCallbackField.SetValue(null, Delegate.CreateDelegate(typeof(UnityEngine.Application.LogCallback), typeof(Entrypoint).GetMethod("_logCallback", BindingFlags.Static | BindingFlags.NonPublic)));
            }
            catch (Exception e)
            {
                ManualSingleton<ILogger>.instance.Error(e.Message);
                ManualSingleton<ILogger>.instance.Error(e.StackTrace);
            }
            // Patch late so all of the Unity native methods are hooked up
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += unitySceneFirstLoad;
        }

        private static void unitySceneFirstLoad(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            doPatches();
            configureGUI();
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

        private static void _logCallback(string condition, string stackTrace, UnityEngine.LogType type)
        {
            try
            {
                if (ManualSingleton<ILogger>.instance != null)
                {
                    if (!string.IsNullOrEmpty(condition))
                    {
                        ManualSingleton<ILogger>.instance.Info(condition);
                    }
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        ManualSingleton<ILogger>.instance.Info(stackTrace);
                    }
                    ManualSingleton<ILogger>.instance.Flush();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
