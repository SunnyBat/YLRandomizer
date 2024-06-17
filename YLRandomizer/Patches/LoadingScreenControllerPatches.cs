using HarmonyLib;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using YLRandomizer.Data;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;

namespace YLRandomizer.Patches
{
    [HarmonyPatch(typeof(LoadingScreenController), nameof(LoadingScreenController.LoadScene))]
    public class LoadingScreenController_LoadScene
    {
        [HarmonyPrefix]
        public static void ModifyParameters(ref string sceneName, ref string playerStartTransform, ref string playerEnterFSM, bool saveGame, LoadingScreenFade fadeMask, bool unpauseGame)
        {
            // TODO Use SavegameManager.isWorldUnlocked()
            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): {sceneName}, {playerStartTransform}, {playerEnterFSM}, {saveGame}, {fadeMask}, {unpauseGame}");
            GameState.SetLoadingScreenStarted();
            // SceneManager.GetActiveScene() -- something like this?
            var currentSceneName = SceneManager.GetActiveScene().name;
            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Scene: {currentSceneName}");
            if (ArchipelagoDataHandler.TryGetSlotData(Constants.CONFIGURATION_NAME_WORLDORDER, out Newtonsoft.Json.Linq.JArray worldOrderFromServer) && worldOrderFromServer.Count == 5)
            {
                var randomizedWorldOrder = worldOrderFromServer.ToArray();
                ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Randomized world order: {randomizedWorldOrder.Join()}");
                var isHubToWorld = Array.IndexOf(Constants.ValidRemappingHubWorldSceneNames, currentSceneName) >= 0;
                ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): isHubToWorld: {isHubToWorld}");
                if (isHubToWorld)
                {
                    for (var i = 0; i < Constants.VanillaSceneNameWorldOrder.Length; i++)
                    {
                        var sceneList = Constants.VanillaSceneNameWorldOrder[i];
                        var sceneToVanillaWorldIdentifierIndex = Array.IndexOf(sceneList, sceneName);
                        if (sceneToVanillaWorldIdentifierIndex >= 0)
                        {
                            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Vanilla logical world index: {i}");
                            var newWorldOptionsId = randomizedWorldOrder[i].ToString();
                            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Randomized world options ID: {newWorldOptionsId}");
                            var newLogicalIndexFromRandomizedWorldOrder = Array.IndexOf(Constants.VanillaConfigurationOptionsWorldOrder, newWorldOptionsId);
                            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Randomized logical world index: {newLogicalIndexFromRandomizedWorldOrder}");
                            if (newLogicalIndexFromRandomizedWorldOrder != -1)
                            {
                                // Use expansion state of original tome, not world being targeted
                                var savegameManagerWorldIndexOfTome = Constants.WorldIndexOrder[i];
                                var worldData = SavegameManager.instance.savegame.worlds[savegameManagerWorldIndexOfTome];
                                if (worldData.worldUnlocked)
                                {
                                    var isTomeExpanded = worldData.worldExpanded;
                                    ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Tome expanded: {isTomeExpanded}");
                                    sceneName = Constants.VanillaSceneNameWorldOrder[newLogicalIndexFromRandomizedWorldOrder][isTomeExpanded ? 1 : 0];
                                    playerStartTransform = "";
                                    playerEnterFSM = "";
                                    ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): New scene: {sceneName}");
                                    GameStatManager.instance.SetGameStat(EGameStats.CurrentLoadStartPoint, 9 + newLogicalIndexFromRandomizedWorldOrder + (isTomeExpanded ? 5 : 0));
                                }
                                else
                                {
                                    ManualSingleton<ILogger>.instance.Error($"World {i} is not unlocked! Defaulting back to Shipwreck Creek. This should be reported as a bug.");
                                    ManualSingleton<ILogger>.instance.Error($"Data: {sceneName} | {playerStartTransform} | {currentSceneName} | {randomizedWorldOrder.Join()} | {isHubToWorld} | {i} | {newWorldOptionsId} | {newLogicalIndexFromRandomizedWorldOrder} | {savegameManagerWorldIndexOfTome}");
                                    sceneName = "Level_00_Hub_A";
                                    playerStartTransform = "";
                                    playerEnterFSM = "EntryFSM";
                                    GameStatManager.instance.SetGameStat(EGameStats.CurrentLoadStartPoint, 0);
                                }
                            }
                            else
                            {
                                ManualSingleton<ILogger>.instance.Error($"Unable to find randomized world translation! Defaulting back to Shipwreck Creek. This should be reported as a bug.");
                                ManualSingleton<ILogger>.instance.Error($"Data: {sceneName} | {playerStartTransform} | {currentSceneName} | {randomizedWorldOrder.Join()} | {isHubToWorld} | {i} | {newWorldOptionsId} | {newLogicalIndexFromRandomizedWorldOrder}");
                                sceneName = "Level_00_Hub_A";
                                playerStartTransform = "";
                                playerEnterFSM = "EntryFSM";
                                GameStatManager.instance.SetGameStat(EGameStats.CurrentLoadStartPoint, 0);
                            }
                            break;
                        }
                    }
                }
                else if (Constants.ValidRemappingHubWorldSceneNames.Contains(sceneName))
                {
                    for (var i = 0; i < Constants.VanillaSceneNameWorldOrder.Length; i++)
                    {
                        var sceneList = Constants.VanillaSceneNameWorldOrder[i];
                        ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Evaluating sceneList: {sceneList.Join()}");
                        var sceneToVanillaWorldIdentifierIndex = Array.IndexOf(sceneList, currentSceneName);
                        if (sceneToVanillaWorldIdentifierIndex >= 0)
                        {
                            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Index: {i}");
                            var randomizedWorldOptionsIdentifier = Constants.VanillaConfigurationOptionsWorldOrder[i];
                            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): Randomized ID: {randomizedWorldOptionsIdentifier}");
                            var newLogicalIndexFromRandomizedWorldOrder = Array.IndexOf(randomizedWorldOrder, randomizedWorldOptionsIdentifier);
                            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): New index: {newLogicalIndexFromRandomizedWorldOrder}");
                            if (newLogicalIndexFromRandomizedWorldOrder != -1)
                            {
                                var worldInfo = WorldInfo.instance.worldSetup.worlds[Constants.WorldIndexOrder[newLogicalIndexFromRandomizedWorldOrder]];
                                sceneName = worldInfo.hubScene;
                                playerStartTransform = worldInfo.hubSpawn;
                                playerEnterFSM = worldInfo.hubStartFSM;
                                ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): New scene: {sceneName}");
                                ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadScene.ModifyParameters(): New PST/FSM: {playerStartTransform}");
                                GameStatManager.instance.SetGameStat(EGameStats.CurrentLoadStartPoint, 4 + newLogicalIndexFromRandomizedWorldOrder);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(LoadingScreenController), nameof(LoadingScreenController.LoadNextScene))]
    public class LoadingScreenController_LoadNextScene
    {
        [HarmonyPrefix]
        public static void NeverReplace(string sceneName, LoadingScreenFade fadeMask)
        {
            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_LoadNextScene.NeverReplace(): {sceneName}, {fadeMask}");
            GameState.SetLoadingScreenStarted();
        }
    }

    [HarmonyPatch(typeof(LoadingScreenController), nameof(LoadingScreenController.PlayOutroAnimation))]
    public class LoadingScreenController_PlayOutroAnimation
    {
        [HarmonyPrefix]
        public static void NeverReplace()
        {
            ManualSingleton<ILogger>.instance.Debug($"LoadingScreenController_PlayOutroAnimation.NeverReplace()");
            GameState.SetLoadingScreenFinished();
        }
    }
}
