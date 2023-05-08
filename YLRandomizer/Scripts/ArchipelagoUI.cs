using UnityEngine;
using YLRandomizer.Data;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Scripts
{
    public class ArchipelagoUI : MonoBehaviour
    {
        private string _hostName = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;

        void OnGUI()
        {
            if (ManualSingleton<IRandomizer>.instance == null)
            {
                _drawShadedRectangle(new Rect(10, 30, 320, 86));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Yoinked directly from Subnautica randomizer, thanks Berserker
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
                    ManualSingleton<IRandomizer>.instance = new ArchipelagoRandomizer(_hostName, _username, _password); // Trigger connection, setup will be after this
                    ArchipelagoDataHandler.HookUpEventSubscribers();
                }
                _printMessages(122);
            }
            else
            {
                _drawShadedRectangle(new Rect(10, 30, 320, 26));
                GUI.Label(new Rect(16, 36, 900, 20), "Archipelago configured.");
                _printMessages(62);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void _printMessages(long yStart)
        {
            var allMessages = ManualSingleton<IUserMessages>.instance.GetMessages();
            _drawShadedRectangle(new Rect(10, yStart - 6, 320, (20 * allMessages.Length) + 6));
            for (int i = 0; i < allMessages.Length; i++)
            {
                GUI.Label(new Rect(16, yStart + (20 * i), 900, 20), allMessages[i]);
            }
        }

        private void _drawShadedRectangle(Rect rect)
        {
            Color startingColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = startingColor;
        }
    }
}
