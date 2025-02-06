using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UI.Menus
{
    public class SettingsMenuUI : MonoBehaviour
    {
        [Header("Back Button")]
        [SerializeField] private Button _backButton;
        [SerializeField] private UnityEngine.Events.UnityEvent _onBackButtonPressed;


        [Header("Controls Settings")]
        [SerializeField] private Toggle _toggleCrouchToggle;
        [SerializeField] private Toggle _toggleSprintToggle;

        [Space(5)]
        [SerializeField] private Toggle _mouseInvertYAxisToggle;
        [SerializeField] private Slider _mouseHorizontalSensitivitySlider;
        [SerializeField] private Slider _mouseVerticalSensitivitySlider;

        [Space(5)]
        [SerializeField] private Toggle _gamepadInvertYAxisToggle;
        [SerializeField] private Slider _gamepadHorizontalSensitivitySlider;
        [SerializeField] private Slider _gamepadVerticalSensitivitySlider;


        [Header("Display Settings")]
        [SerializeField] private TMP_Dropdown _displayModeDropdown;
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private TMP_Dropdown _fovDropdown;
    
        [Space(5)]
        [SerializeField] private Toggle _vSyncToggle;
        [SerializeField] private Toggle _showFPSToggle;
    
    
        [Header("Audio Settings")]
        [SerializeField] private AudioMixer _audioMixer;

        [Space(10)]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
    

        [Header("Audio Preview")]
        [SerializeField] private AudioSource _masterPreviewAudioSource;
        [SerializeField] private AudioSource _musicPreviewAudioSource;
        [SerializeField] private AudioSource _sfxPreviewAudioSource;


        [Header("Fps Display")]
        [SerializeField] private GameObject _fpsDisplay;


        private void Awake()
        {
            SetupUISliders();
            SetupUIEvents();

        }
        private void OnDestroy()
        {
            
        }

        /// <summary> Setup the minimum & maximum values of the UI sliders we have the data for.</summary>
        private void SetupUISliders()
        {
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
        /// <summary> Set up listeners for settings UI.</summary>
        private void SetupUIEvents()
        {
            // Back Button.
            _backButton.onClick.AddListener(OnBackButtonPressed);

            // Controls Settings.
            _toggleCrouchToggle.onValueChanged.AddListener(OnToggleCrouchChanged);
            _toggleSprintToggle.onValueChanged.AddListener(OnToggleSprintChanged);

            _mouseInvertYAxisToggle.onValueChanged.AddListener(OnMouseInvertYAxisChanged);
            _mouseHorizontalSensitivitySlider.onValueChanged.AddListener(OnMouseHorizontalSensitivityChanged);
            _mouseVerticalSensitivitySlider.onValueChanged.AddListener(OnMouseVerticalSensitivityChanged);

            _gamepadInvertYAxisToggle.onValueChanged.AddListener(OnGamepadInvertYAxisChanged);
            _gamepadHorizontalSensitivitySlider.onValueChanged.AddListener(OnGamepadHorizontalSensitivityChanged);
            _gamepadVerticalSensitivitySlider.onValueChanged.AddListener(OnGamepadVerticalSensitivityChanged);


            // Display Settings.
            _displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
            _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            _vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            _showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);
            _fovDropdown.onValueChanged.AddListener(OnFOVChanged);


            // Audio Settings.
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }


        private void OnEnable()
        {
            if (SettingsManager.Instance == null)
            {
                // The settings manager has not been initialised.
                return;
            }

            // Update our displayed values to match what they are currently set to.
            UpdateSettings();
        }
        private void OnDisable()
        {
            // Save Player Settings on close.
            PlayerSettings.SaveSettingsToPlayerPrefs();
        }


        /// <summary> Load settings from PlayerPrefs.</summary>
        private void UpdateSettings()
        {
            // Update the Player Settings.
            PlayerSettings.UpdateSettingsFromPlayerPrefs();

            // Update Sliders (Controls).
            _toggleCrouchToggle.isOn = PlayerSettings.ToggleCrouch;
            _toggleSprintToggle.isOn = PlayerSettings.ToggleSprint;

            _mouseInvertYAxisToggle.isOn = PlayerSettings.MouseInvertY;
            _mouseHorizontalSensitivitySlider.value = PlayerSettings.MouseHorizontalSensititvity;
            _mouseVerticalSensitivitySlider.value = PlayerSettings.MouseVerticalSensititvity;

            _gamepadInvertYAxisToggle.isOn = PlayerSettings.GamepadInvertY;
            _gamepadHorizontalSensitivitySlider.value = PlayerSettings.GamepadHorizontalSensititvity;
            _gamepadVerticalSensitivitySlider.value = PlayerSettings.GamepadVerticalSensititvity;


            // Update Sliders (Display).
            InitializeDisplayModeOptions();
            InitializeResolutionOptions();

            _vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;
            _showFPSToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

            // Lead FOV setting and set the dropdown value
            int savedFOV = PlayerPrefs.GetInt("FOV", 60);
            _fovDropdown.value = savedFOV == 60 ? 0 : (savedFOV == 75 ? 1 : 2);  // 0 = 60, 1 = 75, 2 = 90
            _fovDropdown.RefreshShownValue();

            // FPS display 
            if (_fpsDisplay != null)
            {
                _fpsDisplay.SetActive(_showFPSToggle.isOn);
            }


            // Update Sliders (Audio).
            _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        }

        private void InitializeDisplayModeOptions()
        {
            _displayModeDropdown.options.Clear();
            _displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
            _displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
            _displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Borderless Window"));

            FullScreenMode currentMode = Screen.fullScreenMode;
            int index = 0;
            switch (currentMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    index = 0;
                    break;
                case FullScreenMode.Windowed:
                    index = 1;
                    break;
                case FullScreenMode.FullScreenWindow:
                    index = 2;
                    break;
            }
            _displayModeDropdown.value = index;
            _displayModeDropdown.RefreshShownValue();
        }

        private void InitializeResolutionOptions()
        {
            _resolutionDropdown.options.Clear();
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("2560 x 1440"));
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1920 x 1080"));
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1600 x 900"));
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1280 x 720"));

            int index = 0;
            if (Screen.width == 2560 && Screen.height == 1440)
                index = 0;
            else if (Screen.width == 1920 && Screen.height == 1080)
                index = 1;
            else if (Screen.width == 1600 && Screen.height == 900)
                index = 2;
            else if (Screen.width == 1280 && Screen.height == 720)
                index = 3;
            else
                index = 1; // default to 1920x1080

            _resolutionDropdown.value = index;
            _resolutionDropdown.RefreshShownValue();
        }


        #region UI Element Functions

        // Control Settings.
        private void OnToggleCrouchChanged(bool value) => PlayerSettings.ToggleCrouch = value;
        private void OnToggleSprintChanged(bool value) => PlayerSettings.ToggleSprint = value;

        private void OnMouseInvertYAxisChanged(bool value) => PlayerSettings.MouseInvertY = value;
        private void OnMouseHorizontalSensitivityChanged(float value) => PlayerSettings.MouseHorizontalSensititvity = Mathf.RoundToInt(value);
        private void OnMouseVerticalSensitivityChanged(float value) => PlayerSettings.MouseVerticalSensititvity = Mathf.RoundToInt(value);

        private void OnGamepadInvertYAxisChanged(bool value) => PlayerSettings.GamepadInvertY = value;
        private void OnGamepadHorizontalSensitivityChanged(float value) => PlayerSettings.GamepadHorizontalSensititvity = Mathf.RoundToInt(value);
        private void OnGamepadVerticalSensitivityChanged(float value) => PlayerSettings.GamepadVerticalSensititvity = Mathf.RoundToInt(value);


        // Display Settings.
        private void OnDisplayModeChanged(int index) => SettingsManager.Instance.SetDisplayMode(index);
        private void OnResolutionChanged(int index) => SettingsManager.Instance.SetResolution(index);
        private void OnVSyncChanged(bool value) => SettingsManager.Instance.SetVSync(value);

        // Updated OnFOVChanged method to handle 75 FOV option.
        public void OnFOVChanged(int index)
        {
            int selectedFOV = index == 0 ? 60 : (index == 1 ? 75 : 90);
            SettingsManager.Instance.SetFOV(selectedFOV);
        }
        private void OnShowFPSChanged(bool value)
        {
            SettingsManager.Instance.SetShowFPS(value);

            if (_fpsDisplay != null)
            {
                _fpsDisplay.SetActive(value);
            }
        }


        // Volume Settings.
        private void OnMasterVolumeChanged(float value) => SettingsManager.Instance.SetMasterVolume(value);
        private void OnMusicVolumeChanged(float value) => SettingsManager.Instance.SetMusicVolume(value);
        private void OnSFXVolumeChanged(float value) => SettingsManager.Instance.SetSFXVolume(value);


        // Other.
        private void OnBackButtonPressed() => _onBackButtonPressed?.Invoke();

        #endregion


        /// <summary> Overrides the navigation of the Back Button for the new 'selectOnUp' and 'selectOnDown' selectables.</summary>
        public void SetupBackButton(Selectable selectOnUp, Selectable selectOnDown) => _backButton.SetupNavigation(selectOnUp: selectOnUp, selectOnDown: selectOnDown);
    }
}