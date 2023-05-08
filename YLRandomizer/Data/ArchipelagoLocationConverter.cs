namespace YLRandomizer.Data
{
    public class ArchipelagoLocationConverter
    {
        public const long LOCATION_ID_BASE = 52000; // TODO Actual value
        public const long PAGIES_PER_WORLD = 30; // 5 are ignored, but still need for correct calculations

        public static long GetLocationId(int worldIndex, int pagieIndex)
        {
            // World Info
            // 0 = Nothing
            // 1 = Hivory Towers
            // 2 = Tribalstack Tropics
            // 3 = Galleon Galaxy
            // 4 = Capital Cashino
            // 5 = Glitterglaze Glacier
            // 6 = Moodymaze Marsh
            // 7 = Nothing
            var pagieAddition = (worldIndex - 1) * PAGIES_PER_WORLD;
            return LOCATION_ID_BASE + pagieAddition + pagieIndex;
        }

        public static Tuple<int, int> GetPagieInfo(long locationId)
        {
            var pagieOnly = locationId - LOCATION_ID_BASE;
            return new Tuple<int, int>((int)(pagieOnly / PAGIES_PER_WORLD) + 1, (int) (pagieOnly % PAGIES_PER_WORLD));
        }
    }
}
