using HarmonyLib;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    public class SetCutscenePatches
    {
        private static readonly UnityEngine.Vector3 postGameCutscenePosition = new UnityEngine.Vector3(-16.41f, -4.57f, -27.62144f);
        private static readonly UnityEngine.Vector3 postGameCutsceneRight = new UnityEngine.Vector3(1f, 0f, 0f);
        private static readonly UnityEngine.Vector3 postGameCutsceneEulerAngles = new UnityEngine.Vector3(0f, 0f, 0f);

        [HarmonyPatch(typeof(SetCutscene), "StartCutscene")]
        public class SetCutscene_StartCutscene
        {
            [HarmonyPostfix]
            public static void Postfix(SetCutscene __instance)
            {
                ManualSingleton<ILogger>.instance.Debug($"SetCutscene_StartCutscene.Postfix()");
                // TODO Verify this is the only cutscene with this specific position
                var playerSpawnPoint = __instance?.PlayerSpawnPoint?.value;
                if (playerSpawnPoint?.position == postGameCutscenePosition && playerSpawnPoint.right == postGameCutsceneRight && playerSpawnPoint.eulerAngles == postGameCutsceneEulerAngles)
                {
                    ManualSingleton<IRandomizer>.instance.SetGameCompleted();
                }
            }
        }
    }
}
