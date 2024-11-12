using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : MonoBehaviour, IInteractable
{
    public string buttonType;
    public string number;
    public Keypad keypad;

    public void Interact(PlayerInteraction player)
    {
        if (keypad != null)
        {
            if (buttonType == "Number")
            {
                keypad.ButtonPressed(number);
            }
            else if (buttonType == "Enter")
            {
                keypad.EnterCode();
            }
            else if (buttonType == "Delete")
            {
                keypad.DeleteLastCharacter();
            }
        }
    }
}
