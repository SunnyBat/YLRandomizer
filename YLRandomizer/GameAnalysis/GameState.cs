namespace YLRandomizer.GameAnalysis
{
    public class GameState
    {
        public static bool IsInGame()
        {
            // True when in world, false if in loading screen
            //return GameStatManager.instance != null;

            // True when not in loading screen (including main menu)
            //return EventManager.Instance != null;

            // True when in game (false in main menu), BUT stays true after initial load even in main menu
            return SavegameManager.instance != null;
        }
    }
}
