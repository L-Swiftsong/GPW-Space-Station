using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightController : MonoBehaviour
{
    [SerializeField] private Transform _flashlightHolder;
    public Transform FlashlightHolder => _flashlightHolder;

    private FlashLightController _flashlightController;


    #region Input Events

    private void OnEnable()
    {
        PlayerInput.OnToggleFlashlightPerformed += PlayerInput_OnToggleFlashlightPerformed;
        PlayerInput.OnFocusFlashlightStarted += PlayerInput_OnFocusFlashlightStarted;
        PlayerInput.OnFocusFlashlightCancelled += PlayerInput_OnFocusFlashlightCancelled;
    }
    private void OnDisable()
    {
        PlayerInput.OnToggleFlashlightPerformed -= PlayerInput_OnToggleFlashlightPerformed;
        PlayerInput.OnFocusFlashlightStarted -= PlayerInput_OnFocusFlashlightStarted;
        PlayerInput.OnFocusFlashlightCancelled -= PlayerInput_OnFocusFlashlightCancelled;
    }
    private void PlayerInput_OnToggleFlashlightPerformed()
    {
        if (HasFlashlight())
        {
            _flashlightController.TryToggleFlashlight();
        }
    }
    private void PlayerInput_OnFocusFlashlightStarted()
    {
        if (HasFlashlight())
        {
            _flashlightController.EnableFocusMode();
        }
    }
    private void PlayerInput_OnFocusFlashlightCancelled()
    {
        if (HasFlashlight())
        {
            _flashlightController.DisableFocusMode();
        }
    }

#endregion


    public GameObject CurrentFlashlightPrefab { get; private set; } = null;


    /// <summary> Add a new flashlight to the player, overriding the previous if it exists.</summary>
    public void AddFlashlight(GameObject flashlightPrefab)
    {
        // Remove the currently equipped flashlight (If it exists).
        RemoveFlashlight();

        // Add the new flashlight.
        CurrentFlashlightPrefab = flashlightPrefab;
        _flashlightController = Instantiate(flashlightPrefab, _flashlightHolder).GetComponent<FlashLightController>();

        _flashlightController.OnFlashlightEquipped();
    }
    /// <summary> Delete the current flashlight.</summary>
    public void RemoveFlashlight()
    {
        if (_flashlightController == null)
        {
            // We don't have an equipped flashlight. Return early to avoid errors.
            return;
        }
        
        _flashlightController.OnFlashlightUnequipped();

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

        _flashlightController.OnFlashlightEquipped();
    }
    /// <summary> Remove the current flashlight from the player, if it exists, and set it's parent to the passed transform.</summary>
    public FlashLightController DetatchFlashlight(Transform newParent)
    {
        if (_flashlightController == null)
        {
            // We don't have a flashlight equipped.
            return null;
        }

        _flashlightController.OnFlashlightUnequipped();

        FlashLightController flashlightInstanceCache = _flashlightController;
        _flashlightController = null;

        flashlightInstanceCache.transform.SetParent(newParent, worldPositionStays: false);
        return flashlightInstanceCache;
    }


    public bool HasFlashlight() => _flashlightController != null;
    public FlashLightController GetCurrentFlashlightController() => _flashlightController;
}
