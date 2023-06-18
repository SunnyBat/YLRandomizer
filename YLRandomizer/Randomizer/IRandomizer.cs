using System;

namespace YLRandomizer.Randomizer
{
    /// <summary>
    /// A randomizer. Note that there must be AT MOST ONE active randomizer at a time.
    /// Before creating a new randomizer, the old one must be ended via
    /// <see cref="EndRandomizer"/>.
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// Checks whether the randomizer is configured. When true, this means that the
        /// randomizer has been set up with a configuration. Remote randomizers may change this
        /// to false if the remote service determines the configuration is invalid.
        /// </summary>
        /// <returns>True if configured, false if not</returns>
        bool IsConfigured();
        /// <summary>
        /// Checks whether the randomizer is fully ready to use. When true, this means that the
        /// randomizer is fully set up and will work as intended. Remote randomizers may change
        /// this if the status of the remote service or connection to the remote service is
        /// changed or otherwise invalidated.
        /// </summary>
        /// <returns>True if ready to use, false if not</returns>
        bool IsReadyToUse();
        /// <summary>
        /// Gets all items that have been received. This does NOT remove duplicates.
        /// </summary>
        /// <returns>An array of all the items that have been received</returns>
        long[] GetAllItems();
        /// <summary>
        /// Gets all locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the locations that have been checked</returns>
        long[] GetAllCheckedLocations();
        /// <summary>
        /// Gets the pagie locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the pagie locations that have been checked</returns>
        long[] GetCheckedPagieLocations();
        /// <summary>
        /// Gets the mollycool locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the mollycool locations that have been checked</returns>
        long[] GetCheckedMollycoolLocations();
        /// <summary>
        /// Gets the playcoin locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the playcoin locations that have been checked</returns>
        long[] GetCheckedPlaycoinLocations();
        /// <summary>
        /// Gets the health extender locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the health extender locations that have been checked</returns>
        long[] GetCheckedHealthExtenderLocations();
        /// <summary>
        /// Gets the energy extender locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the energy extender locations that have been checked</returns>
        long[] GetCheckedEnergyExtenderLocations();
        /// <summary>
        /// Gets the amount of pagies received.
        /// </summary>
        /// <returns>The amount of pagies received</returns>
        int GetReceivedPagiesCount();
        /// <summary>
        /// Gets the amount of health exteners received.
        /// </summary>
        /// <returns>The amount of health extenders received</returns>
        int GetReceivedHealthExtenderCount();
        /// <summary>
        /// Gets the amount of energy extenders received.
        /// </summary>
        /// <returns>The amount of energy extenders received</returns>
        int GetReceivedEnergyExtenderCount();
        /// <summary>
        /// Gets whether or not the mollycool for each world has been received. This
        /// is guaranteed to return an array with exactly 5 elements. The array is
        /// sorted in logical vanilla world order.
        /// </summary>
        /// <returns>The mollycool status for each world</returns>
        bool[] GetReceivedMollycools();
        /// <summary>
        /// Gets whether or not the playcoin for each world has been received. This
        /// is guaranteed to return an array with exactly 5 elements. The array is
        /// sorted in logical vanilla world order.
        /// </summary>
        /// <returns>The playcoin status for each world</returns>
        bool[] GetReceivedPlayCoins();
        /// <summary>
        /// Run to unlock locations. This may or may not queue up ItemReceived and/or
        /// LocationReceived checks.
        /// </summary>
        /// <param name="locationIds">The IDs of the locations to set as checked</param>
        void LocationChecked(params long[] locationIds);
        /// <summary>
        /// Sets the state of the randomizer to not in game.
        /// </summary>
        void SetNotInGame();
        /// <summary>
        /// Sets the state of the randomizer to in game.
        /// </summary>
        void SetInGame();
        /// <summary>
        /// Run to complete the game.
        /// </summary>
        void SetGameCompleted();
        /// <summary>
        /// Subscribe to item received events. These are items that should be added to this
        /// game's unlocked item list. Note that this will send ALL items received exactly
        /// once. This means that when first loading a randomizer, ALL the items will be sent
        /// through this event, even if they were sent in a previous session.
        /// This will be processed via <see cref="Tick"/>.
        /// </summary>
        event ItemReceivedCallback ItemReceived;
        /// <summary>
        /// Subscribe to location received events. These are locations that have already been
        /// unlocked by this game, and should be set as such. Note that it IS possible to get
        /// duplicates here, and duplicates should be checked for via the index.
        /// This will be processed via <see cref="Tick"/>.
        /// </summary>
        event LocationReceivedCallback LocationReceived;
        /// <summary>
        /// Subscribe to message received events. These are messages that should be displayed
        /// to the user, and may be anything from informational messages about items received
        /// to error messages. All messages should be displayed to the user, even if the
        /// message contents are identical.
        /// </summary>
        event MessageReceivedCallback MessageReceived;
        /// <summary>
        /// Fires when the randomizer is ready to play. This corresponds with
        /// <see cref="IsReadyToUse"/> flipping from false to true.
        /// </summary>
        event Action ReadyToUse;
        /// <summary>
        /// Handles local item processing. Be sure to call this on the thread you want to use
        /// for receiving items and locations. This is thread-safe for the specific locations
        /// and items being processed, however the event processor is still responsible for
        /// safely processing the data.
        /// </summary>
        void Tick();
        /// <summary>
        /// Ends the randomizer session. This MUST be called BEFORE creating a new Randomizer.
        /// Once called, this kills the randomizer, and it cannot be revived. You must create
        /// a new Randomizer instance to start a new one.
        /// </summary>
        void EndRandomizer();
    }

    /// <summary>
    /// A callback for receiving an item.
    /// </summary>
    /// <param name="itemId">The ID of the item received.</param>
    public delegate void ItemReceivedCallback(long itemId);
    /// <summary>
    /// A callback for receiving an already-checked location.
    /// </summary>
    /// <param name="locationId">The ID of the location already checked.</param>
    public delegate void LocationReceivedCallback(long locationId);
    /// <summary>
    /// A callback for receiving messages.
    /// </summary>
    /// <param name="message"></param>
    public delegate void MessageReceivedCallback(string message);
}
