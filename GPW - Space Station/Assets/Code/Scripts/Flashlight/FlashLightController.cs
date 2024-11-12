using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Inventory.Data;
using Inventory;
/*
* FlashLightController.cs
* 
* This script handles the flashlight functionality. It allows the player to toggle the flashlight,
* enter focus mode to narrow the beam and increase intensity, and drains the battery + deals damage to enemies 
*/

public class FlashlightController : InventoryItem
{
    private Light _flashlightLight;
    private bool _isOn = false;
    private bool _isFocused = false;


    [Header("Flashlight Light Settings")]
    [SerializeField] private float _defaultConeAngle = 45.0f;
    [SerializeField] private float _defaultIntensity = 2.5f;

    [Space(5)]
    [SerializeField] private float _focusedConeAngle = 25.0f;
    [SerializeField] private float _focusedIntensity = 4.0f;


    [Space(5)]
    [SerializeField] private float _angleChangeRate = 2.0f;
    [SerializeField] private float _intensityChangeRate = 2.0f;


    [Header("Battery Settings")]
    [SerializeField] private float _maxBattery = 100.0f;
    private float _currentBattery;
    
    [Space(5)]
    [SerializeField] private float _defaultBatteryDrainRate = 5.0f;
    
    [Space(5)]
    [SerializeField] private float _focusedBatteryDrainRate = 15f;


    [Header("Stun Settings")]
    [SerializeField] private LayerMask _stunnableLayers;
    [SerializeField] private float _focusStunRate = 35.0f;

    
    public static event System.Action<FlashlightController, float, float> OnFlashlightControllerChanged; // FlashlightController: newController, float currentBattery, float: maxBattery
    public static event System.Action<FlashlightController, float> OnFlashlightMaxBatteryChanged; // FlashlightController: thisController, float maxBattery.
    public static event System.Action<FlashlightController, float> OnFlashlightBatteryChanged; // FlashlightController: thisController, float currentBattery.



    private void Awake() => _currentBattery = _maxBattery;
    private void Start() => InitializeFlashlight();


    #region InventoryItem Functions

    public override void Initialise(PlayerInventory inventory, InventoryItemDataSO itemData, float[] itemValues)
    {
        base.Initialise(inventory, itemData, itemValues);

        _currentBattery = itemValues != null ? itemValues[0] : 100.0f;
    }

    public override void StartUse() => EnableFocusMode();
    public override void StopUse() => DisableFocusMode();

    public override void StartAltUse() => TryToggleFlashlight();

    public override string GetItemName() => "Flashlight";

    #endregion


    private void Update()
    {
        HandleFlashlightBattery();
        UpdateFlashlightLight();

        if (_isFocused)
        {
            HandleFocusModeDamage();
        }
    }


    /// <summary> Initialize the flashlight and check for the necessary components.</summary>
    private void InitializeFlashlight()
    {
        _flashlightLight = GetComponentInChildren<Light>();
        if (_flashlightLight == null)
        {
            Debug.LogError("No Light component found on the Flashlight: " + this.name + ".");
        }

        // Setup the Flashlight Light.
        _flashlightLight.enabled = false;
        _flashlightLight.spotAngle = _defaultConeAngle;
        _flashlightLight.intensity = _flashlightLight.intensity;

        // Notify Listeners for UI.
        OnFlashlightControllerChanged?.Invoke(this, _currentBattery, _maxBattery);
    }
    public void OnFlashlightEquipped() => OnFlashlightControllerChanged?.Invoke(this, _currentBattery, _maxBattery);
    public void OnFlashlightUnequipped() => OnFlashlightControllerChanged?.Invoke(null, 1.0f, 1.0f);



    #region Enabling/Disabling

