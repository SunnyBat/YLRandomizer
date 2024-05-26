using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(TrowzaInterface), "ShowShopItems")]
    public class TrowzaInterface_ShowShopItems
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(TrowzaInterface __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_ShowShopItems()");
            // We have this purely to make the code not be weird. It makes very strong assumptions about
            // things like the order in which you unlock moves. This would normally be fine, but it was
            // harder and more error-prone to do what this function originally did rather than just
            // doing it *normally*, like is implemented below. =/
            var trowzaInterfaceClass = typeof(TrowzaInterface);
            var hasMoveBeenUnlockedFunction = trowzaInterfaceClass.GetMethod("HasMoveBeenUnlocked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            trowzaInterfaceClass.GetMethod("MarkTrowzaAsFound", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[0]);
            bool hasSetFirstDisplayedMovedSelected = false;
            trowzaInterfaceClass.GetMethod("UpdateQuillieText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[0]);
            var m_moveItems = (List<TrowzaMoveItem>)trowzaInterfaceClass.GetField("m_moveItems", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            var m_nameConversationUtility = (ConversationGameUtility)trowzaInterfaceClass.GetField("m_nameConversationUtility", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            var m_bargainConversationUtility = (ConversationGameUtility)trowzaInterfaceClass.GetField("m_bargainConversationUtility", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            var m_firstSelectedField = trowzaInterfaceClass.GetField("m_firstSelected", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            for (int i = 0; i < m_moveItems.Count; i++)
            {
                TrowzaMoveItem trowzaMoveItem = m_moveItems[i];
                TrowzaMoveInfo trowzaMoveInfo = __instance.data.data[i];
                if ((bool) hasMoveBeenUnlockedFunction.Invoke(__instance, new object[] { trowzaMoveInfo }))
                {
                    ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_ShowShopItems: {trowzaMoveInfo.move}");
                    trowzaMoveItem.MoveIndex = i;
                    trowzaMoveItem.SetName(m_nameConversationUtility.GetText(trowzaMoveInfo.nameIndex));
                    trowzaMoveItem.SetBargainText(m_bargainConversationUtility.GetText(trowzaMoveInfo.bargainIndex));
                    trowzaMoveItem.SetCost(trowzaMoveInfo.cost);
                    trowzaMoveItem.IsAvailableForSale = true;
                    trowzaMoveItem.IsSold = _hasUnlockedShopMove(trowzaMoveInfo.move);
                    if (!hasSetFirstDisplayedMovedSelected)
                    {
                        m_firstSelectedField.SetValue(__instance, trowzaMoveItem.ButtonControl.gameObject);
                        hasSetFirstDisplayedMovedSelected = true;
                    }
                    ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_ShowShopItems: {trowzaMoveInfo.move}: IsSold: {trowzaMoveItem.IsSold}");
                }
                else
                {
                    ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_ShowShopItems: {trowzaMoveInfo.move}: Not available for sale");
                    trowzaMoveItem.IsAvailableForSale = false;
                }

                m_moveItems[i].SetAnimatorEnabled(isEnabled: true);
            }

            return false;
        }

        private static bool _hasUnlockedShopMove(PlayerMoves.Moves moveEnum)
        {
            var moveId = ItemAndLocationIdConverter.GetLocationIdFromMove(moveEnum);
            var ret = ManualSingleton<IRandomizer>.instance.GetCheckedAbilityLocations().Contains(moveId);
            if (ret)
            {
                IntermediateActionTracker.RemoveLocallyCheckedLocation(moveId);
            }
            else if (IntermediateActionTracker.HasLocationBeenCheckedLocally(moveId))
            {
                ret = true;
            }
            return ret;
        }
    }
}
