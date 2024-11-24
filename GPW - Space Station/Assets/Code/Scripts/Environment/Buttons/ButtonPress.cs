using Environment.Doors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

public class ButtonPress : MonoBehaviour, IInteractable
{
    [Header("Button Press")]

    private bool _isPressed = false;
    public bool IsPressed => _isPressed;

    [Header("References")]
    [SerializeField] private ExternalInputDoor _connectedDoor;
    [SerializeField] private Light correspondingLight;
    private ButtonManager buttonManager;

    void Start()
    {
        buttonManager = GetComponentInParent<ButtonManager>();
    }


    public void Interact(PlayerInteraction playerInteraction)
    {
        if (_isPressed)
        {
            return;
        }

        _isPressed = true;
        Debug.Log(gameObject.name + " pressed");

        buttonManager?.ButtonPressed(this);

    }

    public void ResetButton()
    {
        _isPressed = false;
    }
}
