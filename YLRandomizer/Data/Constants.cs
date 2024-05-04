namespace YLRandomizer.Data
{
    public class Constants
    {
        /// <summary>
        /// The default amount of pagies required to beat Capital B. This must be the correct default value for the game,
        /// otherwise some triggers will not work properly.
        /// </summary>
        public const int DEFAULT_REQUIRED_PAGIES_FOR_CAPITAL_B = 100;
        public const long ID_BASE = 625490000;
        public const long LOCATION_ID_BASE = ID_BASE + 500;
        public const long PAGIES_PER_WORLD = 30; // 5 (10 in Hivory Towers) are ignored, but still need for correct calculations
        public const long MOLLYCOOL_LOCATION_ID_START = ID_BASE + 1020; // 5 total
        public const long PLAYCOIN_LOCATION_ID_START = ID_BASE + 1030; // 5 total
        public const long HEALTH_EXTENDER_LOCATION_ID_START = ID_BASE + 1000; // 6 total
        public const long ENERGY_EXTENDER_LOCATION_ID_START = ID_BASE + 1010; // 6 total
        public const long TROWSER_FREE_ABILITY_LOCATION_ID_START = ID_BASE + 1500;
        public const long TROWSER_PAID_ABILITY_LOCATION_ID_START = ID_BASE + 1510;
        public const long PAGIE_ITEM_ID = ID_BASE;
        public const long HEALTH_EXTENDER_ITEM_ID = ID_BASE + 1;
        public const long ENERGY_EXTENDER_ITEM_ID = ID_BASE + 2;
        public const long MOLLYCOOL_ITEM_ID_START = ID_BASE + 10; // 5 total
        public const long PLAYCOIN_ITEM_ID_START = ID_BASE + 20; // 5 total
        public const long ABILITY_ITEM_ID_START = ID_BASE + 50;
        public const string CONFIGURATION_NAME_CAPITAL_B_PAGIE_COUNT = "CapitalBPagieCount";
        public const string CONFIGURATION_NAME_DISABLE_QUIZZES = "DisableQuizzes";
        public const string CONFIGURATION_NAME_DEATHLINK = "DeathLink";

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
