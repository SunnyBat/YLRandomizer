using HarmonyLib;
using NodeCanvas.Tasks.Actions;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    // NOTE: Patches ALL OnEvent calls
    [HarmonyPatch(typeof(WaitForEventDispatcherEvent<FinalBossPhaseCompleteEvent>), "OnEvent", typeof(FinalBossPhaseCompleteEvent))]
    public class WaitForEventDispatcherEvent_FinalBossPhaseCompleteEvent_OnEvent
    {
        [HarmonyPostfix]
        public static void Postfix(ref WaitForEventDispatcherEvent<FinalBossPhaseCompleteEvent> __instance, FinalBossPhaseCompleteEvent ev)
        {
            ManualSingleton<ILogger>.instance.Warning($"WaitForEventDispatcherEvent_FinalBossPhaseCompleteEvent_OnEvent.Postfix()");
            ManualSingleton<ILogger>.instance.Warning(ev?.ToString());
            ManualSingleton<ILogger>.instance.Warning(__instance.ToString());
            ManualSingleton<ILogger>.instance.Warning(__instance.description);
            ManualSingleton<ILogger>.instance.Warning(__instance.agentInfo);
            ManualSingleton<ILogger>.instance.Warning(__instance.agentType?.Name);
            ManualSingleton<ILogger>.instance.Warning(__instance.overrideAgentParameterName);
            ManualSingleton<ILogger>.instance.Warning("");

            // === SAMPLES === 
            /*
             *
             * 6 total ArenaTransitionCompleteEvent fires. Last one is just before the final boss phase.
                [Warning:YLRandomizer] WaitForEventDispatcherEvent_FinalBossPhaseCompleteEvent_OnEvent.Postfix()
                [Warning:YLRandomizer] ArenaTransitionCompleteEvent
                [Warning:YLRandomizer] <b>owner</b> Wait For Arena Transition Complete
                [Warning:YLRandomizer]
                [Warning:YLRandomizer] <b>owner</b>
                [Warning:YLRandomizer]
                [Warning:YLRandomizer]
                [Warning:YLRandomizer]

                [Warning:YLRandomizer] WaitForEventDispatcherEvent_FinalBossPhaseCompleteEvent_OnEvent.Postfix()
                [Warning:YLRandomizer] BeeSwarmSurroundPlayerEvent
                [Warning:YLRandomizer] <b>owner</b> Wait For Bee Swarm Surround Player
                [Warning:YLRandomizer]
                [Warning:YLRandomizer] <b>owner</b>
                [Warning:YLRandomizer]
                [Warning:YLRandomizer]
                [Warning:YLRandomizer]

                [Warning:YLRandomizer] WaitForEventDispatcherEvent_FinalBossPhaseCompleteEvent_OnEvent.Postfix()
                [Warning:YLRandomizer] BeeSwarmAttackedPlayerEvent
                [Warning:YLRandomizer] <b>owner</b> Wait For Bee Swarm Attacked Player
                [Warning:YLRandomizer]
                [Warning:YLRandomizer] <b>owner</b>
                [Warning:YLRandomizer]
                [Warning:YLRandomizer]
                [Warning:YLRandomizer]
             */
        }
    }

    //[HarmonyPatch(typeof(BeeSwarmAttackedPlayerEvent), "OnEvent")]
    //public class BeeSwarmAttackedPlayerEvent_OnEvent
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(int index, Savegame.CollectionStatus status)
    //    {
    //        ManualSingleton<ILogger>.instance.Debug($"BeeSwarmAttackedPlayerEvent_OnEvent.Postfix(): {index}, {status}");
    //    }
    //}
}
