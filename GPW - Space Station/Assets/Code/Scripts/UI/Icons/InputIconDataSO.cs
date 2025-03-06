using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Icons
{
    [CreateAssetMenu(menuName = "Icons/Gamepad Icon Data")]
    public class InputIconDataSO : ScriptableObject
    {
        [SerializeField] private GamepadIcons _xboxIcons;
        [SerializeField] private GamepadIcons _ds4Icons;
        [SerializeField] private bool _useDs4Icons = false;


        [System.Serializable] public enum DeviceType { Keyboard, XBox, DS4 }
        public Sprite GetSprite(PlayerInput.DeviceType deviceType, string controlPath)
        {
            return deviceType switch
            {
                _ => _useDs4Icons ? _ds4Icons.GetSprite(controlPath) : _xboxIcons.GetSprite(controlPath)
            };
        }
    }


    [System.Serializable]
    public struct GamepadIcons
    {
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;

        public Sprite GetSprite(string controlPath)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (controlPath)
            {
                case "buttonSouth":
                    return buttonSouth;
                case "buttonNorth":
                    return buttonNorth;
                case "buttonEast":
                    return buttonEast;
                case "buttonWest":
                    return buttonWest;
                case "start":
                    return startButton;
                case "select":
                    return selectButton;
                case "leftTrigger":
                    return leftTrigger;
                case "rightTrigger":
                    return rightTrigger;
                case "leftShoulder":
                    return leftShoulder;
                case "rightShoulder":
                    return rightShoulder;
                case "dpad":
                    return dpad;
                case "dpad/up":
                    return dpadUp;
                case "dpad/down":
                    return dpadDown;
                case "dpad/left":
                    return dpadLeft;
                case "dpad/right":
                    return dpadRight;
                case "leftStick":
                    return leftStick;
                case "rightStick":
                    return rightStick;
                case "leftStickPress":
                    return leftStickPress;
                case "rightStickPress":
                    return rightStickPress;
            }
            return null;
        }
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



        #if UNITY_EDITOR

        [ContextMenu(itemName: "Setup KeyboardIcons")]
        private void SetupKeyboardIcons()
        {
            Sprite[] keyboardSprites = Resources.LoadAll("Icons/MouseAndKeyboard", typeof(Sprite)) as Sprite[];

            for(int i = 0; i < keyboardSprites.Length; ++i)
            {
                switch (keyboardSprites[i].name)
                {
                    #region Keyboard

                    case "Keyboard_Q": _q = keyboardSprites[i]; break;
                    case "Keyboard_W": _w = keyboardSprites[i]; break;
                    case "Keyboard_E": _e = keyboardSprites[i]; break;
                    case "Keyboard_R": _r = keyboardSprites[i]; break;
                    case "Keyboard_T": _t = keyboardSprites[i]; break;
                    case "Keyboard_Y": _y = keyboardSprites[i]; break;
                    case "Keyboard_U": _u = keyboardSprites[i]; break;
                    case "Keyboard_I": _i = keyboardSprites[i]; break;
                    case "Keyboard_O": _o = keyboardSprites[i]; break;
                    case "Keyboard_P": _p = keyboardSprites[i]; break;

                    case "Keyboard_A": _a = keyboardSprites[i]; break;
                    case "Keyboard_S": _s = keyboardSprites[i]; break;
                    case "Keyboard_D": _d = keyboardSprites[i]; break;
                    case "Keyboard_F": _f = keyboardSprites[i]; break;
                    case "Keyboard_G": _g = keyboardSprites[i]; break;
                    case "Keyboard_H": _h = keyboardSprites[i]; break;
                    case "Keyboard_J": _j = keyboardSprites[i]; break;
                    case "Keyboard_K": _k = keyboardSprites[i]; break;
                    case "Keyboard_L": _l = keyboardSprites[i]; break;

                    case "Keyboard_Z": _z = keyboardSprites[i]; break;
                    case "Keyboard_X": _x = keyboardSprites[i]; break;
                    case "Keyboard_C": _c = keyboardSprites[i]; break;
                    case "Keyboard_V": _v = keyboardSprites[i]; break;
                    case "Keyboard_B": _b = keyboardSprites[i]; break;
                    case "Keyboard_N": _n = keyboardSprites[i]; break;
                    case "Keyboard_M": _m = keyboardSprites[i]; break;

                    case "Keyboard_1": _1 = keyboardSprites[i]; break;
                    case "Keyboard_2": _2 = keyboardSprites[i]; break;
                    case "Keyboard_3": _3 = keyboardSprites[i]; break;
                    case "Keyboard_4": _4 = keyboardSprites[i]; break;
                    case "Keyboard_5": _5 = keyboardSprites[i]; break;
                    case "Keyboard_6": _6 = keyboardSprites[i]; break;
                    case "Keyboard_7": _7 = keyboardSprites[i]; break;
                    case "Keyboard_8": _8 = keyboardSprites[i]; break;
                    case "Keyboard_9": _9 = keyboardSprites[i]; break;
                    case "Keyboard_0": _0 = keyboardSprites[i]; break;

                    case "Keyboard_UpArrow": _upArrow = keyboardSprites[i]; break;
                    case "Keyboard_DownArrow": _downArrow = keyboardSprites[i]; break;
                    case "Keyboard_LeftArrow": _leftArrow = keyboardSprites[i]; break;
                    case "Keyboard_RightArrow": _rightArrow = keyboardSprites[i]; break;

                    case "Keyboard_Space": _space = keyboardSprites[i]; break;
                    case "Keyboard_Shift": _shift = keyboardSprites[i]; break;
                    case "Keyboard_Control": _control = keyboardSprites[i]; break;
                    case "Keyboard_Return": _return = keyboardSprites[i]; break;
                    case "Keyboard_Backspace": _backspace = keyboardSprites[i]; break;
                    case "Keyboard_Escape": _escape = keyboardSprites[i]; break;

                    #endregion

                    #region Mouse

                    case "Mouse_Button0": _lmb = keyboardSprites[i]; break;
                    case "Mouse_Button1": _rmb = keyboardSprites[i]; break;
                    case "Mouse_Button2": _mmb = keyboardSprites[i]; break;

                    #endregion
                }
            }
        }

        #endif
    }
}
