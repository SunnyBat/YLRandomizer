using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;
using static N64ShaderMap;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(TrowzaInterface), "HasMoveBeenUnlocked")]
    public class TrowzaInterface_HasMoveBeenUnlocked
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(TrowzaMoveInfo moveInfo, ref bool __result)
        {
            //// This is specifically for the Trowza interface to display specific sets of moves per world.
            //// Thus, we're not actually looking for received abilities, but rather sent locations.
            //// Additionally, we don't actually want to check for whether we've unlocked the move itself,
            //// but rather the prerequisite move to get into the next world. This is because having a
            //// requirement to have unlocked the move to display it on the unlock interface is... problematic.
            //var requiredMove = PlayerMoveConverter.GetRequiredAbilityForWorldId(moveInfo.world);
            //var moveId = PlayerMoveConverter.GetLocationIdFromMove(requiredMove);
            //__result = ManualSingleton<IRandomizer>.instance.GetCheckedAbilityLocations().Contains(moveId);
            //if (__result)
            //{
            //    IntermediateActionTracker.RemoveLocallyCheckedLocation(moveId);
            //}
            //else if (IntermediateActionTracker.HasLocationBeenCheckedLocally(moveId))
            //{
            //    __result = true;
            //}
            //ManualSingleton<ILogger>.instance.Debug($"TrowzaInterface_HasMoveBeenUnlocked: {moveId}: {__result}");
            //return false;

            // We instead just let the Trowzer interface manage its own displayed moves. It will display
            // moves based off of whether you've talked to Trowza in specific worlds, which is exactly
            // what we want.
            return true;
        }
    }

    public class TrowzaInterface_ShowShopItems
    {
        [HarmonyPrefix]
        public static bool AlwaysReplace(TrowzaInterface __instance)
        {
            // We have this purely to make the code not be weird. It makes very strong assumptions about
            // things like the order in which you unlock moves. This would normally be fine, but it was
            // harder and more error-prone to do what this function originally did rather than just
            // doing it *normally*, like is implemented below. =/
            var trowzaInterfaceClass = typeof(TrowzaInterface);
            var hasMoveBeenUnlockedFunction = trowzaInterfaceClass.GetMethod("HasMoveBeenUnlocked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            trowzaInterfaceClass.GetMethod("MarkTrowzaAsFound", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[0]);
            bool hasSetFirstDisplayedMovedSelected = false;
            trowzaInterfaceClass.GetMethod("UpdateQuillieText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[0]);
            var m_moveItems = (List<TrowzaMoveItem>)trowzaInterfaceClass.GetField("m_moveItems", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(trowzaInterfaceClass);
            var m_nameConversationUtility = (ConversationGameUtility)trowzaInterfaceClass.GetField("m_nameConversationUtility", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(trowzaInterfaceClass);
            var m_bargainConversationUtility = (ConversationGameUtility)trowzaInterfaceClass.GetField("m_bargainConversationUtility", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(trowzaInterfaceClass);
            var m_playerMoves = (PlayerMoves)trowzaInterfaceClass.GetField("m_playerMoves", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(trowzaInterfaceClass);
            var m_firstSelectedField = trowzaInterfaceClass.GetField("m_firstSelected", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            for (int i = 0; i < m_moveItems.Count; i++)
            {
                TrowzaMoveItem trowzaMoveItem = m_moveItems[i];
                TrowzaMoveInfo trowzaMoveInfo = __instance.data.data[i];
                if ((bool) hasMoveBeenUnlockedFunction.Invoke(__instance, new object[] { trowzaMoveInfo }))
                {
                    trowzaMoveItem.MoveIndex = i;
                    trowzaMoveItem.SetName(m_nameConversationUtility.GetText(trowzaMoveInfo.nameIndex));
                    trowzaMoveItem.SetBargainText(m_bargainConversationUtility.GetText(trowzaMoveInfo.bargainIndex));
                    trowzaMoveItem.SetCost(trowzaMoveInfo.cost);
                    trowzaMoveItem.IsAvailableForSale = true;
                    trowzaMoveItem.IsSold = m_playerMoves.HasLearnedMove(trowzaMoveInfo.move);
                    if (!hasSetFirstDisplayedMovedSelected)
                    {
                        m_firstSelectedField.SetValue(__instance, trowzaMoveItem.ButtonControl.gameObject);
                        hasSetFirstDisplayedMovedSelected = true;
                    }
                }
                else
                {
                    trowzaMoveItem.IsAvailableForSale = false;
                }

                m_moveItems[i].SetAnimatorEnabled(isEnabled: true);
            }

            return false;
        }
    }
}
