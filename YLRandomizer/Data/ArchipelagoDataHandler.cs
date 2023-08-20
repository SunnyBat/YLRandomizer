using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Randomizer;
using static PlayerXFModels;

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
                ManualSingleton<Logging.ILogger>.instance.Debug("Item received: " + itemId);
                if (GameState.IsInGame())
                {
                    if (itemId == Constants.PAGIE_ITEM_ID)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("Pagie received: " + itemId);
                        var playerInstance = SavegameManager.instance?.savegame?.player;
                        if (playerInstance != null)
                        {
                            playerInstance.unspentPagies++;
                            HudController.instance.UpdatePagieCounter(false); // TODO Show or not show?
                        }
                    }
                    else if (itemId >= Constants.MOLLYCOOL_ITEM_ID_START && itemId <= Constants.MOLLYCOOL_ITEM_ID_START + 4)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("Mollycool received: " + itemId);
                        // TODO Check whether we want to update collection status. Assuming no since we override check elsewhere, but if that logic needs to change then put it here.
                    }
                    else if (itemId >= Constants.PLAYCOIN_ITEM_ID_START && itemId <= Constants.PLAYCOIN_ITEM_ID_START + 4)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("Playcoin received: " + itemId);
                        // TODO Check whether we want to update collection status. Assuming no since we override check elsewhere, but if that logic needs to change then put it here.
                    }
                    else if (itemId == Constants.HEALTH_EXTENDER_ITEM_ID)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("HealthExtender received: " + itemId);
                        SavegameManager.instance.savegame.player.healthExtenderTokenCount++;
                    }
                    else if (itemId == Constants.ENERGY_EXTENDER_ITEM_ID)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("EnergyExtender received: " + itemId);
                        SavegameManager.instance.savegame.player.specialExtenderTokenCount++;
                    }
                    else if (itemId >= Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START && itemId <= Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 8)
                    {
                        var move = PlayerMoveConverter.GetMoveFromItemId(itemId);
                        ManualSingleton<Logging.ILogger>.instance.Info("Move received: " + move);
                        SavegameManager.instance.EnableMove(move, true, false);
                    }
                    else
                    {
                        ManualSingleton<Logging.ILogger>.instance.Warning("Unknown item received: " + itemId);
                    }
                }
            };
            // TODO remove, we synchronize at the beginning so we don't need to track locations
            // Just extra work for no reason
            ManualSingleton<IRandomizer>.instance.LocationReceived += (locationId) =>
            {
                if (GameState.IsInGame())
                {
                    ManualSingleton<Logging.ILogger>.instance.Debug("Location marked as checked: " + locationId);
                    //var apPagieData = ArchipelagoLocationConverter.GetPagieInfo(locationId);
                    //var worldPagieData = SavegameManager.instance?.savegame?.worlds?[apPagieData.Item1]?.pagies;
                    //if (worldPagieData != null)
                    //{
                    //    worldPagieData[apPagieData.Item2] = Savegame.CollectionStatus.Collected;
                    //}
                }
            };
            ManualSingleton<IRandomizer>.instance.ReadyToUse += () =>
            {
                UpdateCurrentGameStateToAP();
            };
        }

        public static void UpdateCurrentGameStateToAP(bool forceLoad = false)
        {
            if (GameState.IsInGame() || forceLoad)
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
                            locationIds.Add(ArchipelagoLocationConverter.GetPagieLocationId(i, j));
                        }
                    }
                    // TODO Play Coins, Mollycools, Abilities
                }
                // - Send location list to Archipelago
                ManualSingleton<IRandomizer>.instance.LocationChecked(locationIds.ToArray());

                // === HANDLE NEW LOCATIONS ===
                // - Read all locations from Archipelago
                var foundPagieLocations = ManualSingleton<IRandomizer>.instance.GetCheckedPagieLocations();
                // - Set relevant pagies from Archipelago as found
                for (int i = 0; i < foundPagieLocations.Length; i++)
                {
                    var pagieData = ArchipelagoLocationConverter.GetPagieInfo(foundPagieLocations[i]);
                    SavegameManager.instance.savegame.worlds[pagieData.Item1].pagies[pagieData.Item2] = Savegame.CollectionStatus.Collected;
                }

                var foundMollycoolLocations = ManualSingleton<IRandomizer>.instance.GetCheckedMollycoolLocations();
                for (int i = 0; i < 5; i++)
                {
                    SavegameManager.instance.savegame.worlds[Constants.WorldIndexOrder[i]].transformtoken = foundMollycoolLocations.Contains(Constants.MOLLYCOOL_LOCATION_ID_START + i)
                        ? Savegame.CollectionStatus.Collected
                        : Savegame.CollectionStatus.Spawned; // TODO Should it always be spawned? Hopefully.
                }

                var foundPlaycoinLocations = ManualSingleton<IRandomizer>.instance.GetCheckedPlaycoinLocations();
                for (int i = 0; i < 5; i++)
                {
                    SavegameManager.instance.savegame.worlds[Constants.WorldIndexOrder[i]].arcadetoken = foundPlaycoinLocations.Contains(Constants.PLAYCOIN_LOCATION_ID_START + i)
                        ? Savegame.CollectionStatus.Collected
                        : Savegame.CollectionStatus.Spawned; // TODO Should it always be spawned? Hopefully.
                }

                var foundHealthExtenderLocations = ManualSingleton<IRandomizer>.instance.GetCheckedHealthExtenderLocations();
                SavegameManager.instance.savegame.worlds[1].healthextendertoken = foundHealthExtenderLocations.Contains(Constants.HEALTH_EXTENDER_LOCATION_ID_START)
                        ? Savegame.CollectionStatus.Collected
                        : Savegame.CollectionStatus.Spawned; // TODO Should it always be spawned? Hopefully.
                for (int i = 0; i < 5; i++)
                {
                    SavegameManager.instance.savegame.worlds[Constants.WorldIndexOrder[i]].healthextendertoken = foundHealthExtenderLocations.Contains(Constants.HEALTH_EXTENDER_LOCATION_ID_START + i + 1)
                        ? Savegame.CollectionStatus.Collected
                        : Savegame.CollectionStatus.Spawned; // TODO Should it always be spawned? Hopefully.
                }

                var foundEnergyExtenderLocations = ManualSingleton<IRandomizer>.instance.GetCheckedEnergyExtenderLocations();
                SavegameManager.instance.savegame.worlds[1].specialextendertoken = foundEnergyExtenderLocations.Contains(Constants.ENERGY_EXTENDER_LOCATION_ID_START)
                        ? Savegame.CollectionStatus.Collected
                        : Savegame.CollectionStatus.Spawned; // TODO Should it always be spawned? Hopefully.
                for (int i = 0; i < 5; i++)
                {
                    SavegameManager.instance.savegame.worlds[Constants.WorldIndexOrder[i]].specialextendertoken = foundEnergyExtenderLocations.Contains(Constants.ENERGY_EXTENDER_LOCATION_ID_START + i + 1)
                        ? Savegame.CollectionStatus.Collected
                        : Savegame.CollectionStatus.Spawned; // TODO Should it always be spawned? Hopefully.
                }

                // TODO Abilities

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
                SavegameManager.instance.savegame.player.arcadeTokenCount = ManualSingleton<IRandomizer>.instance.GetReceivedPlayCoins().Length;
                SavegameManager.instance.savegame.player.healthExtenderTokenCount = ManualSingleton<IRandomizer>.instance.GetReceivedHealthExtenderCount();
                SavegameManager.instance.savegame.player.specialExtenderTokenCount = ManualSingleton<IRandomizer>.instance.GetReceivedEnergyExtenderCount();
                var receivedAbilities = ManualSingleton<IRandomizer>.instance.GetReceivedAbilities();
                for (long i = Constants.ABILITY_ITEM_ID_START; i <= Constants.ABILITY_ITEM_ID_START + 14; i++)
                {
                    ManualSingleton<SavegameManager>.instance.EnableMove(PlayerMoveConverter.GetMoveFromItemId(i), receivedAbilities.Any(itemId => itemId == i), false);
                }

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
