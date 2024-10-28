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
    [SerializeField] private GameObject flashlightPrefab;

    public void Interact(PlayerInteraction playerInteraction)
    {
        PickUpFlashlight(playerInteraction.transform);
    }

    private void PickUpFlashlight(Transform playerTransform)
    {
        // Temp.
        Transform flashlightHolder = playerTransform.GetComponent<TmpFlashlightController>().FlashlightHolder;
        GameObject flashlightInstance = Instantiate(flashlightPrefab, flashlightHolder.position, flashlightHolder.rotation, flashlightHolder);

        if (flashlightInstance.TryGetComponent<FlashLightController>(out FlashLightController flashlightController))
        {
            flashlightController.PickUpFlashlight();
        }

        Destroy(gameObject); // remioves the flashlight the player picked up 
    }
}