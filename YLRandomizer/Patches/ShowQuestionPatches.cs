using HarmonyLib;
using ParadoxNotion;
using System;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

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
                int requiredPagieCount = 0;
                for (int i = 0; i < Constants.WorldIndexOrder.Length; i++)
                {
                    int unlockEventNamesIndex = i * 2;
                    ManualSingleton<ILogger>.instance.Info($"Adding {Constants.UnlockEventNames[unlockEventNamesIndex]} ({WorldUnlockCalculator.getCostForWorldUnlock(Constants.WorldIndexOrder[i])})");
                    requiredPagieCount += WorldUnlockCalculator.getCostForWorldUnlock(Constants.WorldIndexOrder[i]);
                    if (Constants.UnlockEventNames[unlockEventNamesIndex] == LastClosedConversation)
                    {
                        break;
                    }

                    ManualSingleton<ILogger>.instance.Info($"Adding {Constants.UnlockEventNames[unlockEventNamesIndex + 1]} ({WorldUnlockCalculator.getCostForWorldExpand(Constants.WorldIndexOrder[i])})");
                    requiredPagieCount += WorldUnlockCalculator.getCostForWorldExpand(Constants.WorldIndexOrder[i]);
                    if (Constants.UnlockEventNames[unlockEventNamesIndex + 1] == LastClosedConversation)
                    {
                        break;
                    }
                }
                int receivedPagieCount = ManualSingleton<IRandomizer>.instance.GetReceivedPagiesCount();
                if (requiredPagieCount > receivedPagieCount)
                {
                    // TODO Change from IUserMessages to a custom Conversation and use QueueConversation in ShowConversation to display it
                    ManualSingleton<IUserMessages>.instance.AddMessage($"You must have enough pagies to unlock and expand all worlds ahead of this before unlocking this one. ({receivedPagieCount}/{requiredPagieCount})");
                    var showQuestionClass = typeof(ShowQuestion);
                    var sendEventFunction = showQuestionClass.GetMethod("SendEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(EventData) }, null);
                    sendEventFunction.Invoke(__instance, new object[] { new EventData(__instance.OptionDeclined.value) });
                    return false;
                }
            }
            return true;
        }
    }
}
