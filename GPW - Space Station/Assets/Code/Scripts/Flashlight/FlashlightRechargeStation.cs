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
    [SerializeField] private float rechargeDuration = 5.0f;

    private bool _isRecharging = false;
    private FlashLightController _currentFlashlight;


    public void Interact(PlayerInteraction playerInteraction)
    {
        if (_isRecharging)
        {
            // We are in the process of recharging.
            return;
        }

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
        if (flashlightController != null)
        {
            StartRecharge(flashlightController);
        }
    }


    // ALLOWS PICKUP AFTER CHARGE IS COMPLETE
    private void HandleFlashlightPickup(Transform player)
    {
        PlayerFlashlightController flashlightController = player.GetComponent<PlayerFlashlightController>();
        Debug.Log(flashlightController.name);
        AttachFlashlightToHolder(flashlightController);
    }


    // STARTS THE RECHARGE PROCESS
    private void StartRecharge(PlayerFlashlightController playerFlashlightController)
    {
        FlashLightController flashlightController = playerFlashlightController.GetCurrentFlashlightController();
        _currentFlashlight = playerFlashlightController.DetatchFlashlight(rechargePoint);

        _isRecharging = true;
        StartCoroutine(RechargeFlashlight(flashlightController));
    }


    /// Coroutine that recharges the flashlight battery over time.
    private IEnumerator RechargeFlashlight(FlashLightController flashlightController)
    {
        float elapsedTime = 0f;
        float startBattery = flashlightController.FlashlightBattery;
        float rechargeTime = rechargeDuration;

        while (elapsedTime < rechargeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rechargeTime;

            float newBatteryLevel = Mathf.Lerp(startBattery, 100f, t);
            flashlightController.SetBatteryLevel(newBatteryLevel);

            yield return null;
        }

        flashlightController.SetBatteryLevel(100f);
        AudioSource.PlayClipAtPoint(rechargeSound, rechargePoint.position);
        _isRecharging = false;
    }


    /// GIVES PLAYER FLASH BACK
    private void AttachFlashlightToHolder(PlayerFlashlightController playerFlashlightController)
    {
        playerFlashlightController.AttachFlashlight(_currentFlashlight);
        _currentFlashlight = null;
    }
}
