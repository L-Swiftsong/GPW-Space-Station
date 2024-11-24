using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

namespace Environment.Buttons
{
    public class KeypadButton : MonoBehaviour, IInteractable
    {
        [System.Serializable] private enum ButtonType { Number, Enter, Delete }
        [SerializeField] private ButtonType _buttonType;
        [SerializeField] private int _number;
        [SerializeField] private Keypad _keypad;

        public void Interact(PlayerInteraction player)
        {
            switch (_buttonType)
            {
                case ButtonType.Number:
                    _keypad.ButtonPressed(_number.ToString());
                    break;
                case ButtonType.Enter:
                    _keypad.EnterCode();
                    break;
                case ButtonType.Delete:
                    _keypad.DeleteLastCharacter();
                    break;
            }
        }
    }
}