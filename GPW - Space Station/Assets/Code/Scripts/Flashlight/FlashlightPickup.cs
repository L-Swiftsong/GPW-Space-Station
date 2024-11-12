using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * CONTEXT:
 * 
 * This script handles the handles the player picking up flashlight 
 */
public class FlashlightPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory.Data.InventoryItemDataSO _flashlightInventoryData;

    public void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Inventory.AddItem(_flashlightInventoryData);
        Destroy(gameObject);
    }
}