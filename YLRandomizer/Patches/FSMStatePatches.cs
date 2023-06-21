#if DEBUG
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
            "TutorialManager",
            "Space_Outhouse_Door_Hits",
            "WalkInLogic",
            "ChallengeLoadLogic", "ChallengeLoadLogic (1)",
            "Anti_Water_At_Enterance", "Anti_Water_Foot_of_Lighthouse", "Anti_Water_Berfore_Inept",
            "Spawn Star Logic",
            "Intro Trigger + Cams",
            "Music_4_SK_Cave",
            "Sad Captain Challenge Logic",
            "GroundPound_Hits",
            "ScoffsAttacks",
            "space_forcefield_electricity (2)",
            "Golf Course Challenge Logic",
            "Green Balloon", "White Balloon", "Red Balloon", "Orange Balloon", "Blue Balloon",
            "Slide_AMB_Music",
            "space_forcefield_destroyable07 (2)",
            "SideCam SpaceGravity",
            "SpaceWaterExclusionZone",
            "Circulate Room Static Camera",
            "IceSwitch_Trigger",
            "PressureSwitch (7)", "PressureSwitch (2)",
            "SonicTrigger Middle",
            "LevelBoundaryExitTrigger",
            "NPC Vendi",
            "Minecart Challenge Manager Space",
            "Space Key 02",
            "Challenge Enter Volume",
            "MoonieLogic",
            "MoonieDialogue",
            "Pipe02CHECKERTRIGGER",
            "Mini-Inept-AI-FSM (1)",
            "Jellyfish_AI1"
        };

        [HarmonyPostfix]
        public static void Postfix(UnityEngine.Component agent, IBlackboard bb)
        {
            if (agent?.name != null && !IgnoredNames.Contains(agent.name))
            {
                ManualSingleton<ILogger>.instance.Debug($"FSMState_OnExecute.Postfix(): {agent?.name}, {bb?.name}");
            }
        }
    }
}
#endif