using HarmonyLib;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;
using static AttackBase;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(Attacker), nameof(Attacker.OnAttackSuccessful))]
    public class Attacker_OnAttackSuccessful
    {
        [HarmonyPostfix]
        public static void NeverReplace(AttackHitInfo attackHitInfo)
        {
            ManualSingleton<ILogger>.instance.Debug($"Attacker_OnAttackSuccessful.NeverReplace():");
            ManualSingleton<ILogger>.instance.Debug($"- Attacker.Name: {attackHitInfo.Attacker.name}");
            ManualSingleton<ILogger>.instance.Debug($"- Attacked.Name: {attackHitInfo.Attacked.name}");
            ManualSingleton<ILogger>.instance.Debug($"- AttackedCollider.Name: {attackHitInfo.AttackedCollider.name}");
            ManualSingleton<ILogger>.instance.Debug($"- Attack.Name: {attackHitInfo.Attack.name}");
            ManualSingleton<ILogger>.instance.Debug($"- AttackType: {attackHitInfo.AttackType}");
            if (attackHitInfo.Attacked.name == "PlayerKamBatV5")
            {
                ManualSingleton<ILogger>.instance.Debug($"Attacker_OnAttackSuccessful.NeverReplace(): Player was attacked");
                var playerHealth = attackHitInfo.Attacked.GetComponentInChildren<PlayerHealth>();
                if (playerHealth == null)
                {
                    ManualSingleton<ILogger>.instance.Warning($"Attacker_OnAttackSuccessful.NeverReplace(): Player was attacked, but PlayerHealth is unavailable. Skipped DeathLink check.");
                }
                else if (playerHealth.CurrentHealth <= 0)
                {
                    ManualSingleton<ILogger>.instance.Debug($"Attacker_OnAttackSuccessful.NeverReplace(): Player was killed");
                    ManualSingleton<IRandomizer>.instance.SendDeathLink($"was killed by {attackHitInfo.AttackType} {attackHitInfo.Attack.name}");
                }
            }
        }
    }
}
