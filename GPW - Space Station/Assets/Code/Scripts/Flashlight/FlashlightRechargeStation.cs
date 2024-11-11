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
    private GameObject _currentFlashlight;

    private PlayerInventory playerInventory;


    private void Start()
    {
        //Refereneces
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

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
        FlashLightController flashlightController = player.GetComponentInChildren<FlashLightController>();
        if (flashlightController != null && flashlightController.HasFlashlight())
        {
            StartRecharge(flashlightController);

            playerInventory.RemoveFlashLight(); //Remove flashlight from inventory when recharge is started
        }
    }

    // ALLOWS PICKUP AFTER CHARGE IS COMPLETE 

    private void HandleFlashlightPickup(Transform player)
    {
        TmpFlashlightController flashlightController = player.GetComponent<TmpFlashlightController>();
        Debug.Log(flashlightController.name);
        AttachFlashlightToHolder(flashlightController);
    }


    // STARTS THE RECHARGE PROCESS 
  
    private void StartRecharge(FlashLightController flashlightController)
    {
        flashlightController.DisableFlashlight();

        _currentFlashlight = flashlightController.gameObject;
        _currentFlashlight.transform.SetParent(rechargePoint);
        _currentFlashlight.transform.position = rechargePoint.position;
        _currentFlashlight.transform.rotation = rechargePoint.rotation;

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

        playerInventory.flashLightPickedUp = false; //Allows flashlight to be re-added to inventory after recharge is finished

        DetachFlashlightFromRechargePoint();
    }

    /// GIVES PLAYER FLASH BACK 

    private void AttachFlashlightToHolder(TmpFlashlightController tmpFlashlightController)
    {
        _currentFlashlight.transform.SetParent(tmpFlashlightController.FlashlightHolder);
        _currentFlashlight.transform.localPosition = Vector3.zero;
        _currentFlashlight.transform.localRotation = Quaternion.identity;

        // Allow the player to use the flashlight again
        FlashLightController flashlightController = _currentFlashlight.GetComponent<FlashLightController>();
        if (flashlightController != null)
        {
            flashlightController.EnableFlashlight();
        }

        _currentFlashlight = null;
    }


    /// REMOVES THE FLASHLIGHT FROM THE DEVICE WHEN PLAYER PICKS IT BACK UP 
  
    private void DetachFlashlightFromRechargePoint()
    {
        _currentFlashlight.transform.SetParent(null);
        _isRecharging = false;
    }
}
