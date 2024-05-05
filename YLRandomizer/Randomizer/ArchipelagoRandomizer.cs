using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YLRandomizer.Data;
using YLRandomizer.Logging;

namespace YLRandomizer.Randomizer
{
    // TODO Make Archipelago use slot indeces 3+
    // This will:
    // - Not overwrite users' current saves
    // - Allow an infinite amount of slots
    // - Auto-recall saves so users don't accidentally use the wrong one
    // Use _session.RoomState.Seed to distinguish between seeds. Use a file to map
    // the seed to the slot number. Always increment the slot number up.
    // Slot number has some hardcoded values (probably just the SavegameManager save
    // array), we'll need to overwrite this ASAP so it supports an arbitrarily large
    // amount of saves (probably go with 10k or something). Have a backup in case we
    // hit this number that prints a message letting the user know the need to clean
    // up their AP saves.
    // Looks like there's some additional logic that would need to be patched, maybe
    // for another time :(
    public class ArchipelagoRandomizer : IRandomizer
    {
        public event ItemReceivedCallback ItemReceived;
        public event LocationReceivedCallback LocationReceived;
        public event MessageReceivedCallback MessageReceived;
        public event DeathLinkReceivedCallback DeathLinkReceived;
        public event Action ReadyToUse;

        /// <summary>
        /// The time to wait between sending things (eg location checks) to Archipelago.
        /// </summary>
        private readonly TimeSpan TIME_BETWEEN_AP_PROCESSES = TimeSpan.FromMilliseconds(500);
        /// <summary>
        /// The time to wait between connection attempts. This is most important when either
        /// reconnecting OR when a previous connection attempt fails.
        /// </summary>
        private readonly TimeSpan TIME_BETWEEN_AP_CONNECTION_ATTEMPTS = TimeSpan.FromSeconds(5);
        private readonly int MAX_CONNECTION_ATTEMPTS = 3;
        private readonly TimeSpan MINIMUM_TIME_BETWEEN_DEATHLINK_INTERACTIONS = TimeSpan.FromSeconds(5);

        private bool _killed = false;
        private bool _sentReadyToUse = false;
        private bool _deathLinkReceived = false;
        private readonly object _threadLock = new object();
        private readonly object _sessionLock = new object();
        private int _sequentialConnectionAttempts = 0;
        private DateTime _lastDeathLinkInteractionTime = DateTime.MinValue;
        private ArchipelagoSession _session;
        private DeathLinkService _deathLink;
        private ArchipelagoClientState _currentGameState = ArchipelagoClientState.ClientUnknown;
        private ArchipelagoClientState _lastSentGameState = ArchipelagoClientState.ClientUnknown;
        private readonly Queue<NetworkItem> _itemReceivedQueue = new Queue<NetworkItem>();
        private readonly Queue<long> _locationReceivedQueue = new Queue<long>();
        private readonly Queue<string> _messageReceivedQueue = new Queue<string>();
        private readonly Queue<long> _locationSendQueue = new Queue<long>();