    /// <summary> Toggle the flashlight on/off.</summary>
    public void TryToggleFlashlight()
    {
        if (_currentBattery <= 0.0f)
        {
            // We cannot toggle the flashlight (Out of battery).
            return;
        }

        // Toggle the flashlight.
        if (_isOn)
        {
            DisableFlashlight();
        }
        else
        {
            EnableFlashlight();
        }
    }
    public void EnableFlashlight()
    {
        _isOn = true;
        _flashlightLight.enabled = true;
    }
    public void DisableFlashlight()
    {
        _isOn = false;
        _flashlightLight.enabled = false;
    }

#endregion


    private void UpdateFlashlightLight()
    {
        if (_isFocused)
        {
            // Is Focused.
            _flashlightLight.spotAngle = Mathf.Lerp(_flashlightLight.spotAngle, _focusedConeAngle, Time.deltaTime * _angleChangeRate);
            _flashlightLight.intensity = Mathf.Lerp(_flashlightLight.intensity, _focusedIntensity, Time.deltaTime * _intensityChangeRate);

        }
        else
        {
            // Not Focused.
            _flashlightLight.spotAngle = Mathf.Lerp(_flashlightLight.spotAngle, _defaultConeAngle, Time.deltaTime * _angleChangeRate);
            _flashlightLight.intensity = Mathf.Lerp(_flashlightLight.intensity, _defaultIntensity, Time.deltaTime * _intensityChangeRate);
        }
    }


    #region Focus Mode

    public void EnableFocusMode()
    {
        if (_isOn && _currentBattery > 0.0f)
        {
            _isFocused = true;
        }
    }
    public void DisableFocusMode() => _isFocused = false;


    /// <summary> Apply stun to any stunnable entities within our flashlight cone.</summary>
    private void HandleFocusModeDamage()
    {
        List<RaycastHit> coneHits = ConeCastAll(transform.position, transform.forward, _flashlightLight.range, _flashlightLight.spotAngle, _stunnableLayers);

        for (int i = 0; i < coneHits.Count; i++)
        {
            if (coneHits[i].collider.TryGetComponent<FlashlightStunnable>(out FlashlightStunnable stunnableScript))
            {
                stunnableScript.ApplyStun(_focusStunRate * Time.deltaTime);
            }
        }
    }

    // Adapted from: 'https://github.com/walterellisfun/ConeCast/blob/master/ConeCastExtension.cs'.
    private List<RaycastHit> ConeCastAll(Vector3 origin, Vector3 direction, float coneRange, float coneAngle, int layerMask)
    {
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - new Vector3(0.0f, 0.0f, coneRange), coneRange, direction, coneRange);
        List<RaycastHit> coneCastHitList = new List<RaycastHit>();

        for (int i = 0; i < sphereCastHits.Length; i++)
        {
            Vector3 hitPoint = sphereCastHits[i].point;
            Vector3 directionToHit = (hitPoint - origin).normalized;
            float angleToHit = Vector3.Angle(direction, directionToHit);

            if (angleToHit < coneAngle)
            {
                coneCastHitList.Add(sphereCastHits[i]);
            }
        }

        return coneCastHitList;
    }

    #endregion


    #region Battery

    private void HandleFlashlightBattery()
    {
        if (!_isOn)
        {
            return;
        }

        // Decrease our remaining battery with a rate determined by if we are focused or not.
        float drainRate = _isFocused ? _focusedBatteryDrainRate : _defaultBatteryDrainRate;
        _currentBattery -= drainRate * Time.deltaTime;

        if (_currentBattery <= 0.0f)
        {
            // We have ran out of battery.
            DisableFlashlight();
        }

        OnFlashlightBatteryChanged?.Invoke(this, _currentBattery);
    }


    /// <summary> Sets the flashlight battery level and updates the UI.</summary>
    public void SetBatteryLevel(float batteryLevel)
    {
        _currentBattery = Mathf.Clamp(batteryLevel, 0, _maxBattery);
        OnFlashlightBatteryChanged?.Invoke(this, _currentBattery);
    }

    public float GetMaxBattery() => _maxBattery;
    public float GetCurrentBattery() => _currentBattery;
    public bool IsFullyCharged() => _currentBattery == _maxBattery;

    #endregion
}
