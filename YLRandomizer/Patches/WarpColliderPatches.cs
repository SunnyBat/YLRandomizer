using HarmonyLib;
using System;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(WarpCollider), "OnCollisionEnter", new Type[] { typeof(UnityEngine.Collision) })]
    public class WarpCollider_OnCollisionEnter
    {
        [HarmonyPostfix]
        public static void NeverReplace(UnityEngine.Collision collisionInfo)
        {
            ManualSingleton<ILogger>.instance.Debug($"WarpCollider_OnCollisionEnter.NeverReplace():");
            //ManualSingleton<ILogger>.instance.Debug($"- {collisionInfo.gameObject?.name}");
            //foreach (var contact in collisionInfo.contacts)
            //{
            //    ManualSingleton<ILogger>.instance.Debug($"---");
            //    ManualSingleton<ILogger>.instance.Debug($"- {contact.thisCollider.name}");
            //    ManualSingleton<ILogger>.instance.Debug($"- {contact.otherCollider.name}");
            //}
            //ManualSingleton<ILogger>.instance.Debug($"- {collisionInfo.contacts}");
            //ManualSingleton<ILogger>.instance.Debug($"- {collisionInfo.collider.name}");
            //ManualSingleton<ILogger>.instance.Debug($"- {collisionInfo.collider.bounds}");
            //ManualSingleton<ILogger>.instance.Debug($"- {collisionInfo.gameObject?.name}");
        }
    }
}
