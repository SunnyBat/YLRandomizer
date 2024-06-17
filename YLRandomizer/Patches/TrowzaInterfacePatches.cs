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
            var enabledMovesItems = new List<TrowzaMoveItem>();
            var enabledMovesInfo = new List<TrowzaMoveInfo>();
            var disabledMovesItems = new List<TrowzaMoveItem>();
            var disabledMovesInfo = new List<TrowzaMoveInfo>();
            ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_ShowShopItems: {m_moveItems.Count}, {__instance.data.data.Length}");
            for (int i = 0; i < m_moveItems.Count; i++)
            {
                TrowzaMoveItem trowzaMoveItem = m_moveItems[i];
                TrowzaMoveInfo trowzaMoveInfo = __instance.data.data[i];
                if ((bool) hasMoveBeenUnlockedFunction.Invoke(__instance, new object[] { trowzaMoveInfo }))
                {
                    ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_ShowShopItems: {trowzaMoveInfo.move}");
                    trowzaMoveItem.MoveIndex = enabledMovesItems.Count;
                    enabledMovesItems.Add(trowzaMoveItem);
                    enabledMovesInfo.Add(trowzaMoveInfo);
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
                    trowzaMoveItem.MoveIndex = m_moveItems.Count - disabledMovesItems.Count;
                    disabledMovesItems.Add(trowzaMoveItem);
                    disabledMovesInfo.Add(trowzaMoveInfo);
                    trowzaMoveItem.IsAvailableForSale = false;
                }

                m_moveItems[i].SetAnimatorEnabled(isEnabled: true);
            }

            // Need to wait until all enabled moves have been found, then set indeces
            // based off of enabled move count
            for (int i = 0; i < disabledMovesItems.Count; i++)
            {
                disabledMovesItems[i].MoveIndex = enabledMovesItems.Count + i;
            }

            // Set up navigation of each element -- the UI assumes every previous move is
            // available, but this is not always the case. Without this, you can only access
            // moves that are logically "next to" each other starting with the first available
            // Trowzer move in the UI. For example, if you have Glacier, Marsh, and Galaxy
            // moves unlocked, you can only access Glacier and Marsh moves, since the UI will
            // not select the Galaxy move without the Cashino move unlocked.
            if (enabledMovesItems.Count > 1)
            {
                for (int i = 0; i < enabledMovesItems.Count; i++)
                {
                    TrowzaMoveItem previousTrowzaMoveItem;
                    if (i == 0)
                    {
                        previousTrowzaMoveItem = enabledMovesItems.Last();
                    }
                    else
                    {
                        previousTrowzaMoveItem = enabledMovesItems[i - 1];
                    }
                    TrowzaMoveItem currentTrowzaMoveItem = enabledMovesItems[i];
                    TrowzaMoveItem nextTrowzaMoveItem;
                    if (i == enabledMovesItems.Count - 1)
                    {
                        nextTrowzaMoveItem = enabledMovesItems.First();
                    }
                    else
                    {
                        nextTrowzaMoveItem = enabledMovesItems[i + 1];
                    }
                    var newNav = new UnityEngine.UI.Navigation();
                    newNav.mode = UnityEngine.UI.Navigation.Mode.Explicit;
                    newNav.selectOnUp = previousTrowzaMoveItem.ButtonControl;
                    newNav.selectOnDown = nextTrowzaMoveItem.ButtonControl;
                    currentTrowzaMoveItem.ButtonControl.navigation = newNav;
                }
            }

            // Set move info to all the visible moves then all the not visible moves, in order
            var combinedMovesInfo = new List<TrowzaMoveInfo>();
            combinedMovesInfo.AddRange(enabledMovesInfo);
            combinedMovesInfo.AddRange(disabledMovesInfo);
            __instance.data.data = combinedMovesInfo.ToArray();
            var combinedMoveItems = new List<TrowzaMoveItem>();
            combinedMoveItems.AddRange(enabledMovesItems);
            combinedMoveItems.AddRange(disabledMovesItems);
            trowzaInterfaceClass.GetField("m_moveItems", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(__instance, combinedMoveItems);

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
