using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
/*
 * FlashLightController.cs
 * 
 * This script handles the flashlight functionality. It allows the player to toggle the flashlight,
 * enter focus mode to narrow the beam and increase intensity, and drains the battery + deals damage to enemies 
 */

public class FlashLightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] private GameObject flashlightLightObject; 
    [SerializeField] private float normalSpotAngle = 30f;     
    [SerializeField] private float decreaseSpotAngle = 20f;    
    [SerializeField] private float increaseRate = 2f;          
    [SerializeField] private float intensityMultiplier = 1.5f; 
    [SerializeField] private float maxBattery = 100f;          
    private float _flashlightBattery;                          

    [Header("Battery Drain Settings")]
    [SerializeField] private float batteryDrainRate = 5f;      
    [SerializeField] private float focusBatteryDrainRate = 15f;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI batteryTextUI;    

    private Light _flashlight;         
    private bool _isOn = false;        
    private bool _hasFlashlight = false;
    private float _defaultIntensity;   

    /// <summary>
    /// Public getter for the flashlight battery level.
    /// </summary>
    public float FlashlightBattery => _flashlightBattery;

    private void Start()
    {
        _flashlightBattery = maxBattery;
        InitializeFlashlight();
    }
    
    private void Update()
    {
        if (_hasFlashlight)
        {
            HandleFlashlightToggle();
            HandleFocusMode();
        }
    }

    /// <summary>
    /// Initialize the flashlight and check for the necessary components.
    /// </summary>
    private void InitializeFlashlight()
    {
        if (flashlightLightObject != null)
        {
            _flashlight = flashlightLightObject.GetComponent<Light>();

            if (_flashlight == null)
            {
                Debug.LogError("No Light component found on the flashlightLightObject.");
                return;
            }

            _flashlight.enabled = false;                      
            _flashlight.spotAngle = normalSpotAngle;          
            _defaultIntensity = _flashlight.intensity;        
        }
        else
        {
            Debug.LogError("FlashlightLightObject is not assigned.");
        }

        UpdateBatteryUI(); 
    }

    /// <summary>
    /// Toggle the flashlight on/off when the player presses the F key.
    /// </summary>
    private void HandleFlashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.F) && _flashlight != null && _flashlightBattery > 0)
        {
            _isOn = !_isOn;
            _flashlight.enabled = _isOn; // Enable or disable the flashlight
        }
    }

    /// <summary>
    /// Handle the focus mode, adjusting beam angle and intensity + applying battery drain.
    /// </summary>
    private void HandleFocusMode()
    {
        if (_isOn && _flashlight != null)
        {
            //  battery drain when the flashlight is on
            _flashlightBattery -= batteryDrainRate * Time.deltaTime;
            _flashlightBattery = Mathf.Clamp(_flashlightBattery, 0, maxBattery);

            if (_flashlightBattery <= 0)
            {
                _flashlightBattery = 0;
                _flashlight.enabled = false;
                _isOn = false;
            }

            if (Input.GetMouseButton(1) && _flashlightBattery > 0) // Focus mode (right mouse button)
            {
                _flashlight.spotAngle = Mathf.Lerp(_flashlight.spotAngle, decreaseSpotAngle, Time.deltaTime * increaseRate);
                _flashlight.intensity = Mathf.Lerp(_flashlight.intensity, _defaultIntensity * intensityMultiplier, Time.deltaTime * increaseRate);
                _flashlightBattery -= focusBatteryDrainRate * Time.deltaTime; // Increased drain for focus mode
                CheckForMimic(); // Check for Mimic in focus mode
            }
            else
            {
                _flashlight.spotAngle = Mathf.Lerp(_flashlight.spotAngle, normalSpotAngle, Time.deltaTime * increaseRate);
                _flashlight.intensity = Mathf.Lerp(_flashlight.intensity, _defaultIntensity, Time.deltaTime * increaseRate);
            }

            UpdateBatteryUI(); 
        }
    }

    /// <summary>
    /// Update the battery level in the UI.
    /// </summary>
    private void UpdateBatteryUI()
    {
        if (batteryTextUI != null)
        {
            batteryTextUI.text = "Battery: " + Mathf.RoundToInt(_flashlightBattery) + "%";
        }
    }

    /// <summary>
    /// a raycast to check if the flashlight is hitting the mimic and apply damage to its shield if so.
    /// </summary>
    private void CheckForMimic()
    {
        RaycastHit hit;
        if (Physics.Raycast(flashlightLightObject.transform.position, flashlightLightObject.transform.forward, out hit))
        {
            if (hit.collider.CompareTag("Mimic"))
            {
                MimicShieldController mimicShield = hit.collider.GetComponent<MimicShieldController>();
                mimicShield?.StartTakingDamage(); // Apply damage to 
            }
        }
        else
        {
            StopDamageMimic(); // Stop damaging if mimic isn't hit
        }
    }

    /// <summary>
    /// Stop damaging mimic shield
    /// </summary>
    private void StopDamageMimic()
    {
        MimicShieldController[] mimics = FindObjectsOfType<MimicShieldController>();
        foreach (var mimic in mimics)
        {
            mimic.StopTakingDamage();
        }
    }

    /// <summary>
    /// Called when the player picks up the flashlight flashlight 
    /// </summary>
    public void PickUpFlashlight()
    {
        _hasFlashlight = true;
        UpdateBatteryUI();
        Debug.Log("Flashlight picked up!");
    }

    /// <summary>
    /// Disables the flashlight and prevents it from being used.
    /// </summary>
    public void DisableFlashlight()
    {
        _hasFlashlight = false;
        _flashlight.enabled = false;
        _isOn = false;
    }

    /// <summary>
    /// Enables the flashlight, allowing it to be used.
    /// </summary>
    public void EnableFlashlight()
    {
        _hasFlashlight = true;
    }

    /// <summary>
    /// Sets the flashlight battery level and updates the UI.
    /// </summary>
    public void SetBatteryLevel(float batteryLevel)
    {
        _flashlightBattery = Mathf.Clamp(batteryLevel, 0, maxBattery);
        UpdateBatteryUI();
    }

    public void SetBatteryUI(TextMeshProUGUI batteryUI)
    {
        batteryTextUI = batteryUI;
        UpdateBatteryUI();
    }

    /// <summary>
    /// Public method to check if the player has the flashlight.
    /// </summary>
    public bool HasFlashlight()
    {
        return _hasFlashlight;
    }
}
