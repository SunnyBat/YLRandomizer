using HarmonyLib;
using ParadoxNotion;
using System;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(ShowQuestion), nameof(ShowQuestion.OnConversationOptionPicked))]
    public class ShowQuestion_OnConversationOptionPicked
    {
        public static string LastClosedConversation;

        [HarmonyPrefix]
        public static bool SometimesReplace(Conversation.DialogOption option, ShowQuestion __instance)
        {
            ManualSingleton<ILogger>.instance.Info($"ShowQuestion_OnConversationOptionPicked.Postfix(): {option}");
            if (option == Conversation.DialogOption.Accept && Constants.UnlockEventNames.Contains(LastClosedConversation))
            {
                // TODO Calculate if player has enough pagies to unlock and expand all previous worlds plus this one before allowing
                // TODO Change from IUserMessages to a custom Conversation and use QueueConversation in ShowConversation to display it
                ManualSingleton<IUserMessages>.instance.AddMessage("You must have enough pagies to unlock all of Tribalstack Tropics first.");
                var showQuestionClass = typeof(ShowQuestion);
                var sendEventFunction = showQuestionClass.GetMethod("SendEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(EventData) }, null);
                sendEventFunction.Invoke(__instance, new object[] { new EventData(__instance.OptionDeclined.value) });
                return false;
            }
            return true;
        }
    }
}
