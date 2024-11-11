using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlashlightUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _container;
        [SerializeField] private ProgressBar _flashlightBatteryBar;
        private FlashLightController _currentFlashlightController;


        private void Awake() => _container.SetActive(false);
        private void OnEnable()
        {
            FlashLightController.OnFlashlightControllerChanged += FlashLightController_OnFlashlightControllerChanged;
            FlashLightController.OnFlashlightMaxBatteryChanged += FlashLightController_OnFlashlightMaxBatteryChanged;
            FlashLightController.OnFlashlightBatteryChanged += FlashLightController_OnFlashlightBatteryChanged;
        }
        private void OnDisable()
        {
            FlashLightController.OnFlashlightControllerChanged -= FlashLightController_OnFlashlightControllerChanged;
            FlashLightController.OnFlashlightMaxBatteryChanged -= FlashLightController_OnFlashlightMaxBatteryChanged;
            FlashLightController.OnFlashlightBatteryChanged -= FlashLightController_OnFlashlightBatteryChanged;
        }


        private void FlashLightController_OnFlashlightControllerChanged(FlashLightController newController, float currentBattery, float maxBattery)
        {
            if (newController == null)
            {
                // We no longer have a flashlight equipped.
                _container.SetActive(false);
                return;
            }

            _container.SetActive(true);

            // Cache the FlashlightController for comparison later.
            _currentFlashlightController = newController;

            // Set the current & maximum values of the battery progress bar.
            _flashlightBatteryBar.SetValues(current: currentBattery, min: 0.0f, max: maxBattery);
        }
        private void FlashLightController_OnFlashlightMaxBatteryChanged(FlashLightController controller, float maxBattery)
        {
            if (controller != _currentFlashlightController)
            {
                // The updating controller isn't the player's currently equipped flashlight.
                return;
            }

            // Update the maximum value of the battery progress bar.
            _flashlightBatteryBar.SetMaximumValue(maxBattery);
        }
        private void FlashLightController_OnFlashlightBatteryChanged(FlashLightController controller, float currentBattery)
        {
            if (controller != _currentFlashlightController)
            {
                // The updating controller isn't the player's currently equipped flashlight.
                return;
            }

            // Update the maximum value of the battery progress bar.
            _flashlightBatteryBar.SetCurrentValue(currentBattery);
        }
    }
}