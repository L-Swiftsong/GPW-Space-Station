using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
/*
 * CONTEXT:
 * 
 * This script handles the Flashlight recharge process 
 */
public class FlashlightRechargeStation : MonoBehaviour, IInteractable
{
    [Header("Recharge Settings")]
    [SerializeField] private Transform rechargePoint;
    [SerializeField] private AudioClip rechargeSound;
    [SerializeField] private float _rechargeRate = 20.0f;

    private Coroutine _rechargeFlashlightCoroutine;
    private FlashlightController _currentFlashlight;
    private int _cachedFlashlightLayer;

    private FlashlightController flashLightController;


    public void Interact(PlayerInteraction playerInteraction)
    {
        if (_currentFlashlight == null)
        {
            // The station currently doesn't have a flashlight inside it.

            if (playerInteraction.Inventory.GetEquippedItem() is FlashlightController)
            {
                // The player is holding a flashlight.
                // Start recharging the flashlight.
                HandleRecharge(playerInteraction.Inventory);
            }
        }
        else
        {
            // The station currently has a flashlight inside it.
            // Stop recharging the flashlight and return it to the player.
            HandleFlashlightPickup(playerInteraction.Inventory);
        }
    }



   // INPUT TO START THE CHARGE
    private void HandleRecharge(PlayerInventory playerInventory)
    {
        if (playerInventory.TryRemoveInventoryItemByType<FlashlightController>(out FlashlightController flashlightInstance))
        {
            // The inventory had a flashlight, which we have subsequently removed.
            StartRecharge(flashlightInstance);
        }
    }


    // ALLOWS PICKUP AFTER CHARGE IS COMPLETE
    private void HandleFlashlightPickup(PlayerInventory playerInventory)
    {
        if (_rechargeFlashlightCoroutine != null)
        {
            StopCoroutine(_rechargeFlashlightCoroutine);
        }

        AttachFlashlightToHolder(playerInventory);
    }


    // STARTS THE RECHARGE PROCESS
    private void StartRecharge(FlashlightController flashlightInstance)
    {
        _currentFlashlight = flashlightInstance;

        // Re-parent the flashlight instance to this recharge station.
        flashlightInstance.transform.SetParent(rechargePoint, worldPositionStays: false);
        flashlightInstance.transform.localPosition = Vector3.zero;

        // (Temp) Prevent the flashlight from continually being drawn in front.
        _cachedFlashlightLayer = _currentFlashlight.gameObject.layer;
        SetLayerThroughChildren(_currentFlashlight.gameObject, 0);

        _rechargeFlashlightCoroutine = StartCoroutine(RechargeFlashlight(flashlightInstance));
    }


    /// Coroutine that recharges the flashlight battery over time.
    private IEnumerator RechargeFlashlight(FlashlightController flashlightInstance)
    {
        while (flashlightInstance.IsFullyCharged() == false)
        {
            // Recharge the flashlight at a fixed rate over time.
            flashlightInstance.SetBatteryLevel(Mathf.MoveTowards(
                current: flashlightInstance.GetCurrentBattery(),
                target: flashlightInstance.GetMaxBattery(),
                maxDelta: _rechargeRate * Time.deltaTime));

            yield return null;
        }

        // Notify the player that we have finished charging.
        AudioSource.PlayClipAtPoint(rechargeSound, rechargePoint.position);
    }


    /// GIVES PLAYER FLASH BACK
    private void AttachFlashlightToHolder(PlayerInventory playerInventory)
    {
        // Revert the flashlight's layer.
        SetLayerThroughChildren(_currentFlashlight.gameObject, _cachedFlashlightLayer);

        // Add the item back to the player's inventory.
        playerInventory.AddInstantiatedItem(_currentFlashlight, new float[1] { _currentFlashlight.GetCurrentBattery() });
        _currentFlashlight = null;
    }



    private void SetLayerThroughChildren(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerThroughChildren(child.gameObject, layer);
        }
    }
}
