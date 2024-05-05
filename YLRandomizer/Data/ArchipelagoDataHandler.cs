using System;
using System.Collections.Generic;
using System.Linq;
using YLRandomizer.GameAnalysis;
using YLRandomizer.Logging;
using YLRandomizer.Randomizer;
using static PlayerMoves;

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
                            HudController.instance.UpdatePagieCounter(false);
                        }
                    }
                    else if (itemId >= Constants.MOLLYCOOL_ITEM_ID_START && itemId <= Constants.MOLLYCOOL_ITEM_ID_START + 4)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("Mollycool received: " + itemId);
                    }
                    else if (itemId >= Constants.PLAYCOIN_ITEM_ID_START && itemId <= Constants.PLAYCOIN_ITEM_ID_START + 4)
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("Playcoin received: " + itemId);
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
                    else if (itemId >= Constants.ABILITY_ITEM_ID_START && itemId <= Constants.ABILITY_ITEM_ID_START + 14)
                    {
                        var move = ItemAndLocationIdConverter.GetMoveFromItemId(itemId);
                        ManualSingleton<Logging.ILogger>.instance.Info("Move received: " + move);
                        SavegameManager.instance.EnableMove(move, true, false);
                        var playerMoves = UnityEngine.GameObject.FindObjectOfType<PlayerMoves>();
                        if (playerMoves?.moveDictionary.TryGetValue(move, out var moveToEnable) ?? false)
                        {
                            ManualSingleton<Logging.ILogger>.instance.Info("Enabling move: " + moveToEnable.DevName);
                            moveToEnable.EnableInGame();
                            if (move == Moves.BasicAttack && playerMoves.moveDictionary.TryGetValue(Moves.BasicAttackAir, out var moveToEnable2))
                            {
                                ManualSingleton<Logging.ILogger>.instance.Info("Enabling move 2: " + moveToEnable2.DevName);
                                SavegameManager.instance.EnableMove(Moves.BasicAttackAir, true, false);
                                moveToEnable2.EnableInGame();
                            }
                        }
                    }
                    else
                    {
                        ManualSingleton<Logging.ILogger>.instance.Warning("Unknown item received: " + itemId);
                    }
                }
            };
            ManualSingleton<IRandomizer>.instance.ReadyToUse += () =>
            {
                UpdateCurrentGameStateToAP();
            };
            ManualSingleton<IRandomizer>.instance.DeathLinkReceived += (Action clearDeathLink) =>
            {
                var playerHealth = UnityEngine.GameObject.FindObjectOfType<PlayerHealth>();
                if (!playerHealth.Invulnerable)
                {
                    clearDeathLink();
                    playerHealth.SubtractHealth(playerHealth.CurrentHealth, true);
                }
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

                // TODO Send unlocked ability locations (requires restructuring of SavegameManager usage WRT abilities)

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
                var playerMoves = UnityEngine.GameObject.FindObjectOfType<PlayerMoves>();
                for (long i = Constants.ABILITY_ITEM_ID_START; i <= Constants.ABILITY_ITEM_ID_START + 14; i++)
                {
                    var hasReceivedMove = receivedAbilities.Any(itemId => itemId == i);
                    var move = ItemAndLocationIdConverter.GetMoveFromItemId(i);
                    SavegameManager.instance.EnableMove(move, hasReceivedMove, false);
                    if (hasReceivedMove && playerMoves != null && (playerMoves?.moveDictionary?.TryGetValue(move, out var moveToEnable) ?? false))
                    {
                        ManualSingleton<Logging.ILogger>.instance.Info("Enabling move: " + moveToEnable.DevName);
                        moveToEnable.EnableInGame();
                        if (move == Moves.BasicAttack && playerMoves.moveDictionary.TryGetValue(Moves.BasicAttackAir, out var moveToEnable2))
                        {
                            ManualSingleton<Logging.ILogger>.instance.Info("Enabling move 2: " + moveToEnable2.DevName);
                            SavegameManager.instance.EnableMove(Moves.BasicAttackAir, true, false);
                            moveToEnable2.EnableInGame();
                        }
                    }
                }
                ManualSingleton<Logging.ILogger>.instance.Debug("ABILITIES DONE");

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

        public static bool TryGetSlotData<T>(string key, out T obj)
        {
            var dictionaryToRead = ManualSingleton<IRandomizer>.instance.GetConfigurationOptions();

            if (dictionaryToRead != null && dictionaryToRead.TryGetValue(key, out object outVal))
            {
                try
                {
                    obj = (T)outVal;
                    return true;
                }
                catch (Exception e)
                {
                    ManualSingleton<ILogger>.instance.Error(e.Message);
                    ManualSingleton<ILogger>.instance.Debug("Expected type: " + typeof(T));
                    ManualSingleton<ILogger>.instance.Debug("Actual type: " + outVal.GetType());
                }
            }

            obj = default(T);
            return false;
        }
    }
}
