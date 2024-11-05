using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
 * CONTEXT:
 * 
 * This script handles the handles the player picking up flashlight 
 */
public class FlashlightPickup : MonoBehaviour, IInteractable
{
    [Header("Flashlight Pickup Settings")]
    [SerializeField] private GameObject _flashlightPrefab;

    public void Interact(PlayerInteraction playerInteraction)
    {
        PickUpFlashlight(playerInteraction.transform);
    }

    private void PickUpFlashlight(Transform playerTransform)
    {
        playerTransform.GetComponent<PlayerFlashlightController>().AddFlashlight(_flashlightPrefab);
        Destroy(gameObject);
    }
}