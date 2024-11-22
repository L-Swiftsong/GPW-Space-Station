using UnityEngine;
using Items.Flashlight;

namespace UI
{
    public class FlashlightUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _container;
        [SerializeField] private ProgressBar _batteryProgressBar;


        private void Awake() => Hide();
        private void OnEnable()
        {
            FlashlightController.OnObtainedFlashlight += UpdateCurrentBattery;
            FlashlightController.OnLostFlashlight += Hide;
            FlashlightController.OnFlashlightBatteryChanged += UpdateCurrentBattery;
        }
        private void OnDisable()
        {
            FlashlightController.OnObtainedFlashlight -= UpdateCurrentBattery;
            FlashlightController.OnLostFlashlight -= Hide;
            FlashlightController.OnFlashlightBatteryChanged -= UpdateCurrentBattery;
        }


        private void UpdateCurrentBattery(float currentBattery)
        {
            // Show the UI.
            Show();

            // Update the progress bar to show our current battery.
            _batteryProgressBar.SetCurrentValue(currentBattery);
        }
        private void Show() => _container.SetActive(true);
        private void Hide() => _container.SetActive(false);
    }
}