namespace YLRandomizer.Data
{
    public class Constants
    {
        public const long LOCATION_ID_BASE = 50500;
        public const long PAGIES_PER_WORLD = 30; // 5 (10 in Hivory Towers) are ignored, but still need for correct calculations
        public const int PAGIE_ITEM_ID = 50000;

        /// <summary>
        /// The order in which book world indeces (eg excluding Hivory Towers) are ordered in SavegameManager.instance.savegame.worlds.
        /// </summary>
        public static readonly int[] WorldIndexOrder = new int[] { 2, 5, 6, 4, 3 };

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
