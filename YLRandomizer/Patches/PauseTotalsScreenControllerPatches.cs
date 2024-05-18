﻿using HarmonyLib;
using System.Collections;
using System.Linq;
using YLRandomizer.Data;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(PauseTotalsScreenController), "FillElements")]
    public class PauseTotalsScreenController_FillElements
    {
        private const string TRIPLE_TOTAL_LABELLED = "RCV/SND/T";
        private const string TRIPLE_TOTAL_FORMATTABLE = "{0}/{1}/{2}";
        [HarmonyPostfix]
        public static IEnumerator Postfix(IEnumerator result, PauseTotalsScreenController __instance)
        {
            ManualSingleton<ILogger>.instance.Debug($"PauseTotalsScreenController_FillElements.SometimesReplace()");
            while (result?.MoveNext() ?? false)
            {
                yield return result.Current;
            }
            ManualSingleton<ILogger>.instance.Debug($"PauseTotalsScreenController_FillElements.SometimesReplace(): Done waiting");
            var m_currentWorldTotalsBeingShown = (int) typeof(PauseTotalsScreenController)
                .GetField("m_currentWorldTotalsBeingShown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(__instance);
            //var conversationGameUtility = (ConversationGameUtility)typeof(PauseTotalsScreenController).GetField("m_conversationGameUtility").GetValue(__instance);
            if (m_currentWorldTotalsBeingShown == 0) // Totals window
            {
                ManualSingleton<ILogger>.instance.Debug($"PauseTotalsScreenController_FillElements.SometimesReplace(): Updating Totals window");
                CollectiblesInfo collectiblesInfo = (!(SavegameManager.instance != null)) ? new CollectiblesInfo() : SavegameManager.instance.collectiblesTotals;
                int pagieReceiveCount = ManualSingleton<IRandomizer>.instance?.GetReceivedPagiesCount() ?? 0;
                int pagieSentCount = SavegameManager.instance.GetAllPagieCount();
                int healthExtenderReceiveCount = ManualSingleton<IRandomizer>.instance?.GetReceivedHealthExtenderCount() ?? 0;
                int healthExtenderSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.HealthExtendersCollected);
                int energyExtenderReceiveCount = ManualSingleton<IRandomizer>.instance?.GetReceivedEnergyExtenderCount() ?? 0;
                int energyExtenderSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.EnergyExtendersCollected);
                int playCoinReceiveCount = ManualSingleton<IRandomizer>.instance?.GetReceivedPlayCoins().Count(val => val) ?? 0;
                int playCoinSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.ArcadeTokensCollected);
                int mollycoolReceiveCount = ManualSingleton<IRandomizer>.instance?.GetReceivedMollycools().Count(val => val) ?? 0;
                int mollycoolSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.TransformationTokensCollected);
                __instance.m_quillText.text = TRIPLE_TOTAL_LABELLED; // Probably can't localize this :(
                __instance.m_pagieText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, pagieReceiveCount, pagieSentCount, collectiblesInfo.pagieMaximum);
                __instance.m_healthExtenderText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, healthExtenderReceiveCount, healthExtenderSentCount, collectiblesInfo.healthExtenderMaximum);
                __instance.m_energyExtenderText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, energyExtenderReceiveCount, energyExtenderSentCount, collectiblesInfo.energyExtenderMaximum);
                __instance.m_arcadeTokenText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, playCoinReceiveCount, playCoinSentCount, collectiblesInfo.arcadeTokenMaximum);
                __instance.m_transformationTokenText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, mollycoolReceiveCount, mollycoolSentCount, collectiblesInfo.transformationTokenMaximum);
            }
            else
            {
                ManualSingleton<ILogger>.instance.Debug($"PauseTotalsScreenController_FillElements.SometimesReplace(): Updating World {m_currentWorldTotalsBeingShown} window");
                var m_worldSetupLookup = (int[])typeof(PauseTotalsScreenController)
                    .GetField("m_worldSetupLookup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(__instance);
                int worldIndex = m_worldSetupLookup[m_currentWorldTotalsBeingShown - 1];
                WorldData worldData = __instance.m_worldSetup.worlds[worldIndex];
                string worldName = ((EWorlds)m_currentWorldTotalsBeingShown).ToString();
                int pagieSentCount = SavegameManager.instance.savegame.worlds[m_currentWorldTotalsBeingShown].pagieCount;
                var playCoinIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex] - 1;
                var receivedPlayCoins = ManualSingleton<IRandomizer>.instance.GetReceivedPlayCoins();
                int playCoinReceiveCount = playCoinIndex >= 0 ? receivedPlayCoins[playCoinIndex] ? 1 : 0 : 0;
                int playCoinSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.ArcadeTokensCollected.ToString() + worldName);
                var mollycoolIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex] - 1;
                var receivedMollycools = ManualSingleton<IRandomizer>.instance.GetReceivedMollycools();
                int mollycoolReceiveCount = mollycoolIndex >= 0 ? receivedMollycools[mollycoolIndex] ? 1 : 0 : 0;
                int mollycoolSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.TransformationTokensCollected.ToString() + worldName);
                int healthExtenderSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.HealthExtendersCollected.ToString() + worldName);
                int energyExtenderSentCount = GameStatManager.instance.GetCurrentValue(EGameStats.EnergyExtendersCollected.ToString() + worldName);
                __instance.m_quillText.text = TRIPLE_TOTAL_LABELLED; // Probably can't localize this :(
                __instance.m_pagieText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, "-", pagieSentCount, worldData.pagieMaximum);
                __instance.m_healthExtenderText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, "-", healthExtenderSentCount, worldData.healthExtenderMaximum);
                __instance.m_energyExtenderText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, "-", energyExtenderSentCount, worldData.energyExtenderMaximum);
                __instance.m_arcadeTokenText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, playCoinReceiveCount, playCoinSentCount, worldData.arcadeTokenMaximum);
                __instance.m_transformationTokenText.text = string.Format(TRIPLE_TOTAL_FORMATTABLE, mollycoolReceiveCount, mollycoolSentCount, worldData.transformationTokenMaximum);
            }
        }
    }
}
