using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Icons
{
    [CreateAssetMenu(menuName = "Icons/Gamepad Icon Data")]
    public class InputIconDataSO : ScriptableObject
    {
        [SerializeField] private KeyboardIcons _keyboardIcons;
        [SerializeField] private GamepadIcons _xboxIcons;
        [SerializeField] private GamepadIcons _ds4Icons;
        [SerializeField] private bool _useDs4Icons = false;


        public Sprite GetSprite(PlayerInput.DeviceType deviceType, string controlPath)
        {
            return deviceType switch
            {
                PlayerInput.DeviceType.MnK => _keyboardIcons.GetSprite(controlPath),
                _ => _useDs4Icons ? _ds4Icons.GetSprite(controlPath) : _xboxIcons.GetSprite(controlPath)
            };
        }


#if UNITY_EDITOR

        private const string KEYBOARD_ICONS_RESOURCES_PATH = "UI/Icons/KeyboardAndMouse";
        private const string XBOX_ICONS_RESOURCES_PATH = "UI/Icons/XboxSeriesX";
        private const string DS4_ICONS_RESOURCES_PATH = "UI/Icons/PS4";


        [ContextMenu(itemName: "Setup/KeyboardIcons")] private void SetupKeyboardIcons() => _keyboardIcons.Editor_SetupKeyboardIcons(KEYBOARD_ICONS_RESOURCES_PATH);
        [ContextMenu(itemName: "Setup/XboxIcons")] private void SetupXboxIcons() => _xboxIcons.Editor_SetupXboxIcons(XBOX_ICONS_RESOURCES_PATH);
        [ContextMenu(itemName: "Setup/DS4Icons")] private void SetupDS4Icons() => _ds4Icons.Editor_SetupDS4Icons(DS4_ICONS_RESOURCES_PATH);
        

#endif
    }


    [System.Serializable]
    public struct GamepadIcons
    {
        [SerializeField] private Sprite buttonNorth;
        [SerializeField] private Sprite buttonSouth;
        [SerializeField] private Sprite buttonEast;
        [SerializeField] private Sprite buttonWest;

        [Space(5)]
        [SerializeField] private Sprite leftTrigger;
        [SerializeField] private Sprite rightTrigger;
        [SerializeField] private Sprite leftShoulder;
        [SerializeField] private Sprite rightShoulder;

        [Space(5)]
        [SerializeField] private Sprite dpad;
        [SerializeField] private Sprite dpadUp;
        [SerializeField] private Sprite dpadDown;
        [SerializeField] private Sprite dpadLeft;
        [SerializeField] private Sprite dpadRight;

        [Space(5)]
        [SerializeField] private Sprite leftStick;
        [SerializeField] private Sprite rightStick;
        [SerializeField] private Sprite leftStickPress;
        [SerializeField] private Sprite rightStickPress;

        [Space(5)]
        [SerializeField] private Sprite startButton;
        [SerializeField] private Sprite selectButton;


        public Sprite GetSprite(string controlPath)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            return controlPath switch
            {
                "buttonNorth" => buttonNorth,
                "buttonSouth" => buttonSouth,
                "buttonEast" => buttonEast,
                "buttonWest" => buttonWest,

                "dpad" => dpad,
                "dpad/up" => dpadUp,
                "dpad/down" => dpadDown,
                "dpad/left" => dpadLeft,
                "dpad/right" => dpadRight,

                "leftTrigger" => leftTrigger,
                "rightTrigger" => rightTrigger,
                "leftShoulder" => leftShoulder,
                "rightShoulder" => rightShoulder,

                "leftStick" => leftStick,
                "rightStick" => rightStick,
                "leftStickPress" => leftStickPress,
                "rightStickPress" => rightStickPress,

                "start" => startButton,
                "select" => selectButton,
                _ => null,
            };
        }


#if UNITY_EDITOR

        // Prefix: "PS4_" = 4 chars.
        private const int DS4_SPRITE_PREFIX_LENGTH = 4;
        // No Suffix.
        private const int DS4_SPRITE_SUFFIX_LENGTH = 0;


        public void Editor_SetupDS4Icons(string path)
        {
            Object[] spriteObjects = Resources.LoadAll(path, typeof(Sprite));

            for (int i = 0; i < spriteObjects.Length; ++i)
            {
                Sprite sprite = spriteObjects[i] as Sprite;
                string spriteKeyName = Editor_RemoveSuffixAndPrefix(sprite.name, DS4_SPRITE_PREFIX_LENGTH, DS4_SPRITE_SUFFIX_LENGTH);
                Debug.Log(spriteKeyName);

                switch (spriteKeyName)
                {
                    // Buttons.
                    case "triangle": buttonNorth = sprite; break;
                    case "cross": buttonSouth = sprite; break;
                    case "circle": buttonEast = sprite; break;
                    case "square": buttonWest = sprite; break;
                    
                    // D-Pad.
                    case "dpad_up": dpadUp = sprite; break;
                    case "dpad_down": dpadDown = sprite; break;
                    case "dpad_left": dpadLeft = sprite; break;
                    case "dpad_right": dpadRight = sprite; break;
                    case "dpad": dpad = sprite; break;
                    
                    // Triggers.
                    case "l2": leftTrigger = sprite; break;
                    case "r2": rightTrigger = sprite; break;
                    case "l1": leftShoulder = sprite; break;
                    case "r1": rightShoulder = sprite; break;
                    
                    // Sticks.
                    case "left_stick": leftStick = sprite; break;
                    case "right_stick": rightStick = sprite; break;
                    case "left_stick_click": leftStickPress = sprite; break;
                    case "right_stick_click": rightStickPress = sprite; break;

                    // Other.
                    case "options": startButton = sprite; break;
                    case "share": selectButton = sprite; break;
                }
            }
        }


        // Prefix: "XboxSeriesX_" = 12 chars.
        private const int XBOX_SPRITE_PREFIX_LENGTH = 12;
        // No Suffix.
        private const int XBOX_SPRITE_SUFFIX_LENGTH = 0;

        public void Editor_SetupXboxIcons(string path)
        {
            Object[] spriteObjects = Resources.LoadAll(path, typeof(Sprite));

            for (int i = 0; i < spriteObjects.Length; ++i)
            {
                Sprite sprite = spriteObjects[i] as Sprite;
                string spriteKeyName = Editor_RemoveSuffixAndPrefix(sprite.name, XBOX_SPRITE_PREFIX_LENGTH, XBOX_SPRITE_SUFFIX_LENGTH);
                Debug.Log(spriteKeyName);

                switch (spriteKeyName)
                {
                    // Buttons.
                    case "y": buttonNorth = sprite; break;
                    case "a": buttonSouth = sprite; break;
                    case "b": buttonEast = sprite; break;
                    case "x": buttonWest = sprite; break;
                    
                    // D-Pad.
                    case "dpad_up": dpadUp = sprite; break;
                    case "dpad_down": dpadDown = sprite; break;
                    case "dpad_left": dpadLeft = sprite; break;
                    case "dpad_right": dpadRight = sprite; break;
                    case "dpad": dpad = sprite; break;
                    
                    // Triggers.
                    case "lt": leftTrigger = sprite; break;
                    case "rt": rightTrigger = sprite; break;
                    case "lb": leftShoulder = sprite; break;
                    case "rb": rightShoulder = sprite; break;
                    
                    // Sticks.
                    case "left_stick": leftStick = sprite; break;
                    case "right_stick": rightStick = sprite; break;
                    case "left_stick_click": leftStickPress = sprite; break;
                    case "right_stick_click": rightStickPress = sprite; break;

                    // Other.
                    case "menu": startButton = sprite; break;
                    case "view": selectButton = sprite; break;
                }
            }
        }


        private string Editor_RemoveSuffixAndPrefix(string stringToProcess, int prefixLength, int suffixLength)
        {
            string output = stringToProcess.ToLower();

            // Remove Prefix.
            if (prefixLength > 0)
            {
                output = output.Substring(prefixLength);
            }

            // Remove Suffix.
            if (suffixLength > 0)
            {
                output = output.Remove(output.Length - suffixLength);
            }

            return output;
        }

        #endif
    }

    [System.Serializable]
    public struct KeyboardIcons
    {
        [Header("Keyboard")]
        [SerializeField] private Sprite _q;
        [SerializeField] private Sprite _w;
        [SerializeField] private Sprite _e;
        [SerializeField] private Sprite _r;
        [SerializeField] private Sprite _t;
        [SerializeField] private Sprite _y;
        [SerializeField] private Sprite _u;
        [SerializeField] private Sprite _i;
        [SerializeField] private Sprite _o;
        [SerializeField] private Sprite _p;

        [Space(5)]
        [SerializeField] private Sprite _a;
        [SerializeField] private Sprite _s;
        [SerializeField] private Sprite _d;
        [SerializeField] private Sprite _f;
        [SerializeField] private Sprite _g;
        [SerializeField] private Sprite _h;
        [SerializeField] private Sprite _j;
        [SerializeField] private Sprite _k;
        [SerializeField] private Sprite _l;

        [Space(5)]
        [SerializeField] private Sprite _z;
        [SerializeField] private Sprite _x;
        [SerializeField] private Sprite _c;
        [SerializeField] private Sprite _v;
        [SerializeField] private Sprite _b;
        [SerializeField] private Sprite _n;
        [SerializeField] private Sprite _m;

        [Space(5)]
        [SerializeField] private Sprite _1;
        [SerializeField] private Sprite _2;
        [SerializeField] private Sprite _3;
        [SerializeField] private Sprite _4;
        [SerializeField] private Sprite _5;
        [SerializeField] private Sprite _6;
        [SerializeField] private Sprite _7;
        [SerializeField] private Sprite _8;
        [SerializeField] private Sprite _9;
        [SerializeField] private Sprite _0;

        [Space(5)]
        [SerializeField] private Sprite _upArrow;
        [SerializeField] private Sprite _downArrow;
        [SerializeField] private Sprite _leftArrow;
        [SerializeField] private Sprite _rightArrow;

        [Space(5)]
        [SerializeField] private Sprite _space;
        [SerializeField] private Sprite _shift;
        [SerializeField] private Sprite _control;
        [SerializeField] private Sprite _return;
        [SerializeField] private Sprite _backspace;
        [SerializeField] private Sprite _escape;


        [Header("Mouse")]
        [SerializeField] private Sprite _lmb;
        [SerializeField] private Sprite _rmb;
        [SerializeField] private Sprite _mmb;


        public Sprite GetSprite(string controlPath)
        {
            return controlPath switch
            {
                #region Keyboard

                // Letters.
                "q" => _q,
                "w" => _w,
                "e" => _e,
                "r" => _r,
                "t" => _t,
                "y" => _y,
                "u" => _u,
                "i" => _i,
                "o" => _o,
                "p" => _p,

                "a" => _a,
                "s" => _s,
                "d" => _d,
                "f" => _f,
                "g" => _g,
                "h" => _h,
                "j" => _j,
                "k" => _k,
                "l" => _l,

                "z" => _z,
                "x" => _x,
                "c" => _c,
                "v" => _v,
                "b" => _b,
                "n" => _n,
                "m" => _m,

                // Numbers.
                "0" => _0,
                "1" => _1,
                "2" => _2,
                "3" => _3,
                "4" => _4,
                "5" => _5,
                "6" => _6,
                "7" => _7,
                "8" => _8,
                "9" => _9,

                // Arrow Keys.
                "upArrow" => _upArrow,
                "downArrow" => _downArrow,
                "leftArrow" => _leftArrow,
                "rightArrow" => _rightArrow,

                // Special Keys.
                "space" => _space,
                "leftShift" => _shift,
                "leftCtrl" => _control,
                "enter" => _return,
                "backspace" => _backspace,
                "escape" => _escape,

                #endregion

                #region Mouse

                "leftButton" => _lmb,
                "rightButton" => _rmb,
                "middleButton" => _mmb,

                #endregion


                _ => null,
            };
        }


#if UNITY_EDITOR

        // No Prefix.
        private const int KEYBOARD_SPRITE_PREFIX_LENGTH = 0;
        // Suffix: "_Key" = 4 chars.
        private const int KEYBOARD_SPRITE_SUFFIX_LENGTH = 4;

        public void Editor_SetupKeyboardIcons(string path)
        {
            Object[] keyboardSpriteObjects = Resources.LoadAll(path, typeof(Sprite));

            for(int i = 0; i < keyboardSpriteObjects.Length; ++i)
            {
                Sprite sprite = keyboardSpriteObjects[i] as Sprite;
                string spriteKeyName = Editor_RemoveSuffixAndPrefix(sprite.name, KEYBOARD_SPRITE_PREFIX_LENGTH, KEYBOARD_SPRITE_SUFFIX_LENGTH);
                Debug.Log(spriteKeyName);

                switch (spriteKeyName)
                {
                    #region Keyboard

                    // Letters.
                    case "q": _q = sprite; break;
                    case "w": _w = sprite; break;
                    case "e": _e = sprite; break;
                    case "r": _r = sprite; break;
                    case "t": _t = sprite; break;
                    case "y": _y = sprite; break;
                    case "u": _u = sprite; break;
                    case "i": _i = sprite; break;
                    case "o": _o = sprite; break;
                    case "p": _p = sprite; break;

                    case "a": _a = sprite; break;
                    case "s": _s = sprite; break;
                    case "d": _d = sprite; break;
                    case "f": _f = sprite; break;
                    case "g": _g = sprite; break;
                    case "h": _h = sprite; break;
                    case "j": _j = sprite; break;
                    case "k": _k = sprite; break;
                    case "l": _l = sprite; break;

                    case "z": _z = sprite; break;
                    case "x": _x = sprite; break;
                    case "c": _c = sprite; break;
                    case "v": _v = sprite; break;
                    case "b": _b = sprite; break;
                    case "n": _n = sprite; break;
                    case "m": _m = sprite; break;

                    // Numbers.
                    case "0": _0 = sprite; break;
                    case "1": _1 = sprite; break;
                    case "2": _2 = sprite; break;
                    case "3": _3 = sprite; break;
                    case "4": _4 = sprite; break;
                    case "5": _5 = sprite; break;
                    case "6": _6 = sprite; break;
                    case "7": _7 = sprite; break;
                    case "8": _8 = sprite; break;
                    case "9": _9 = sprite; break;

                    // Arrow Keys.
                    case "arrow_up": _upArrow = sprite; break;
                    case "arrow_down": _downArrow = sprite; break;
                    case "arrow_left": _leftArrow = sprite; break;
                    case "arrow_right": _rightArrow = sprite; break;

                    // Special Keys.
                    case "space": _space = sprite; break;
                    case "shift": _shift = sprite; break;
                    case "ctrl": _control = sprite; break;
                    case "enter_alt": _return = sprite; break;
                    case "backspace_alt": _backspace = sprite; break;
                    case "esc": _escape = sprite; break;

                    #endregion

                    #region Mouse

                    case "mouse_left": _lmb = sprite; break;
                    case "mouse_right": _rmb = sprite; break;
                    case "mouse_middle": _mmb = sprite; break;

                    #endregion
                }
            }
        }

        private string Editor_RemoveSuffixAndPrefix(string stringToProcess, int prefixLength, int suffixLength)
        {
            string output = stringToProcess.ToLower();

            // Remove Prefix.
            if (prefixLength > 0)
            {
                output = output.Substring(prefixLength);
            }

            // Remove Suffix.
            if (suffixLength > 0)
            {
                output = output.Remove(output.Length - suffixLength);
            }

            return output;
        }


#endif
    }
}
