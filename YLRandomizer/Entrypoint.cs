using HarmonyLib;
using System.Reflection;
using YLRandomizer.Logging;
using YLRandomizer;
using System;

namespace Doorstop
{
    public class Entrypoint
    {
        public static void Start()
        {
            doSetup();
            doPatches();
        }

        private static void doSetup()
        {
            ManualSingleton<ILogger>.instance = new FileLogger("YLRandomizer.log");
            var info = typeof(UnityEngine.Debug).GetField("s_Logger", BindingFlags.NonPublic | BindingFlags.Static);
            info.SetValue(null, new UnityEngine.Logger(new UnityILoggerImpl(ManualSingleton<ILogger>.instance)));
            try
            {
                var lmrt = typeof(UnityEngine.Application).GetField("s_LogCallbackHandlerThreaded", BindingFlags.NonPublic | BindingFlags.Static);
                lmrt.SetValue(null, Delegate.CreateDelegate(typeof(UnityEngine.Application.LogCallback), typeof(Entrypoint).GetMethod("_logCallback", BindingFlags.Static | BindingFlags.NonPublic)));
                UnityEngine.Debug.Log("TESTASDF");
            }
            catch (Exception e)
            {
                ManualSingleton<ILogger>.instance.Info(e.Message);
                ManualSingleton<ILogger>.instance.Info(e.StackTrace.ToString());
            }
        }

        private static void doPatches()
        {
            ManualSingleton<ILogger>.instance.Info("Patching game...");
            try
            {
                var gameObjectProperty = typeof(UnityEngine.Component).GetProperty("gameObject");
                ManualSingleton<ILogger>.instance.Info("" + (gameObjectProperty != null));
                var gameObjectPropetyGetFunc = gameObjectProperty.GetGetMethod();
                ManualSingleton<ILogger>.instance.Info("" + (gameObjectPropetyGetFunc != null));
                var mB = gameObjectPropetyGetFunc.GetMethodBody();
                ManualSingleton<ILogger>.instance.Info("" + (mB != null));
                var byteData = mB.GetILAsByteArray();
                ManualSingleton<ILogger>.instance.Info("" + (byteData != null));
                new Harmony("com.github.sunnybat.YLRandomizer").PatchAll(Assembly.GetExecutingAssembly());
                ReplaceMethodInstructions(typeof(UnityEngine.Component).GetProperty("gameObject").GetGetMethod(), byteData);
            }
            catch (Exception e)
            {
                ManualSingleton<ILogger>.instance.Info(e.Message);
                ManualSingleton<ILogger>.instance.Info(e.StackTrace.ToString());
            }
            ManualSingleton<ILogger>.instance.Info("Finished patching game.");
        }

        private static void _logCallback(string condition, string stackTrace, UnityEngine.LogType type)
        {
            try
            {
                if (ManualSingleton<ILogger>.instance != null)
                {
                    ManualSingleton<ILogger>.instance.Info("CALLED");
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

        public static void ReplaceMethodInstructions(MethodInfo originalMethod, byte[] newInstructions)
        {
            ManualSingleton<ILogger>.instance.Info("" + (originalMethod != null));
            // Retrieve the method body of the original method
            MethodBody originalBody = originalMethod.GetMethodBody();
            ManualSingleton<ILogger>.instance.Info("" + (originalBody != null));

            // Retrieve the byte array that represents the IL instructions of the new method
            ManualSingleton<ILogger>.instance.Info("" + (newInstructions != null));

            // Use reflection to obtain a reference to the private field that stores the IL instructions of the original method
            FieldInfo methodBodyILField = typeof(MethodBody).GetField("m_IL", BindingFlags.NonPublic | BindingFlags.Instance);
            ManualSingleton<ILogger>.instance.Info("" + (methodBodyILField != null));

            // Overwrite the IL instructions of the original method with the IL instructions of the new method
            methodBodyILField.SetValue(originalBody, newInstructions);
        }
    }
}
