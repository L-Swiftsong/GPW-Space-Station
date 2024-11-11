using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Doors;
using System;

public class ButtonManager : MonoBehaviour
{

    [Header("Button Press Sequence")]
    [SerializeField] public ButtonPress[] correctButtonSequence;
    private static int currentButtonIndex = 0;

    [Header("references")]
    [SerializeField] private ExternalInputDoor _connectedDoor;
    [SerializeField] private Light[] buttonLights;
    [SerializeField] private ButtonPress[] buttonPress;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var light in buttonLights)
        {
            light.enabled = true;
        }

        buttonPress = GetComponentsInChildren<ButtonPress>();
    }

    public void ButtonPressed(ButtonPress button)
    {
        if (correctButtonSequence[currentButtonIndex] == button)
        {
            Debug.Log(button.gameObject.name + " pressed correctly!");

            buttonLights[currentButtonIndex].enabled = true;
            buttonLights[currentButtonIndex].color = Color.green;

            currentButtonIndex++;

            if (currentButtonIndex == correctButtonSequence.Length)
            {
                Debug.Log("All buttons pressed correctly. Opening the door!");
                OpenDoor();
            }
        }
        else
        {
            StartCoroutine(FlashLights());
            ResetSequence();
        }
    }

    private void OpenDoor()
    {
        if (_connectedDoor != null)
        {
            _connectedDoor.Open();
        }
    }

    private void ResetSequence()
    {
        Debug.Log("Wrong button! Resetting sequence.");
        currentButtonIndex = 0;

        foreach (var light in buttonLights)
        {
            light.enabled = true;
        }

        foreach (var button in buttonPress)
        {
            button.ResetButton();
        }
    }

    private IEnumerator FlashLights()
    {
        foreach (var light in buttonLights)
        {
            light.enabled = true;
            light.color = Color.red;
        }

        yield return new WaitForSeconds(1f);

        foreach (var light in buttonLights)
        {
            light.color = Color.white;
        }

        yield return new WaitForSeconds(0.2f);

    }
}
