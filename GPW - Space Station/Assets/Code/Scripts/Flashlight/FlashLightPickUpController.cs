using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
 * CONTEXT:
 * 
 * This script handles the handles the player picking up flashlight 
 */
public class FlashLightPickUpController : MonoBehaviour
{
    [Header("Flashlight Pickup Settings")]

    [SerializeField] private GameObject flashlightPrefab;
    [SerializeField] private TextMeshProUGUI batteryTextUI;

    private bool _hasFlashlight = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasFlashlight)
        {
            StartCoroutine(WaitForPickup(other));
        }
    }

    private IEnumerator WaitForPickup(Collider other)
    {
        while (other.CompareTag("Player") && !_hasFlashlight)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                PickUpFlashlight();
            }
            yield return null;
        }
    }

    private void PickUpFlashlight()
    {
        // Temp.
        Transform flashlightHolder = PlayerManager.Instance.Player.GetComponent<TmpFlashlightController>().FlashlightHolder;
        GameObject flashlightInstance = Instantiate(
            flashlightPrefab,
            flashlightHolder.position,
            flashlightHolder.rotation,
            flashlightHolder
        );

        _hasFlashlight = true;

        FlashLightController flashlightController = flashlightInstance.GetComponent<FlashLightController>();
        if (flashlightController != null)
        {
            flashlightController.SetBatteryUI(batteryTextUI);
            flashlightController.PickUpFlashlight();
        }

        Destroy(gameObject); // remioves the flashlight the player picked up 
    }
}