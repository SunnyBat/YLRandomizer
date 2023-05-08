using UnityEngine;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Scripts
{
    public class ArchipelagoUI : MonoBehaviour
    {
        string _hostName = string.Empty;
        string _username = string.Empty;
        string _password = string.Empty;

        void OnGUI()
        {
            // Relevant class: FrontendSavegameScreenController -- OnSubmit() will call LoadSlot(), which loads the savegame of interest
            if (ManualSingleton<IRandomizer>.instance == null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Yoinked directly from Subnautica, thanks Berserker
                // https://github.com/Berserker66/ArchipelagoSubnauticaModSrc/blob/master/mod/Archipelago.cs
                GUI.Label(new Rect(16, 36, 150, 20), "Host: ");
                GUI.Label(new Rect(16, 56, 150, 20), "PlayerName: ");
                GUI.Label(new Rect(16, 76, 150, 20), "Password: ");
                bool submit = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
                _hostName = GUI.TextField(new Rect(150 + 16 + 8, 36, 150, 20), _hostName);
                _username = GUI.TextField(new Rect(150 + 16 + 8, 56, 150, 20), _username);
                _password = GUI.TextField(new Rect(150 + 16 + 8, 76, 150, 20), _password);
                if (submit && Event.current.type == EventType.KeyDown)
                {
                    // The text fields have not consumed the event, which means they were not focused.
                    submit = false;
                }
                if ((GUI.Button(new Rect(16, 96, 100, 20), "Connect") || submit) && !string.IsNullOrEmpty(_hostName) && !string.IsNullOrEmpty(_username))
                {
                    ManualSingleton<IRandomizer>.instance = new ArchipelagoRandomizer(_hostName, _username, _password);
                }
            }
            else
            {
                GUI.Label(new Rect(16, 36, 150, 20), "Archipelago configured.");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
