using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private FlashLightController _currentFlashlight;
    private int _cachedFlashlightLayer;


    public void Interact(PlayerInteraction playerInteraction)
    {
        if (_currentFlashlight == null)
        {
            // The station currently doesn't have a flashlight inside it.
            // Start recharging the flashlight.
            HandleRecharge(playerInteraction.transform);
        }
        else
        {
            // The station currently has a flashlight inside it.
            // Stop recharging the flashlight and return it to the player.
            HandleFlashlightPickup(playerInteraction.transform);
        }
    }



   // INPUT TO START THE CHARGE
    private void HandleRecharge(Transform player)
    {
        PlayerFlashlightController flashlightController = player.GetComponent<PlayerFlashlightController>();
        if (flashlightController != null && !flashlightController.GetCurrentFlashlightController().IsFullyCharged())
        {
            StartRecharge(flashlightController);
        }
    }


    // ALLOWS PICKUP AFTER CHARGE IS COMPLETE
    private void HandleFlashlightPickup(Transform player)
    {
        if (_rechargeFlashlightCoroutine != null)
        {
            StopCoroutine(_rechargeFlashlightCoroutine);
        }

        PlayerFlashlightController flashlightController = player.GetComponent<PlayerFlashlightController>();
        Debug.Log(flashlightController.name);
        AttachFlashlightToHolder(flashlightController);
    }


    // STARTS THE RECHARGE PROCESS
    private void StartRecharge(PlayerFlashlightController playerFlashlightController)
    {
        FlashLightController flashlightController = playerFlashlightController.GetCurrentFlashlightController();
        _currentFlashlight = playerFlashlightController.DetatchFlashlight(rechargePoint);

        // (Temp) Prevent the flashlight from continually being drawn in front.
        _cachedFlashlightLayer = _currentFlashlight.gameObject.layer;
        SetLayerThroughChildren(_currentFlashlight.gameObject, 0);

        _rechargeFlashlightCoroutine = StartCoroutine(RechargeFlashlight(flashlightController));
    }


    /// Coroutine that recharges the flashlight battery over time.
    private IEnumerator RechargeFlashlight(FlashLightController flashlightController)
    {
        while (flashlightController.IsFullyCharged() == false)
        {
            // Recharge the flashlight at a fixed rate over time.
            flashlightController.SetBatteryLevel(Mathf.MoveTowards(
                current: flashlightController.GetCurrentBattery(),
                target: flashlightController.GetMaxBattery(),
                maxDelta: _rechargeRate * Time.deltaTime));

            yield return null;
        }

        // Notify the player that we have finished charging.
        AudioSource.PlayClipAtPoint(rechargeSound, rechargePoint.position);
    }


    /// GIVES PLAYER FLASH BACK
    private void AttachFlashlightToHolder(PlayerFlashlightController playerFlashlightController)
    {
        // Revert the flashlight's layer.
        SetLayerThroughChildren(_currentFlashlight.gameObject, _cachedFlashlightLayer);

        playerFlashlightController.AttachFlashlight(_currentFlashlight);
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
