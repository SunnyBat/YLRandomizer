using HarmonyLib;
using Newtonsoft.Json;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(ConversationManager), nameof(ConversationManager.QueueConversation), typeof(ConversationData), typeof(ConversationProgression), typeof(bool))]
    public class ConversationManager_QueueConversation
    {
        [HarmonyPrefix]
        public static void SometimesReplace(ConversationData data, ConversationProgression progression, bool capitaliseText)
        {
            ConversationGameUtility conversationUtility = new ConversationGameUtility(data, capitaliseText);
            ManualSingleton<ILogger>.instance.Debug("=== ConversationManager_QueueConversation() ===");
            ManualSingleton<ILogger>.instance.Debug(data.name);
            ManualSingleton<ILogger>.instance.Debug(data.table);
            foreach (var convo in data.conversation)
            {
                ManualSingleton<ILogger>.instance.Debug($"{convo.bottomDialog} - {convo.textIndex} - {convo.headIndex} - {convo.priority}");
            }
            for (var i = 0; i < conversationUtility.Count; i++)
            {
                ManualSingleton<ILogger>.instance.Debug($"{conversationUtility.GetText(i)}");
                
            }
            ManualSingleton<ILogger>.instance.Debug("=== END ConversationManager_QueueConversation() ===");
        }
    }
}
