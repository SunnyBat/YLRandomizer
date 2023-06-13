namespace YLRandomizer.Data
{
    public class ArchipelagoLocationConverter
    {
        private static readonly int[] _worldIndexTranslations = new int[] { -1, 0, 1, 5, 4, 2, 3, -1 };

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
            var translatedWorldIndex = _worldIndexTranslations[worldIndex];
            var pagieAddition = translatedWorldIndex * Constants.PAGIES_PER_WORLD;
            return Constants.LOCATION_ID_BASE + pagieAddition + pagieIndex;
        }

        public static Tuple<int, int> GetPagieInfo(long locationId)
        {
            var pagieOnly = locationId - Constants.LOCATION_ID_BASE;
            return new Tuple<int, int>((int)(pagieOnly / Constants.PAGIES_PER_WORLD) + 1, (int) (pagieOnly % Constants.PAGIES_PER_WORLD));
        }
    }
}
