using HarmonyLib;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
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
