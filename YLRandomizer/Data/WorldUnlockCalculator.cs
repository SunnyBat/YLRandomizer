namespace YLRandomizer.Data
{
    public class WorldUnlockCalculator
    {
        public static int getCostForWorldUnlock(int savegameManagerWorldId)
        {
            switch (savegameManagerWorldId)
            {
                case 2:
                    return 1;
                case 3:
                    return 12;
                case 4:
                    return 10;
                case 5:
                    return 3;
                case 6:
                    return 7;
                default:
                    return 0;
            }
        }

        public static int getCostForWorldExpand(int savegameManagerWorldId)
        {
            switch (savegameManagerWorldId)
            {
                case 2:
                    return 3;
                case 3:
                    return 15;
                case 4:
                    return 11;
                case 5:
                    return 5;
                case 6:
                    return 8;
                default:
                    return 0;
            }
        }
    }
}
