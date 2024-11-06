using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightController : MonoBehaviour
{
    [SerializeField] private Transform _flashlightHolder;
    public Transform FlashlightHolder => _flashlightHolder;

    private FlashLightController _flashlightController;
    

    public GameObject CurrentFlashlightPrefab { get; private set; } = null;


    /// <summary> Add a new flashlight to the player, overriding the previous if it exists.</summary>
    public void AddFlashlight(GameObject flashlightPrefab)
    {
        // Remove the currently equipped flashlight (If it exists).
        RemoveFlashlight();

        // Add the new flashlight.
        CurrentFlashlightPrefab = flashlightPrefab;
        _flashlightController = Instantiate(flashlightPrefab, _flashlightHolder).GetComponent<FlashLightController>();
    }
    /// <summary> Delete the current flashlight.</summary>
    public void RemoveFlashlight()
    {
        if (_flashlightController == null)
        {
            // We don't have an equipped flashlight. Return early to avoid errors.
            return;
        }
        
        // Destroy and remove references to the flashlight instance.
        Destroy(_flashlightController.gameObject);
        _flashlightController = null;

        // Clear our currently assigned flashlight prefab.
        CurrentFlashlightPrefab = null;
    }


    /// <summary> Attach an existing flashlight to this player, overriding the previous flashlight if it exists.</summary>
    public void AttachFlashlight(FlashLightController flashlightInstance)
    {
        // Remove the currently equipped flashlight (If it exists).
        RemoveFlashlight();
        

        _flashlightController = flashlightInstance;

        _flashlightController.transform.SetParent(_flashlightHolder);
        _flashlightController.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    /// <summary> Remove the current flashlight from the player, if it exists, and set it's parent to the passed transform.</summary>
    public GameObject DetatchFlashlight(Transform newParent)
    {
        if (_flashlightController == null)
        {
            // We don't have a flashlight equipped.
            return null;
        }

        GameObject flashlightInstance = _flashlightController.gameObject;
        _flashlightController = null;

        flashlightInstance.transform.SetParent(newParent);
        return flashlightInstance;
    }


    public bool HasFlashlight() => _flashlightController != null;
    public float GetFlashlightCharge()
    {
        if (!HasFlashlight())
            return -1.0f;

        return _flashlightController.FlashlightBattery;
    }
}
