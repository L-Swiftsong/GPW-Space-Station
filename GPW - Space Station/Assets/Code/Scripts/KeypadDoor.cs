using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Environment.Doors;

public class KeypadDoor : Door
{
    [SerializeField] private Keypad keypad; 

    private void Start()
    {
        if (keypad != null)
        {
            keypad.OnCodeCorrect += Open; 
        }
    }

    private void OnDestroy()
    {
        if (keypad != null)
        {
            keypad.OnCodeCorrect -= Open; 
        }
    }
}
