namespace YLRandomizer.Data
{
    public class Constants
    {
        public const bool MOD_DEBUG = false;
        public const long LOCATION_ID_BASE = 50500;
        public const long PAGIES_PER_WORLD = 30; // 5 (10 in Hivory Towers) are ignored, but still need for correct calculations
        public const int MOLLYCOOL_LOCATION_ID_START = 51020; // 5 total
        public const int PLAYCOIN_LOCATION_ID_START = 51030; // 5 total
        public const int HEALTH_EXTENDER_LOCATION_ID_START = 51000; // 6 total
        public const int ENERGY_EXTENDER_LOCATION_ID_START = 51010; // 6 total
        public const int PAGIE_ITEM_ID = 50000;
        public const int HEALTH_EXTENDER_ITEM_ID = 50001;
        public const int ENERGY_EXTENDER_ITEM_ID = 50002;
        public const int MOLLYCOOL_ITEM_ID_START = 50010; // 5 total
        public const int PLAYCOIN_ITEM_ID_START = 50020; // 5 total

        /// <summary>
        /// The order in which book world indeces (eg excluding Hivory Towers) are ordered in SavegameManager.instance.savegame.worlds.
        /// </summary>
        public static readonly int[] WorldIndexOrder = new int[] { 2, 5, 6, 4, 3 };
        /// <summary>
        /// Converts a world index to a logical world order number. Includes Hivory Towers as 0, Tribalstack Tropics as 1, Glacier as 2, etc
        /// </summary>
        public static readonly int[] WorldIndexToLogicalIndexTranslations = new int[] { -1, 0, 1, 5, 4, 2, 3, -1 };

        /// <summary>
        /// The names of all of the world unlock event names. These correspond to the same worlds referenced in <see cref="WorldIndexOrder"/>,
        /// but there are two entries in here for every one entry in there. The first entry of each set of two is the Unlock event, and the
        /// second entry is the Expand event.
        /// </summary>
        public static readonly string[] UnlockEventNames = new string[] {
            "UnlockJungle", "ExpandJungle",
            "UnlockGlacier", "ExpandGlacier",
            "UnlockSwamp", "ExpandSwamp",
            "UnlockCasino", "ExpandCasino",
            "UnlockSpace", "ExpandSpace"
        };
    }
}
