using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Items.Flashlight
{
    public class FlashlightController : MonoBehaviour
    {
        private bool _isOn = false;
        private bool _isFocused = false;


        [Header("Light Settings")]
        [SerializeField] private Light _flashlightLight;
        [SerializeField] private float _defaultConeAngle = 45.0f;
        [SerializeField] private float _defaultIntensity = 2.5f;
        
        [Space(5)]
        [SerializeField] private float _focusedConeAngle = 25.0f;
        [SerializeField] private float _focusedIntensity = 4.0f;

        [Space(5)]
        [SerializeField] private float _angleChangeRate = 2.0f;
        [SerializeField] private float _intensityChangeRate = 2.0f;


        [Header("Battery Settings")]
        [Tooltip("How long the flashlight's battery lasts if continuously left on")]
            [SerializeField] private float _defaultBatteryLifetime = 120.0f;
        [Tooltip("How long the flashlight's battery lasts if continuously focused")]
            [SerializeField] private float _focusedBatteryLifetime = 10.0f;

        private float m_currentBattery;
        private float _currentBattery
        {
            get => m_currentBattery;
            set
            {
                m_currentBattery = value;
                OnFlashlightBatteryChanged?.Invoke(m_currentBattery);
            }
        }


        [Header("Stun Settings")]
        [SerializeField] private LayerMask _stunnableLayers;
        [SerializeField] private float _focusStunRate = 35.0f;

    
        public static event System.Action<float> OnObtainedFlashlight; // float: currentBattery.
        public static event System.Action OnLostFlashlight;
        public static event System.Action<float> OnFlashlightBatteryChanged; // float: currentBattery.


        private void OnEnable()
        {
            SubscribeToInput();
            OnObtainedFlashlight?.Invoke(_currentBattery);

            // The flashlight will only be enabled when we obtain it, and we set its active state to false here so that it always starts with the light off.
            SetActiveState(false);
        }
        private void OnDisable()
        {
            UnsubscribeFromInput();
            OnLostFlashlight?.Invoke();
        }


        private void SubscribeToInput()
        {
            PlayerInput.OnToggleFlashlightPerformed += TryToggleActiveState;
            PlayerInput.OnFocusFlashlightStarted += StartFocus;
            PlayerInput.OnFocusFlashlightCancelled += StopFocus;
        }
        private void UnsubscribeFromInput()
        {
            PlayerInput.OnToggleFlashlightPerformed -= TryToggleActiveState;
            PlayerInput.OnFocusFlashlightStarted -= StartFocus;
            PlayerInput.OnFocusFlashlightCancelled -= StopFocus;
        }


        /// <summary> Toggle the flashlight's active state IF we are able to do so.</summary>
        private void TryToggleActiveState()
        {
            if (_currentBattery <= 0.0f)
            {
                // We cannot toggle the flashlight (Out of battery).
                return;
            }

            SetActiveState(!_isOn);
        }
        private void SetActiveState(bool newActiveState)
        {
            _isOn = newActiveState;
            _flashlightLight.enabled = newActiveState;
        }

        private void StartFocus()
        {
            if (!_isOn)
            {
                // We cannot start focusing (Flashlight is disabled).
                return;
            }
            if (_currentBattery <= 0.0f)
            {
                // We cannot start focusing (Out of battery).
                return;
            }

            _isFocused = true;
        }
        private void StopFocus() => _isFocused = false;


        private void Update()
        {
            HandleFlashlightBattery();
            UpdateFlashlightLight();

            if (_isFocused)
            {
                HandleFocusModeDamage();
            }
        }


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
            float drainRate = 100.0f / (_isFocused ? _focusedBatteryLifetime : _defaultBatteryLifetime);
            _currentBattery -= drainRate * Time.deltaTime;

            if (_currentBattery <= 0.0f)
            {
                // We have ran out of battery.
                SetActiveState(false);
            }
        }


        /// <summary> Sets the flashlight's current battery level.</summary>
        public void SetBatteryLevel(float batteryLevel) => _currentBattery = batteryLevel;
        public float GetCurrentBattery() => _currentBattery;

        #endregion
    }
}