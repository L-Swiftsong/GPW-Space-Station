using System.Collections.Generic;
using UnityEngine;
using Items.Flashlight;
using Items.Keycards;
using Items.Healing;

namespace Items
{
    public class PlayerInventory : MonoBehaviour
    {

        [Header("Flashlight")]
        [SerializeField] private FlashlightController _flashlightController;
        private bool _hasFlashlight = false;


        [Header("Keycards")]
        [SerializeField] private KeycardDecoder _keycardDecoder;
        private bool _hasKeycardDecoder = false;


        [Header("Medkits")]
        [SerializeField] private Medkit _medkit;


        private void Awake()
        {
            // Start without any items.
            RemoveFlashlight();
            //RemoveKeycardDecoder();
            AddKeycardDecoder(0);
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

        #endregion
    }
}