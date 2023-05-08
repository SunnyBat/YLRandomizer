using YLRandomizer.Randomizer;

namespace YLRandomizer.Data
{
    public class ArchipelagoDataHandler
    {
        public static void HookUpEventSubscribers()
        {
            ManualSingleton<IRandomizer>.instance.MessageReceived += (message) =>
            {
                ManualSingleton<YLRandomizer.Logging.ILogger>.instance.Info(message);
                ManualSingleton<IUserMessages>.instance.AddMessage(message);
            };
            ManualSingleton<IRandomizer>.instance.ItemReceived += (itemId) =>
            {
                ManualSingleton<YLRandomizer.Logging.ILogger>.instance.Info("Pagie received: " + itemId);
                SavegameManager.instance.savegame.player.unspentPagies++;
                HudController.instance.UpdatePagieCounter(false); // TODO Show or not show?
            };
            ManualSingleton<IRandomizer>.instance.LocationReceived += (locationId) =>
            {
                ManualSingleton<YLRandomizer.Logging.ILogger>.instance.Info("Location marked as checked: " + locationId);
                var pagieData = ArchipelagoLocationConverter.GetPagieInfo(locationId);
                SavegameManager.instance.savegame.worlds[pagieData.Item1].pagies[pagieData.Item2] = Savegame.CollectionStatus.Collected;
            };
        }
    }
}
