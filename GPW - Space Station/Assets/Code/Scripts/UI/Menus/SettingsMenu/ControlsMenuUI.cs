using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus.Settings
{
    public class ControlsMenuUI : SettingsSubmenuUI
    {
        [Header("Controls Settings")]
        [SerializeField] private Toggle _toggleCrouchToggle;
        [SerializeField] private Toggle _toggleSprintToggle;
        [SerializeField] private Slider _cameraShakeStrengthSlider;
        [SerializeField] private Slider _cutsceneCameraShakeStrengthSlider;

        [Space(5)]
        [SerializeField] private Toggle _mouseInvertYAxisToggle;
        [SerializeField] private Slider _mouseHorizontalSensitivitySlider;
        [SerializeField] private Slider _mouseVerticalSensitivitySlider;

        [Space(5)]
        [SerializeField] private Toggle _gamepadInvertYAxisToggle;
        [SerializeField] private Slider _gamepadHorizontalSensitivitySlider;
        [SerializeField] private Slider _gamepadVerticalSensitivitySlider;



        protected override void Awake()
        {
            SetupSensitivitySliders();
            base.Awake();
        }


        private void SetupSensitivitySliders()
        {
            // Camera Shake.
            _cameraShakeStrengthSlider.minValue = 0.0f;
            _cameraShakeStrengthSlider.maxValue = 1.0f;

            _cutsceneCameraShakeStrengthSlider.minValue = 0.0f;
            _cutsceneCameraShakeStrengthSlider.maxValue = 1.0f;


            // Mouse.
            _mouseHorizontalSensitivitySlider.minValue = PlayerSettings.MouseSensitivityRange.Min.x;
            _mouseHorizontalSensitivitySlider.maxValue = PlayerSettings.MouseSensitivityRange.Max.x;

            _mouseVerticalSensitivitySlider.minValue = PlayerSettings.MouseSensitivityRange.Min.y;
            _mouseVerticalSensitivitySlider.maxValue = PlayerSettings.MouseSensitivityRange.Max.y;


            // Gamepad.
            _gamepadHorizontalSensitivitySlider.minValue = PlayerSettings.GamepadSensitivityRange.Min.x;
            _gamepadHorizontalSensitivitySlider.maxValue = PlayerSettings.GamepadSensitivityRange.Max.x;

            _gamepadVerticalSensitivitySlider.minValue = PlayerSettings.GamepadSensitivityRange.Min.y;
            _gamepadVerticalSensitivitySlider.maxValue = PlayerSettings.GamepadSensitivityRange.Max.y;
        }
        protected override void SubscribeToUIEvents()
        {
            _toggleCrouchToggle.onValueChanged.AddListener(OnToggleCrouchChanged);
            _toggleSprintToggle.onValueChanged.AddListener(OnToggleSprintChanged);

            _cameraShakeStrengthSlider.onValueChanged.AddListener(OnCameraShakeStrengthChanged);
            _cutsceneCameraShakeStrengthSlider.onValueChanged.AddListener(OnCutsceneCameraShakeStrengthChanged);

            _mouseInvertYAxisToggle.onValueChanged.AddListener(OnMouseInvertYAxisChanged);
            _mouseHorizontalSensitivitySlider.onValueChanged.AddListener(OnMouseHorizontalSensitivityChanged);
            _mouseVerticalSensitivitySlider.onValueChanged.AddListener(OnMouseVerticalSensitivityChanged);

            _gamepadInvertYAxisToggle.onValueChanged.AddListener(OnGamepadInvertYAxisChanged);
            _gamepadHorizontalSensitivitySlider.onValueChanged.AddListener(OnGamepadHorizontalSensitivityChanged);
            _gamepadVerticalSensitivitySlider.onValueChanged.AddListener(OnGamepadVerticalSensitivityChanged);
        }
        protected override void UnsubscribeFromUIEvents()
        {
            _toggleCrouchToggle.onValueChanged.RemoveListener(OnToggleCrouchChanged);
            _toggleSprintToggle.onValueChanged.RemoveListener(OnToggleSprintChanged);

            _cameraShakeStrengthSlider.onValueChanged.RemoveListener(OnCameraShakeStrengthChanged);
            _cutsceneCameraShakeStrengthSlider.onValueChanged.RemoveListener(OnCutsceneCameraShakeStrengthChanged);

            _mouseInvertYAxisToggle.onValueChanged.RemoveListener(OnMouseInvertYAxisChanged);
            _mouseHorizontalSensitivitySlider.onValueChanged.RemoveListener(OnMouseHorizontalSensitivityChanged);
            _mouseVerticalSensitivitySlider.onValueChanged.RemoveListener(OnMouseVerticalSensitivityChanged);

            _gamepadInvertYAxisToggle.onValueChanged.RemoveListener(OnGamepadInvertYAxisChanged);
            _gamepadHorizontalSensitivitySlider.onValueChanged.RemoveListener(OnGamepadHorizontalSensitivityChanged);
            _gamepadVerticalSensitivitySlider.onValueChanged.RemoveListener(OnGamepadVerticalSensitivityChanged);
        }


        public override void UpdateSettings()
        {
            _toggleCrouchToggle.isOn = PlayerSettings.ToggleCrouch;
            _toggleSprintToggle.isOn = PlayerSettings.ToggleSprint;

            _cameraShakeStrengthSlider.value = PlayerSettings.CameraShakeStrength;
            _cutsceneCameraShakeStrengthSlider.value = PlayerSettings.CutsceneCameraShakeStrength;

            _mouseInvertYAxisToggle.isOn = PlayerSettings.MouseInvertY;
            _mouseHorizontalSensitivitySlider.value = PlayerSettings.MouseHorizontalSensititvity;
            _mouseVerticalSensitivitySlider.value = PlayerSettings.MouseVerticalSensititvity;

            _gamepadInvertYAxisToggle.isOn = PlayerSettings.GamepadInvertY;
            _gamepadHorizontalSensitivitySlider.value = PlayerSettings.GamepadHorizontalSensititvity;
            _gamepadVerticalSensitivitySlider.value = PlayerSettings.GamepadVerticalSensititvity;
        }


        #region UI Element Functions

        private void OnToggleCrouchChanged(bool value) => PlayerSettings.ToggleCrouch = value;
        private void OnToggleSprintChanged(bool value) => PlayerSettings.ToggleSprint = value;

        private void OnCameraShakeStrengthChanged(float value) => PlayerSettings.CameraShakeStrength = value;
        private void OnCutsceneCameraShakeStrengthChanged(float value) => PlayerSettings.CutsceneCameraShakeStrength = value;

        private void OnMouseInvertYAxisChanged(bool value) => PlayerSettings.MouseInvertY = value;
        private void OnMouseHorizontalSensitivityChanged(float value) => PlayerSettings.MouseHorizontalSensititvity = Mathf.RoundToInt(value);
        private void OnMouseVerticalSensitivityChanged(float value) => PlayerSettings.MouseVerticalSensititvity = Mathf.RoundToInt(value);

        private void OnGamepadInvertYAxisChanged(bool value) => PlayerSettings.GamepadInvertY = value;
        private void OnGamepadHorizontalSensitivityChanged(float value) => PlayerSettings.GamepadHorizontalSensititvity = Mathf.RoundToInt(value);
        private void OnGamepadVerticalSensitivityChanged(float value) => PlayerSettings.GamepadVerticalSensititvity = Mathf.RoundToInt(value);

        #endregion
    }
}
