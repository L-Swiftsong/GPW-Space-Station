using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{   
    private bool isLocked = true;

    [SerializeField]
    private int requiredKeyCardId;

    public int RequiredKeyCardId
    {
        get => requiredKeyCardId; 
        set => requiredKeyCardId = value; 
    }

    [SerializeField]
    private Color doorColour;

    public Color DoorColour
    {
        get => doorColour;
        set => doorColour = value;
    }

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material materialInstance = new Material(renderer.material);

            renderer.material = materialInstance; 

            renderer.material.SetColor("_Color", doorColour);

        }
    }

    public void TryUnlockDoor(PlayerInventory playerInventory)
    {
        if (playerInventory.HasKeyCard(RequiredKeyCardId))
        {
            UnlockDoor();
            Debug.Log($"Door opened with keycard: {RequiredKeyCardId}");
        }
        else
        {
            Debug.Log($"This door requires keycard: {RequiredKeyCardId}");
        }
    }

    private void UnlockDoor()
    {
        if (isLocked)
        {
            isLocked = false;
            Debug.Log("Door unlocked!");

            gameObject.SetActive(false);
        }
    }
}

