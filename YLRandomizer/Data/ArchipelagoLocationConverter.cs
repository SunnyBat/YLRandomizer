namespace YLRandomizer.Data
{
    public class ArchipelagoLocationConverter
    {
        public static long GetPagieLocationId(int worldIndex, int pagieIndex)
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
            var translatedWorldIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex];
            var pagieAddition = translatedWorldIndex * Constants.PAGIES_PER_WORLD;
            return Constants.LOCATION_ID_BASE + pagieAddition + pagieIndex;
        }

        public static long GetPlaycoinLocationId(int worldIndex)
        {
            var translatedWorldIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex];
            return Constants.PLAYCOIN_LOCATION_ID_START + translatedWorldIndex - 1; // No playcoin in Hub, offset by 1
        }

        public static long GetMollycoolLocationId(int worldIndex)
        {
            var translatedWorldIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex];
            return Constants.MOLLYCOOL_LOCATION_ID_START + translatedWorldIndex - 1; // No mollycool in Hub, offset by 1
        }

        public static long GetHealthExtenderLocationId(int worldIndex)
        {
            var translatedWorldIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex];
            return Constants.HEALTH_EXTENDER_LOCATION_ID_START + translatedWorldIndex;
        }

        public static long GetEnergyExtenderLocationId(int worldIndex)
        {
            var translatedWorldIndex = Constants.WorldIndexToLogicalIndexTranslations[worldIndex];
            return Constants.ENERGY_EXTENDER_LOCATION_ID_START + translatedWorldIndex;
        }

        public static Tuple<int, int> GetPagieInfo(long locationId)
        {
            var pagieOnly = locationId - Constants.LOCATION_ID_BASE;
            return new Tuple<int, int>((int)(pagieOnly / Constants.PAGIES_PER_WORLD) + 1, (int) (pagieOnly % Constants.PAGIES_PER_WORLD));
        }
    }
}
