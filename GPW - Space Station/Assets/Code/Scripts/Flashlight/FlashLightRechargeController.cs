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
public class FlashLightRechargeController : MonoBehaviour
{
    [Header("Recharge Settings")]

    [SerializeField] private Transform rechargePoint;
    [SerializeField] private AudioClip rechargeSound;
    [SerializeField] private float rechargeDuration = 5.0f;
    [SerializeField] private Transform flashlightHolder;

    private bool _isRecharging = false;
    private GameObject _currentFlashlight;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleRecharge(other);
            HandleFlashlightPickup(other);
        }
    }


   // INPUT TO START THE CHARGE 

    private void HandleRecharge(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !_isRecharging && _currentFlashlight == null)
        {
            FlashLightController flashlightController = other.GetComponentInChildren<FlashLightController>();
            if (flashlightController != null && flashlightController.HasFlashlight())
            {
                StartRecharge(flashlightController);
            }
        }
    }

    // ALLOWS PICKUP AFTER CHARGE IS COMPLETE 

    private void HandleFlashlightPickup(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _currentFlashlight != null && !_isRecharging)
        {
            AttachFlashlightToHolder();
        }
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

        DetachFlashlightFromRechargePoint();
    }

    /// GIVES PLAYER FLASH BACK 

    private void AttachFlashlightToHolder()
    {
        _currentFlashlight.transform.SetParent(flashlightHolder);
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _currentFlashlight != null && !_isRecharging)
        {
            _currentFlashlight.transform.SetParent(null);
        }
    }
}
