using System.Collections.Generic;
using System.Linq;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Data
{
    public class ArchipelagoDataHandler
    {
        public static void HookUpEventSubscribers()
        {
            ManualSingleton<IRandomizer>.instance.MessageReceived += (message) =>
            {
                ManualSingleton<Logging.ILogger>.instance.Info(message);
                ManualSingleton<IUserMessages>.instance.AddMessage(message);
            };
            ManualSingleton<IRandomizer>.instance.ItemReceived += (itemId) =>
            {
                if (GameState.IsInGame())
                {
                    var playerInstance = SavegameManager.instance?.savegame?.player;
                    if (playerInstance != null)
                    {
                        playerInstance.unspentPagies++;
                        HudController.instance.UpdatePagieCounter(false); // TODO Show or not show?
                    }
                    ManualSingleton<Logging.ILogger>.instance.Info("Pagie received: " + itemId);
                }
            };
            ManualSingleton<IRandomizer>.instance.LocationReceived += (locationId) =>
            {
                if (GameState.IsInGame())
                {
                    ManualSingleton<Logging.ILogger>.instance.Info("Location marked as checked: " + locationId);
                    var apPagieData = ArchipelagoLocationConverter.GetPagieInfo(locationId);
                    var worldPagieData = SavegameManager.instance?.savegame?.worlds?[apPagieData.Item1]?.pagies;
                    if (worldPagieData != null)
                    {
                        worldPagieData[apPagieData.Item2] = Savegame.CollectionStatus.Collected;
                    }
                }
            };
            ManualSingleton<IRandomizer>.instance.ReadyToUse += () =>
            {
                UpdateCurrentGameStateToAP();
            };
        }

        public static void UpdateCurrentGameStateToAP()
        {
            if (GameState.IsInGame())
            {
                // === HANDLE EXISTING LOCATIONS ===
                // - Convert save to location list
                var locationIds = new List<long>();
                for (int i = 1; i < 7; i++) // Length 8; 0 and 7 are unused
                {
                    var currentWorld = SavegameManager.instance.savegame.worlds[i];
                    for (int j = 0; j < currentWorld.pagies.Length; j++)
                    {
                        if (currentWorld.pagies[j] == Savegame.CollectionStatus.Collected)
                        {
                            locationIds.Add(ArchipelagoLocationConverter.GetLocationId(i, j));
                        }
                    }
                }
                // - Send location list to Archipelago
                ManualSingleton<IRandomizer>.instance.LocationChecked(locationIds.ToArray());

                // === HANDLE NEW LOCATIONS ===
                // - Read all locations from Archipelago
                var foundLocations = ManualSingleton<IRandomizer>.instance.GetAllCheckedLocations();
                // - Set relevant pagies from Archipelago as found
                for (int i = 0; i < foundLocations.Length; i++)
                {
                    var pagieData = ArchipelagoLocationConverter.GetPagieInfo(foundLocations[i]);
                    SavegameManager.instance.savegame.worlds[pagieData.Item1].pagies[pagieData.Item2] = Savegame.CollectionStatus.Collected;
                }

                // === HANDLE RECEIVED ITEMS ===
                // - Calculate pagies spent on worlds
                var spentPagies = 0;
                for (int i = 1; i < 7; i++)
                {
                    if (SavegameManager.instance.savegame.worlds[i].worldUnlocked)
                    {
                        switch (i)
                        {
                            case 2:
                                spentPagies++;
                                break;
                            case 3:
                                spentPagies += 12;
                                break;
                            case 4:
                                spentPagies += 10;
                                break;
                            case 5:
                                spentPagies += 3;
                                break;
                            case 6:
                                spentPagies += 7;
                                break;
                        }
                    }
                    if (SavegameManager.instance.savegame.worlds[i].worldExpanded)
                    {
                        switch (i)
                        {
                            case 2:
                                spentPagies += 3;
                                break;
                            case 3:
                                spentPagies += 15;
                                break;
                            case 4:
                                spentPagies += 11;
                                break;
                            case 5:
                                spentPagies += 5;
                                break;
                            case 6:
                                spentPagies += 8;
                                break;
                        }
                    }
                }
                // - Set unspent pagies based off of items from Archipelago and worlds unlocked
                var totalPagiesReceived = ManualSingleton<IRandomizer>.instance.GetReceivedPagiesCount();
                SavegameManager.instance.savegame.player.unspentPagies = totalPagiesReceived - spentPagies;

                // == HANDLE GAME STATE ==
                // TODO Maybe make this slightly better, eg don't check tonics but instead just directly call HasConditionBeenMet() (or similar)
                // with the correct conditions? That way we don't have to go through a Tonic's indirect requirements.
                var endGameTonicInfo = TonicManager.instance.UnlockRequirements.FindAll(req => req.Tonic == ETonics.Athlete);
                if (endGameTonicInfo.Count > 0 && endGameTonicInfo.All(req => GameStatManager.instance.HasConditionBeenMet(req.Condition)))
                {
                    ManualSingleton<IRandomizer>.instance.SetGameCompleted();
                }
                else
                {
                    ManualSingleton<IRandomizer>.instance.SetInGame();
                }
            }
        }
    }
}
