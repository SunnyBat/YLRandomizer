using System;

namespace YLRandomizer.GameAnalysis
{
    public class GameState
    {
        private static readonly TimeSpan OuttroTime = TimeSpan.FromSeconds(5); // 5sec = rough amount of time for outtro. More for mod logic than accuracy.
        private static bool _inLoadingScreen = false;
        private static DateTime _lastLoadingScreenOuttroTime = DateTime.MinValue;

        private static CameraManager _cameraManager;
        private static PlayerLogic _playerLogic;
        private static HudSilhouetteController _hudSilhouetteController;

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
        /// Checks to see if the player has full control of the character in
        /// any way (eg main game, arcade game, etc)
        /// </summary>
        /// <returns></returns>
        public static bool HasFullControlOfCharacter()
        {
            return HasFullControlOfMainGameCharacter() || HasFullControlOfArcadeCharacter();
        }

        /// <summary>
        /// This attempts to check to make sure the player has full control, eg can walk around,
        /// attack, etc.
        /// </summary>
        /// <returns></returns>
        public static bool HasFullControlOfMainGameCharacter()
        {
            if (!IsInGame() || PauseController.instance.IsPaused)
            {
                return false;
            }

            if (_cameraManager == null || !_cameraManager.isActiveAndEnabled)
            {
                _cameraManager = UnityEngine.Object.FindObjectOfType<CameraManager>();
            }
            if (_playerLogic == null || !_playerLogic.isActiveAndEnabled)
            {
                _playerLogic = UnityEngine.Object.FindObjectOfType<PlayerLogic>();
            }
            if (_hudSilhouetteController == null || !_hudSilhouetteController.isActiveAndEnabled)
            {
                _hudSilhouetteController = UnityEngine.Object.FindObjectOfType<HudSilhouetteController>();
            }
            return (_cameraManager?.IsPlayerCamera(_cameraManager?.GetCurrentCamera()) ?? false)
                && (_playerLogic?.IsMovementEnabled() ?? false)
                && (!_hudSilhouetteController?.IsAnimating() ?? true);
        }

        /// <summary>
        /// This attempts to check to make sure the player has full control within a Rextro's
        /// Arcade game.
        /// </summary>
        /// <returns></returns>
        public static bool HasFullControlOfArcadeCharacter()
        {
            if (!IsInGame() || PauseController.instance.IsPaused)
            {
                return false;
            }

            return ArcadeGameController.Instance?.CurrentGameState == ArcadeGameController.ArcadeGameState.InPlay;
        }
    }
}
