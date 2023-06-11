using HarmonyLib;
using UnityEngine.EventSystems;
using YLRandomizer.Data;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(FrontendSavegameScreenController), "OnSubmit")]
    public class FrontendSavegameScreenController_OnSubmit
    {
        [HarmonyPrefix]
        public static bool SometimesReplace(BaseEventData eventData)
        {
            ManualSingleton<ILogger>.instance.Debug($"FrontendSavegameScreenController_OnSubmit.SometimesReplace()");
            if (ManualSingleton<IRandomizer>.instance == null)
            {
                ManualSingleton<IUserMessages>.instance.AddMessage("Not connected to Archipelago, cannot load save!");
                ManualSingleton<IUserMessages>.instance.AddMessage("Connect before loading the save.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
