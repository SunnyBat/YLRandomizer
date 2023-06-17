using HarmonyLib;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using System.Linq;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(FSMState), "OnExecute")]
    public class FSMState_OnExecute
    {
        private static readonly string[] IgnoredNames = new string[] {
            "Mr Blowy on off with FSM", "Mr Blowy on off with FSM (1)", "Mr Blowy on off with FSM (2)", "Mr Blowy on off with FSM (3)",
            "slide hazard on off (1)", "slide hazard off on (2)",
            "Mini-Inept-AI-FSM",
            "TutorialManager"
        };

        [HarmonyPostfix]
        public static void Postfix(UnityEngine.Component agent, IBlackboard bb)
        {
            if (agent?.name != null &&!IgnoredNames.Contains(agent?.name))
            {
                ManualSingleton<ILogger>.instance.Debug($"FSMState_OnExecute.Postfix(): {agent?.name}, {bb?.name}");
            }
        }
    }
}
