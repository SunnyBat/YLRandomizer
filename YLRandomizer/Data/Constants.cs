namespace YLRandomizer.Data
{
    public class Constants
    {
        /// <summary>
        /// The order in which book world indeces (eg excluding Hivory Towers) are ordered in SavegameManager.instance.savegame.worlds.
        /// </summary>
        public static readonly int[] WorldIndexOrder = new int[] { 2, 5, 6, 4, 3 };

        /// <summary>
        /// The names of all of the world unlock event names. These correspond to the same worlds referenced in <see cref="WorldIndexOrder"/>.
        /// </summary>
        public static readonly string[] UnlockEventNames = new string[] { "UnlockTribalstack", "UnlockGlacier", "UnlockMarsh", "UnlockCasino", "UnlockGalleon" };
    }
}
