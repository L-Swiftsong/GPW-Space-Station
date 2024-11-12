using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightController : MonoBehaviour
{
    [SerializeField] private Transform _flashlightHolder;
    public Transform FlashlightHolder => _flashlightHolder;

    private FlashlightController _flashlightController;


    public GameObject CurrentFlashlightPrefab { get; private set; } = null;


    /// <summary> Add a new flashlight to the player, overriding the previous if it exists.</summary>
    public void AddFlashlight(GameObject flashlightPrefab)
    {
        // Add the new flashlight.
        CurrentFlashlightPrefab = flashlightPrefab;
        _flashlightController = Instantiate(flashlightPrefab, _flashlightHolder).GetComponent<FlashlightController>();

        _flashlightController.OnFlashlightEquipped();
    }


    public bool HasFlashlight() => _flashlightController != null;
    public FlashlightController GetCurrentFlashlightController() => _flashlightController;
}
