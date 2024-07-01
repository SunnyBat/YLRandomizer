using System;
using UnityEngine;
using YLRandomizer.Data;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Scripts
{
    public class ArchipelagoUI : MonoBehaviour
    {
        private const int SPACE_BETWEEN_GUI_ROWS = 8;
        private const int SPACE_BETWEEN_GUI_ELEMENTS_IN_ROW = 8;
        private const int DEFAULT_TEXT_SIZE = 12;
        private const int BACKGROUND_RECT_X_COORD = 10;
        private const int BACKGROUND_RECT_Y_COORD = 30;
        private const int BACKGROUND_RECT_BASE_WIDTH = 320;
        private const int BACKGROUND_RECT_PADDING = 6;
        private string _hostName = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private int _fontSize = DEFAULT_TEXT_SIZE;

        void OnGUI()
        {
            GUIStyle labelFontStyle = new GUIStyle(GUI.skin.label);
            GUIStyle buttonFontStyle = new GUIStyle(GUI.skin.button);
            GUIStyle inputFontStyle = new GUIStyle(GUI.skin.textField);
            _setFontSize(labelFontStyle, buttonFontStyle, inputFontStyle);
            labelFontStyle.normal.textColor = Color.white;
            var calculatedRectWidth = (int)Math.Round(BACKGROUND_RECT_BASE_WIDTH * ((double)_fontSize / DEFAULT_TEXT_SIZE));
            var calculatedHeightForEachElement = _fontSize + SPACE_BETWEEN_GUI_ROWS;
            _drawShadedRectangle(new Rect(BACKGROUND_RECT_X_COORD, BACKGROUND_RECT_Y_COORD, calculatedRectWidth, BACKGROUND_RECT_PADDING * 2 + calculatedHeightForEachElement));
            var currentXCoordinate = BACKGROUND_RECT_X_COORD + BACKGROUND_RECT_PADDING;
            var currentYCoordinate = BACKGROUND_RECT_Y_COORD + BACKGROUND_RECT_PADDING;
            var inputFieldXCoordinateOffset = calculatedRectWidth / 2;
            GUI.Label(new Rect(currentXCoordinate, currentYCoordinate, inputFieldXCoordinateOffset, calculatedHeightForEachElement), "YLRandomizer font size: ", labelFontStyle);
            if (GUI.Button(new Rect(currentXCoordinate + inputFieldXCoordinateOffset, currentYCoordinate, _fontSize * 2, calculatedHeightForEachElement), "-", buttonFontStyle))
            {
                _fontSize--;
            }
            if (GUI.Button(new Rect(currentXCoordinate + inputFieldXCoordinateOffset + (_fontSize * 2) + SPACE_BETWEEN_GUI_ELEMENTS_IN_ROW, currentYCoordinate, _fontSize * 2, calculatedHeightForEachElement), "+", buttonFontStyle))
            {
                _fontSize++;
            }
            currentYCoordinate += calculatedHeightForEachElement;
            if (ManualSingleton<IRandomizer>.instance == null || !ManualSingleton<IRandomizer>.instance.IsConfigured())
            {
                _drawShadedRectangle(new Rect(BACKGROUND_RECT_X_COORD, currentYCoordinate + BACKGROUND_RECT_PADDING, calculatedRectWidth, BACKGROUND_RECT_PADDING + calculatedHeightForEachElement * 4));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Yoinked directly from Subnautica randomizer, thanks Berserker
                // https://github.com/Berserker66/ArchipelagoSubnauticaModSrc/blob/master/mod/Archipelago.cs
                var inputFieldWidth = calculatedRectWidth - inputFieldXCoordinateOffset;
                var inputFieldLabelWidth = inputFieldXCoordinateOffset - SPACE_BETWEEN_GUI_ELEMENTS_IN_ROW;
                bool submit = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
                GUI.Label(new Rect(currentXCoordinate, currentYCoordinate, inputFieldXCoordinateOffset, calculatedHeightForEachElement), "Host: ", labelFontStyle);
                _hostName = GUI.TextField(new Rect(inputFieldXCoordinateOffset, currentYCoordinate, inputFieldWidth, calculatedHeightForEachElement), _hostName, inputFontStyle);
                currentYCoordinate += calculatedHeightForEachElement;
                GUI.Label(new Rect(currentXCoordinate, currentYCoordinate, calculatedRectWidth - inputFieldXCoordinateOffset, calculatedHeightForEachElement), "PlayerName: ", labelFontStyle);
                _username = GUI.TextField(new Rect(inputFieldXCoordinateOffset, currentYCoordinate, inputFieldWidth, calculatedHeightForEachElement), _username, inputFontStyle);
                currentYCoordinate += calculatedHeightForEachElement;
                GUI.Label(new Rect(currentXCoordinate, currentYCoordinate, calculatedRectWidth - inputFieldXCoordinateOffset, calculatedHeightForEachElement), "Password: ", labelFontStyle);
                _password = GUI.TextField(new Rect(inputFieldXCoordinateOffset, currentYCoordinate, inputFieldWidth, calculatedHeightForEachElement), _password, inputFontStyle);
                currentYCoordinate += calculatedHeightForEachElement;
                if (submit && Event.current.type == EventType.KeyDown)
                {
                    // The text fields have not consumed the event, which means they were not focused.
                    Console.WriteLine("NO SUBMIT FOR YOU");
                    submit = false;
                }
                if ((GUI.Button(new Rect(currentXCoordinate, currentYCoordinate, 76 + (_fontSize * 2), calculatedHeightForEachElement), "Connect", buttonFontStyle) || submit) && !string.IsNullOrEmpty(_hostName) && !string.IsNullOrEmpty(_username))
                {
                    ManualSingleton<IRandomizer>.instance = new ArchipelagoRandomizer(_hostName, _username, _password); // Trigger connection, setup will be after this
                    ArchipelagoDataHandler.HookUpEventSubscribers();
                }
                currentYCoordinate += calculatedHeightForEachElement;
                _printMessages(currentYCoordinate + BACKGROUND_RECT_PADDING * 2, calculatedRectWidth, calculatedHeightForEachElement, labelFontStyle);
            }
            else
            {
                _drawShadedRectangle(new Rect(BACKGROUND_RECT_X_COORD, currentYCoordinate + BACKGROUND_RECT_PADDING, calculatedRectWidth, BACKGROUND_RECT_PADDING + calculatedHeightForEachElement));
                GUI.Label(new Rect(currentXCoordinate, currentYCoordinate, 900, calculatedHeightForEachElement), "Archipelago configured.", labelFontStyle);
                currentYCoordinate += calculatedHeightForEachElement;
                _printMessages(currentYCoordinate + BACKGROUND_RECT_PADDING * 2, calculatedRectWidth, calculatedHeightForEachElement, labelFontStyle);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void _printMessages(long yStart, int rectWidth, int calculatedHeightForEachElement, GUIStyle labelFontStyle)
        {
            var allMessages = ManualSingleton<IUserMessages>.instance.GetMessages();
            _drawShadedRectangle(new Rect(BACKGROUND_RECT_X_COORD, yStart, rectWidth, (calculatedHeightForEachElement * allMessages.Length) + BACKGROUND_RECT_PADDING));
            for (int i = 0; i < allMessages.Length; i++)
            {
                GUI.Label(new Rect(BACKGROUND_RECT_X_COORD + BACKGROUND_RECT_PADDING, yStart + (calculatedHeightForEachElement * i), rectWidth * 3, calculatedHeightForEachElement), allMessages[i], labelFontStyle);
            }
        }

        private void _drawShadedRectangle(Rect rect)
        {
            Color startingColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = startingColor;
        }

        private void _drawAndIncrementLabel(int x, ref int y, int elementHeight, string message, GUIStyle style)
        {
            GUI.Label(new Rect(), message, style);
            y += elementHeight;
        }

        private void _setFontSize(params GUIStyle[] styles)
        {
            foreach (var style in styles)
            {
                style.fontSize = _fontSize;
            }
        }
    }
}
