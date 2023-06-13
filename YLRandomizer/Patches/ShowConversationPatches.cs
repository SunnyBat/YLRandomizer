using HarmonyLib;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(ShowConversation), nameof(ShowConversation.OnConversationEnd))]
    public class ShowConversation_OnConversationEnd
    {
        [HarmonyPostfix]
        public static void Postfix(ConversationEndEvent convoEndEvent)
        {
            ManualSingleton<ILogger>.instance.Debug($"ShowConversation_OnConversationEnd.Postfix(): {convoEndEvent.mConversationData.name}, {convoEndEvent.mInterrupted}, {convoEndEvent.mConversationData.table}");
            ShowQuestion_OnConversationOptionPicked.LastClosedConversation = convoEndEvent.mConversationData.name;

            // Technically not what we want. We want the cutscene itself, which is in SetCutscene, but it's hard to pull any unique identifier from it :(
            if (convoEndEvent.mConversationData.name == "EndCutscene11") // First dialog in cutscene immediately after beating game -- not required to hit if cutscene is skipped
            {
                ManualSingleton<IRandomizer>.instance.SetGameCompleted();
            }
        }
    }
}