        /// <summary>
        /// Creates a new ArchipelagoRandomizer. This is a multi-threaded randomizer that will never
        /// block the calling thread.
        /// </summary>
        /// <param name="address">The address to use. May contain a protocol and/or port.</param>
        public ArchipelagoRandomizer(string address, string username, string password)
        {
            lock (_sessionLock)
            {
                _session = ArchipelagoSessionFactory.CreateSession(address);
                _session.Items.ItemReceived += (itemHelper) =>
                {
                    lock (_threadLock)
                    {
                        _itemReceivedQueue.Enqueue(itemHelper.DequeueItem());
                    }
                };
                _session.Locations.CheckedLocationsUpdated += (locHelper) =>
                {
                    lock (_threadLock)
                    {
                        locHelper.Do(locId => _locationReceivedQueue.Enqueue(locId));
                    }
                };
                _session.MessageLog.OnMessageReceived += (message) =>
                {
                    lock (_threadLock)
                    {
                        _messageReceivedQueue.Enqueue(message.ToString());
                    }
                };
            }
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        bool killed;
                        bool connected;
                        lock (_threadLock)
                        {
                            killed = _killed;
                        }
                        lock (_sessionLock)
                        {
                            connected = _session.Socket.Connected;
                        }
                        if (killed)
                        {
                            try
                            {
                                _apTick(); // One last process, if it fails we've already disconnected anyways
                                lock (_sessionLock)
                                {
                                    _session.Socket.Disconnect();
                                }
                            }
                            catch { }
                            // Session will hang around accumulating locations if left un-reconnected, but
                            // it doesn't /really/ matter since we re-send all locations on reconnection
                            // anyways.
                            // Letting it hang around also gives us a chance to flush out any last-minute
                            // messages (eg failed to connect messages)
                            return; // Kill thread
                        }
                        else if (connected)
                        {
                            DateTime startTime = DateTime.Now;
                            _apTick(); // Thread-safe
                            var timeToSleep = TIME_BETWEEN_AP_PROCESSES - (DateTime.Now - startTime);
                            if (timeToSleep > TimeSpan.Zero)
                            {
                                Thread.Sleep(timeToSleep);
                            }
                        }
                        else
                        {
                            try
                            {
                                lock (_threadLock)
                                {
                                    if (_sequentialConnectionAttempts < MAX_CONNECTION_ATTEMPTS)
                                    {
                                        _sequentialConnectionAttempts++;
                                        _messageReceivedQueue.Enqueue("Attempting to connect to Archipelago...");
                                    }
                                    else
                                    {
                                        _messageReceivedQueue.Enqueue("FATAL: Max connection attempts reached.");
                                        _killed = true;
                                        _lastSentGameState = ArchipelagoClientState.ClientUnknown;
                                        continue;
                                    }
                                }
                                LoginResult result;
                                lock (_sessionLock)
                                {
                                    result = _session.TryConnectAndLogin("Yooka-Laylee", username, Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags.AllItems, new Version(1, 0, 0), password: password);
                                }
                                if (result.Successful)
                                {
                                    lock (_threadLock)
                                    {
                                        _sequentialConnectionAttempts = 0;
                                        _messageReceivedQueue.Enqueue("Connected to Archipelago server!");
                                    }
                                    if (GetConfigurationOptions().TryGetValue(Constants.CONFIGURATION_NAME_DEATHLINK, out object isDeathLinkEnabled) && ((bool)isDeathLinkEnabled))
                                    {
                                        try
                                        {
                                            var deathLinkService = _session.CreateDeathLinkService();
                                            lock (_threadLock)
                                            {
                                                _deathLink = deathLinkService;
                                                _deathLink.OnDeathLinkReceived += (DeathLink deathLink) =>
                                                {
                                                    lock (_threadLock)
                                                    {
                                                        _deathLinkReceived = true;
                                                    }
                                                };
                                                _deathLink.EnableDeathLink();
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            _messageReceivedQueue.Enqueue("ERROR: Unable to set up DeathLink: " + e.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    var failedRes = result as LoginFailure;
                                    lock (_threadLock)
                                    {
                                        _messageReceivedQueue.Enqueue("ERROR: Failed to connect to Archipelago server for the following reasons:");
                                        failedRes.Errors.Do(err => _messageReceivedQueue.Enqueue("- " + err));
                                        failedRes.ErrorCodes.Do(eC => _messageReceivedQueue.Enqueue("= " + eC));
                                        lock (_sessionLock)
                                        {
                                            connected = _session.Socket.Connected;
                                        }
                                        if (connected)
                                        {
                                            if (failedRes.ErrorCodes.Length > 0)
                                            {
                                                _sequentialConnectionAttempts = MAX_CONNECTION_ATTEMPTS; // Got error response from server, don't retry
                                            }
                                            lock (_sessionLock)
                                            {
                                                _session.Socket.Disconnect();
                                            }
                                        }
                                    }
                                    if (_sequentialConnectionAttempts < MAX_CONNECTION_ATTEMPTS)
                                    {
                                        Thread.Sleep(TIME_BETWEEN_AP_CONNECTION_ATTEMPTS);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                lock (_threadLock)
                                {
                                    _messageReceivedQueue.Enqueue("ERROR: Couldn't connect and log in: " + e.Message);
                                }
                                if (_sequentialConnectionAttempts < MAX_CONNECTION_ATTEMPTS)
                                {
                                    Thread.Sleep(TIME_BETWEEN_AP_CONNECTION_ATTEMPTS);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ManualSingleton<ILogger>.instance.Error(e.Message);
                        ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    }
                }
            }).Start();
        }

        public bool IsConfigured()
        {
            lock (_threadLock)
            {
                return !_killed; // Configuration required before randomizer is created
            }
        }

        public bool IsReadyToUse()
        {
            lock (_threadLock)
            {
                return !_killed && _lastSentGameState != ArchipelagoClientState.ClientUnknown; // ClientUnknown is proxy for not connected/authenticated
            }
        }

        public Dictionary<string, object> GetConfigurationOptions()
        {
            lock (_threadLock)
            {
                return this.IsConfigured() ? this._session.DataStorage.GetSlotData() ?? new Dictionary<string, object>() : new Dictionary<string, object>();
            }
        }

        public long[] GetAllItems()
        {
            lock (_sessionLock)
            {
                if (_session.Socket.Connected)
                {
                    // Don't need to lock on this specifically because:
                    // - _session is never changed
                    // - Items is never changed
                    // - AllItemsReceived is thread-safe
                    return _session.Items.AllItemsReceived.Select(itm => itm.Item).ToArray();
                }
                else
                {
                    return new long[0];
                }
            }
        }

        public int GetReceivedPagiesCount()
        {
            return GetAllItems().Where(itemId => itemId == Constants.PAGIE_ITEM_ID).Count();
        }

        public int GetReceivedHealthExtenderCount()
        {
            return GetAllItems().Where(itemId => itemId == Constants.HEALTH_EXTENDER_ITEM_ID).Count();
        }

        public int GetReceivedEnergyExtenderCount()
        {
            return GetAllItems().Where(itemId => itemId == Constants.ENERGY_EXTENDER_ITEM_ID).Count();
        }

        public bool[] GetReceivedMollycools()
        {
            var itemsReceived = GetAllItems().Where(itemId => itemId >= Constants.MOLLYCOOL_ITEM_ID_START && itemId <= Constants.MOLLYCOOL_ITEM_ID_START + 5);
            var ret = new bool[5];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = itemsReceived.Contains(Constants.MOLLYCOOL_ITEM_ID_START + i);
            }
            return ret;
        }

        public bool[] GetReceivedPlayCoins()
        {
            var itemsReceived = GetAllItems().Where(itemId => itemId >= Constants.PLAYCOIN_ITEM_ID_START && itemId <= Constants.PLAYCOIN_ITEM_ID_START + 5);
            var ret = new bool[5];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = itemsReceived.Contains(Constants.PLAYCOIN_ITEM_ID_START + i);
            }
            return ret;
        }

        public long[] GetReceivedAbilities()
        {
            return GetAllItems().Where(itemId => itemId >= Constants.ABILITY_ITEM_ID_START && itemId <= Constants.ABILITY_ITEM_ID_START + 14).ToArray();
        }

        public long[] GetAllCheckedLocations()
        {
            lock (_sessionLock)
            {
                if (_session.Socket.Connected)
                {
                    // Don't need to lock on this specifically because:
                    // - _session is never changed
                    // - Locations is never changed
                    // - AllLocationsChecked is thread-safe
                    return _session.Locations.AllLocationsChecked.ToArray();
                }
                else
                {
                    return new long[0];
                }
            }
        }

        public long[] GetCheckedPagieLocations()
        {
            return GetAllCheckedLocations().Where(locationId => locationId >= Constants.LOCATION_ID_BASE && locationId <= (Constants.LOCATION_ID_BASE + (Constants.PAGIES_PER_WORLD * 6))).ToArray();
        }

        public long[] GetCheckedMollycoolLocations()
        {
            return GetAllCheckedLocations().Where(locationId => locationId >= Constants.MOLLYCOOL_LOCATION_ID_START && locationId <= (Constants.MOLLYCOOL_LOCATION_ID_START + 5)).ToArray();
        }

        public long[] GetCheckedPlaycoinLocations()
        {
            return GetAllCheckedLocations().Where(locationId => locationId >= Constants.PLAYCOIN_LOCATION_ID_START && locationId <= (Constants.PLAYCOIN_LOCATION_ID_START + 5)).ToArray();
        }

        public long[] GetCheckedHealthExtenderLocations()
        {
            return GetAllCheckedLocations().Where(locationId => locationId >= Constants.HEALTH_EXTENDER_LOCATION_ID_START && locationId <= (Constants.HEALTH_EXTENDER_LOCATION_ID_START + 6)).ToArray();
        }

        public long[] GetCheckedEnergyExtenderLocations()
        {
            return GetAllCheckedLocations().Where(locationId => locationId >= Constants.ENERGY_EXTENDER_LOCATION_ID_START && locationId <= (Constants.ENERGY_EXTENDER_LOCATION_ID_START + 6)).ToArray();
        }

        public long[] GetCheckedAbilityLocations()
        {
            return GetAllCheckedLocations().Where(locationId => locationId >= Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START && locationId <= (Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 6)).ToArray();
        }

        public void LocationChecked(params long[] locationIds)
        {
            ManualSingleton<ILogger>.instance.Debug($"ArchipelagoRandomizer.LocationChecked(): {locationIds.Join()}");
            lock (_threadLock)
            {
                locationIds.Do((id) => _locationSendQueue.Enqueue(id));
            }
        }

        public void SetNotInGame()
        {
            lock (_threadLock)
            {
                _currentGameState = ArchipelagoClientState.ClientReady;
            }
        }

        public void SetInGame()
        {
            lock (_threadLock)
            {
                _currentGameState = ArchipelagoClientState.ClientPlaying;
            }
        }

        public void SendDeathLink(string message)
        {
            lock (_threadLock)
            {
                if (DateTime.Now - _lastDeathLinkInteractionTime > MINIMUM_TIME_BETWEEN_DEATHLINK_INTERACTIONS)
                {
                    _deathLink.SendDeathLink(new DeathLink(_session.Players.GetPlayerName(_session.ConnectionInfo.Slot), message));
                }
                _lastDeathLinkInteractionTime = DateTime.Now;
            }
        }

        public void SetGameCompleted()
        {
            lock (_threadLock)
            {
                _currentGameState = ArchipelagoClientState.ClientGoal;
            }
        }

        public void Tick()
        {
            lock (_threadLock)
            {
                if (_killed)
                {
                    return; // Do nothing, we're done
                }
            }

            // Process locations
            if (LocationReceived != null)
            {
                long[] locations;
                lock (_threadLock)
                {
                    locations = _locationReceivedQueue.ToArray();
                    _locationReceivedQueue.Clear();
                }
                locations.Do(id => {
                    try
                    {
                        LocationReceived(id);
                    }
                    catch (Exception e)
                    {
                        lock (_threadLock)
                        {
                            _messageReceivedQueue.Enqueue($"ERROR: Exception while processing location unlock {id}: {e.Message}");
                        }
                        ManualSingleton<ILogger>.instance.Error($"Exception while processing location unlock {id}: {e.Message}");
                        ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    }
                });
            }

            if (ItemReceived != null)
            {
                // Process items
                NetworkItem[] items;
                lock (_threadLock)
                {
                    items = _itemReceivedQueue.ToArray();
                    _itemReceivedQueue.Clear();
                }
                items.Do(item => {
                    try
                    {
                        ItemReceived(item.Item);
                    }
                    catch (Exception e)
                    {
                        lock (_threadLock)
                        {
                            _messageReceivedQueue.Enqueue($"ERROR: Exception while processing item unlock ({item.Item}, {item.Location}, {item.Player}): {e.Message}");
                        }
                        ManualSingleton<ILogger>.instance.Error($"Exception while processing item unlock ({item.Item}, {item.Location}, {item.Player}): {e.Message}");
                        ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    }
                });
            }

            if (MessageReceived != null)
            {
                // Process messages
                string[] messages;
                lock (_threadLock)
                {
                    messages = _messageReceivedQueue.ToArray();
                    _messageReceivedQueue.Clear();
                }
                messages.Do(message =>
                {
                    try
                    {
                        MessageReceived(message);
                    }
                    catch (Exception e)
                    {
                        ManualSingleton<ILogger>.instance.Error($"Exception while processing message \"{message}\": {e.Message}");
                        ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    }
                });
            }

            if (ReadyToUse != null)
            {
                bool shouldFireReadyToUse = false;
                lock (_threadLock)
                {
                    if (!_sentReadyToUse && IsReadyToUse())
                    {
                        shouldFireReadyToUse = true;
                        _sentReadyToUse = true;
                    }
                }
                if (shouldFireReadyToUse)
                {
                    try
                    {
                        ReadyToUse();
                    }
                    catch (Exception e)
                    {
                        lock (_threadLock)
                        {
                            _messageReceivedQueue.Enqueue($"ERROR: Exception while firing ready to use: {e.Message}");
                        }
                        ManualSingleton<ILogger>.instance.Error($"Exception while firing ready to use: {e.Message}");
                        ManualSingleton<ILogger>.instance.Error(e.StackTrace);
                    }
                }
            }
        }

        public void EndRandomizer()
        {
            lock (_threadLock)
            {
                _killed = true;
                _lastSentGameState = ArchipelagoClientState.ClientUnknown;
            }
        }

        private void _apTick()
        {
            long[] locationsToSend;
            ArchipelagoClientState? gameStateToSend = null;
            bool hasReceivedDeathLink;
            lock (_threadLock)
            {
                locationsToSend = _locationSendQueue.ToArray();
                _locationSendQueue.Clear();
                if (_currentGameState != _lastSentGameState)
                {
                    gameStateToSend = _currentGameState;
                    _lastSentGameState = _currentGameState;
                }
                hasReceivedDeathLink = _deathLinkReceived;
            }
            if (locationsToSend.Length > 0)
            {
                lock (_sessionLock)
                {
                    _session.Locations.CompleteLocationChecks(locationsToSend);
                }
            }
            if (gameStateToSend != null)
            {
                lock (_sessionLock)
                {
                    ManualSingleton<ILogger>.instance.Debug("Setting game state to " + gameStateToSend);
                    _session.Socket.SendPacket(new StatusUpdatePacket()
                    {
                        Status = (ArchipelagoClientState)gameStateToSend
                    });
                }
            }
            if (hasReceivedDeathLink)
            {
                _lastDeathLinkInteractionTime = DateTime.Now;
                DeathLinkReceived(_clearDeathLinkReceived);
            }
        }

        private void _clearDeathLinkReceived()
        {
            lock (_threadLock)
            {
                _deathLinkReceived = false;
                _lastDeathLinkInteractionTime = DateTime.Now;
            }
        }
    }
}
