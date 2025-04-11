using System.Collections.Generic;
using UnityEngine;
using Items.Flashlight;
using Items.Keycards;
using Items.Healing;
using Items.KeyItem;

namespace Items
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Flashlight")]
        [SerializeField] private FlashlightController _flashlightController;

        #if UNITY_EDITOR
        [SerializeField] private bool _startWithFlashlightInEditor = true;
        #endif

        private bool _hasFlashlight = false;


        [Header("Keycards")]
        [SerializeField] private KeycardDecoder _keycardDecoder;
        private bool _hasKeycardDecoder = false;


        [Header("Medkits")]
        [SerializeField] private Medkit _medkit;


        [Header("KeyItems")]
        [SerializeField] private KeyItemManager _keyItemManager;



        private void Awake()
        {
            // Set Starting Items.
            #if UNITY_EDITOR
            if (_startWithFlashlightInEditor)
            {
                AddFlashlight(100.0f);
            }
            #endif

            if (!_hasKeycardDecoder)
            {
                AddKeycardDecoder(0);
            }
        }


        public void SetHasObtainedFlashlight(bool hasFlashlight, float flashlightBattery)
        {
            if (hasFlashlight)
            {
                AddFlashlight(flashlightBattery);
            }
            else
            {
                RemoveFlashlight();
            }
        }
        public void SetHasObtainedKeycardDecoder(bool hasDecoder, int securityLevel)
        {
            if (hasDecoder)
            {
                AddKeycardDecoder(securityLevel);
            }
            else
            {
                RemoveKeycardDecoder();
            }
        }


        #region Flashlight

        public void AddFlashlight(float currentBattery)
        {
            // Enable the flashlight.
            _hasFlashlight = true;
            _flashlightController.gameObject.SetActive(true);


            // Set the flashlight's battery level
            _flashlightController.SetBatteryLevel(currentBattery);
        }
        public void RemoveFlashlight()
        {
            // Disable the flashlight.
            _hasFlashlight = false;
            _flashlightController.gameObject.SetActive(false);
        }
        public float GetFlashlightBattery() => _flashlightController.GetCurrentBattery();
        public bool HasFlashlight() => _hasFlashlight;

        public void LoadFlashlightActiveState(bool activeState) => _flashlightController.LoadActiveState(activeState);
        public bool GetFlashlightActiveState() => _flashlightController.GetActiveState();


        #endregion


        #region Keycard Decoder

        public void AddKeycardDecoder(int securityLevel)
        {
            _keycardDecoder.SetSecurityLevel(securityLevel, allowReduction: true);

            _hasKeycardDecoder = true;
            _keycardDecoder.gameObject.SetActive(true);
        }
        public void RemoveKeycardDecoder()
        {
            _hasKeycardDecoder = false;
            _keycardDecoder.gameObject.SetActive(false);
        }

        public KeycardDecoder GetKeycardDecoder() => _keycardDecoder;
        public int GetDecoderSecurityLevel() => _keycardDecoder.GetSecurityLevel();
        public bool HasKeycardDecoder() => _hasKeycardDecoder;

        #endregion


        #region Medkits

        public int AddMedkits(int numberToAdd) => _medkit.AddMedkits(numberToAdd);
        public void SetMedkits(int newCount) => _medkit.SetMedkits(newCount);
        public void RemoveMedkits(int numberToRemove) => _medkit.RemoveMedkits(numberToRemove);
        public int GetMedkitCount() => _medkit.GetCurrentCount();

        #endregion
    }
}