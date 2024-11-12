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
    [Header("Flashlight Pickup Settings")]
    [SerializeField] private FlashlightController _flashlightPrefab;

    public void Interact(PlayerInteraction playerInteraction)
    {
        playerInteraction.Inventory.AddItem(_flashlightPrefab);
        Destroy(gameObject);
    }
}