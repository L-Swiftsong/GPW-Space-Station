using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

public class KeypadButton : MonoBehaviour, IInteractable
{
    [System.Serializable] private enum ButtonType { Number, Enter, Delete }
    [SerializeField] private ButtonType _buttonType;
    [SerializeField] private int _number;
    [SerializeField] private Keypad _keypad;

    public void Interact(PlayerInteraction player)
    {
        if (_keypad != null)
        {
            if (_buttonType == ButtonType.Number)
            {
                _keypad.ButtonPressed(_number.ToString());
            }
            else if (_buttonType == ButtonType.Enter)
            {
                _keypad.EnterCode();
            }
            else if (_buttonType == ButtonType.Delete)
            {
                _keypad.DeleteLastCharacter();
            }
        }
    }
}
