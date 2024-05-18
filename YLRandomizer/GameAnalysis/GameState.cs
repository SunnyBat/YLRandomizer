using System;

namespace YLRandomizer.GameAnalysis
{
    public class GameState
    {
        private static readonly TimeSpan OuttroTime = TimeSpan.FromSeconds(5); // 5sec = rough amount of time for outtro. More for mod logic than accuracy.
        private static bool _inLoadingScreen = false;
        private static DateTime _lastLoadingScreenOuttroTime = DateTime.MinValue;

        private static PlayerLogic _playerLogic;

        /// <summary>
        /// Sets the loading screen state to finished. Important to call when loading screen finishes.
        /// </summary>
        public static void SetLoadingScreenFinished()
        {
            _inLoadingScreen = false;
            _lastLoadingScreenOuttroTime = DateTime.Now;
        }

        /// <summary>
        /// Sets the loading screen state to started. Important to call when loading screen starts.
        /// </summary>
        public static void SetLoadingScreenStarted()
        {
            _inLoadingScreen = true;
        }

        /// <summary>
        /// Checks whether we're in the loading screen or recently exited the loading screen.
        /// </summary>
        /// <returns>True if in loading screen or recently exited, false if not</returns>
        public static bool IsWithinLoadingScreenWindow()
        {
            return _inLoadingScreen || DateTime.Now - _lastLoadingScreenOuttroTime < OuttroTime;
        }

        /// <summary>
        /// Returns true if in game (regardless of scene/loading/etc), false if not.
        /// </summary>
        /// <returns>True if in game, false if not</returns>
        public static bool IsInGame()
        {
            // GameStatManager.instance != null = True when in world, false if in loading screen
            return GameStatManager.instance != null || IsWithinLoadingScreenWindow();

            // True when not in loading screen (including main menu)
            //return EventManager.Instance != null;

            // True when in game (false in main menu), BUT stays true after initial load even in main menu
            //return SavegameManager.instance != null;
        }

        /// <summary>
        /// This attempts to check to make sure the player has full control, eg can walk around,
        /// attack, etc.
        /// </summary>
        /// <returns></returns>
        public static bool HasFullControlOfCharacter()
        {
            if (PauseController.instance.IsPaused)
            {
                return false;
            }

            if (_playerLogic == null || !_playerLogic.isActiveAndEnabled)
            {
                _playerLogic = UnityEngine.Object.FindObjectOfType<PlayerLogic>();
            }
            return _playerLogic?.IsMovementEnabled() ?? false;
        }
    }
}
