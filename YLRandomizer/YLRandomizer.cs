using HarmonyLib;
using System.Reflection;
using YLRandomizer.Logging;
using System;
using YLRandomizer.Scripts;
using YLRandomizer.Data;
using BepInEx;

namespace YLRandomizer
{
    [BepInPlugin("50feeca9-9265-4972-b7f8-af7d5b4889fb", "YLRandomizer", "0.2.0")]
    public class YLRandomizer : BaseUnityPlugin
    {
        private bool _patched = false;

        private void Awake()
        {
            doSetup();
        }

        private void doSetup()
        {
            ManualSingleton<IUserMessages>.instance = new UserMessages();
            ManualSingleton<ILogger>.instance = new BepInExLogger("YLRandomizer");
            doPatches();
            configureGUI();
            ArchipelagoHeartbeat.CreateNewHeartbeat();
        }

        private void doPatches()
        {
            if (!_patched)
            {
                ManualSingleton<ILogger>.instance.Info("Patching game...");
                try
                {
                    new Harmony("com.github.sunnybat.YLRandomizer").PatchAll(Assembly.GetExecutingAssembly());
                    ManualSingleton<ILogger>.instance.Info("Finished patching game.");
                    _patched = true;
                }
                catch (Exception e)
                {
                    ManualSingleton<ILogger>.instance.Error(e.Message);
                    ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    ManualSingleton<ILogger>.instance.Error("Error patching game. Future patches will be retried, but could break.");
                }
            }
        }

        private void configureGUI()
        {
            // Create a game object that will be responsible to drawing the IMGUI in the Menu.
            var guiGameobject = new UnityEngine.GameObject();
            guiGameobject.AddComponent<ArchipelagoUI>();
            UnityEngine.GameObject.DontDestroyOnLoad(guiGameobject);
        }

        private void getClosestPagie()
        {
            var playerPosition = UnityEngine.Object.FindObjectOfType<PlayerDev>().transform.position;
            var pagies = UnityEngine.Object.FindObjectsOfType<PagiePickup>();
            var closestPagieIndex = -1;
            float closestDistance = 9999999f;
            for (int i = 0; i < pagies.Length; i++)
            {
                if (pagies[i].GetCollectionStatus() != Savegame.CollectionStatus.Collected)
                {
                    var distanceAway = UnityEngine.Vector3.Distance(playerPosition, pagies[i].gameObject.transform.position);
                    if (closestDistance > distanceAway)
                    {
                        closestPagieIndex = i;
                        closestDistance = distanceAway;
                    }
                }
            }
            if (closestPagieIndex >= 0)
            {
                UnityEngine.Debug.Log(pagies[closestPagieIndex].identifier + ": " + pagies[closestPagieIndex].name);
                UnityEngine.Debug.Log(closestDistance);
            }
        }
    }
}
