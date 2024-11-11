using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Keypad : MonoBehaviour
{
    public string correctCode = "0210";
    private string playerInput = "";

    public TextMeshPro displayText; 

    public event Action OnCodeCorrect;

    private void Start()
    {
        if (displayText == null)
        {
            displayText = GameObject.Find("DisplayText").GetComponent<TextMeshPro>();
        }
    }

    public void ButtonPressed(string number)
    {
        if (playerInput.Length < correctCode.Length)
        {
            playerInput += number;
            displayText.text = playerInput;
        }
    }

    public void EnterCode()
    {
        if (playerInput == correctCode)
        {
            OnCodeCorrect?.Invoke(); 
        }
        else
        {
            playerInput = "";
            displayText.text = "";
        }
    }

    public void DeleteLastCharacter()
    {
        if(playerInput.Length > 0)
        {
            playerInput = playerInput.Substring(0, playerInput.Length - 1); 

            displayText.text = playerInput;
        }
    }
}
