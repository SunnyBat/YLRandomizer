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
        /// Gets all items that have been received. This does NOT remove duplicates.
        /// </summary>
        /// <returns>An array of all the items that have been received.</returns>
        long[] GetAllItems();
        /// <summary>
        /// Gets all locations that have been checked. This should not contain duplicates.
        /// </summary>
        /// <returns>An array of all the locations that have been checked.</returns>
        long[] GetAllCheckedLocations();
        /// <summary>
        /// Run to unlock locations. This may or may not queue up ItemReceived and/or
        /// LocationReceived checks.
        /// </summary>
        /// <param name="locationIds">The IDs of the locations to set as checked</param>
        void LocationChecked(params long[] locationIds);
        void SetNotInGame();
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
