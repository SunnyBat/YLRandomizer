using HarmonyLib;
using System.Diagnostics;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(ShowConversation), "OnExecute")]
    public class ShowConversation_OnExecute
    {
        [HarmonyPostfix]
        public static void Postfix(ShowConversation __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"ShowConversation_OnExecute.Postfix()");
            ManualSingleton<ILogger>.instance.Debug($"- Data.value.name: {__instance.Data.value.name}");
            ManualSingleton<ILogger>.instance.Debug($"- name: {__instance.name}");
            ManualSingleton<ILogger>.instance.Debug($"- description: {__instance.description}");
            ManualSingleton<ILogger>.instance.Debug($"- Data.value.conversation:");
            for (int i = 0; i < __instance.Data.value.conversation.Length; i++)
            {
                ManualSingleton<ILogger>.instance.Debug($"-- conversation[{i}].headIndex: {__instance.Data.value.conversation[i].headIndex}");
                ManualSingleton<ILogger>.instance.Debug($"-- conversation[{i}].textIndex: {__instance.Data.value.conversation[i].textIndex}");
                ManualSingleton<ILogger>.instance.Debug($"-- conversation[{i}].priority: {__instance.Data.value.conversation[i].priority}");
                ManualSingleton<ILogger>.instance.Debug($"-- conversation[{i}].autoProgress: {__instance.Data.value.conversation[i].autoProgress}");
                ManualSingleton<ILogger>.instance.Debug($"-- conversation[{i}].bottomDialog: {__instance.Data.value.conversation[i].bottomDialog}");
            }
        }
    }

    [HarmonyPatch(typeof(ShowConversation), nameof(ShowConversation.OnConversationEnd))]
    public class ShowConversation_OnConversationEnd
    {
        [HarmonyPostfix]
        public static void Postfix(ConversationEndEvent convoEndEvent, ShowConversation __instance)
        {
            ManualSingleton<ILogger>.instance.Info($"ShowConversation_OnConversationEnd.Postfix(): {convoEndEvent.mConversationData.name}, {convoEndEvent.mInterrupted}, {convoEndEvent.mConversationData.table}");
            ShowQuestion_OnConversationOptionPicked.LastClosedConversation = convoEndEvent.mConversationData.name;
        }
    }
}
