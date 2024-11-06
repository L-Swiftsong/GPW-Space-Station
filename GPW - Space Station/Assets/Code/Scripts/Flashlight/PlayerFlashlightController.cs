using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightController : MonoBehaviour
{
    [SerializeField] private Transform _flashlightHolder;
    public Transform FlashlightHolder => _flashlightHolder;

    private FlashLightController _flashlightController;
    

    public GameObject CurrentFlashlightPrefab { get; private set; } = null;


    public void AddFlashlight(GameObject flashlightPrefab)
    {
        CurrentFlashlightPrefab = flashlightPrefab;
        _flashlightController = Instantiate(flashlightPrefab, _flashlightHolder).GetComponent<FlashLightController>();
    }

    public void AttachFlashlight(FlashLightController flashlightInstance)
    {
        _flashlightController = flashlightInstance;

        _flashlightController.transform.SetParent(_flashlightHolder);
        _flashlightController.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    public GameObject DetatchFlashlight(Transform newParent)
    {
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
