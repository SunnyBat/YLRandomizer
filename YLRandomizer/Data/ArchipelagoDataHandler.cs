using YLRandomizer.Randomizer;

namespace YLRandomizer.Data
{
    public class ArchipelagoDataHandler
    {
        public static void HookUpEventSubscribers()
        {
            ManualSingleton<IRandomizer>.instance.MessageReceived += (message) =>
            {
                ManualSingleton<Logging.ILogger>.instance.Info(message);
                ManualSingleton<IUserMessages>.instance.AddMessage(message);
            };
            ManualSingleton<IRandomizer>.instance.ItemReceived += (itemId) =>
            {
                var playerInstance = SavegameManager.instance?.savegame?.player;
                if (playerInstance != null)
                {
                    playerInstance.unspentPagies++;
                    HudController.instance.UpdatePagieCounter(false); // TODO Show or not show?
                }
                ManualSingleton<Logging.ILogger>.instance.Info("Pagie received: " + itemId);
            };
            ManualSingleton<IRandomizer>.instance.LocationReceived += (locationId) =>
            {
                ManualSingleton<Logging.ILogger>.instance.Info("Location marked as checked: " + locationId);
                var apPagieData = ArchipelagoLocationConverter.GetPagieInfo(locationId);
                var worldPagieData = SavegameManager.instance?.savegame?.worlds?[apPagieData.Item1]?.pagies;
                if (worldPagieData != null)
                {
                    worldPagieData[apPagieData.Item2] = Savegame.CollectionStatus.Collected;
                }
            };
        }
    }
}
