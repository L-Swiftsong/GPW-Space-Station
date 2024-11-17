using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Back Button")]
    [SerializeField] private Button _backButton;
    [SerializeField] private UnityEngine.Events.UnityEvent _onBackButtonPressed;


    [Header("Controls Settings")]
    [SerializeField] private GameObject _controlsContainer;

    [Space(10)]
    [SerializeField] private Toggle _toggleCrouchToggle;
    [SerializeField] private Toggle _toggleSprintToggle;

    [Space(5)]
    [SerializeField] private Toggle _invertYAxisToggle;
    [SerializeField] private Slider _lookSensitivitySlider;


    [Header("Display Settings")]
    [SerializeField] private GameObject _displayContainer;

    [Space(10)]
    [SerializeField] private TMP_Dropdown _displayModeDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _fovDropdown;
    
    [Space(5)]
    [SerializeField] private Toggle _vSyncToggle;
    [SerializeField] private Toggle _showFPSToggle;
    
    
    [Header("Audio Settings")]
    [SerializeField] private GameObject _audioContainer;

    [Space(5)]
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
        SetupUIEvents();
    }

    /// <summary> Set up listeners for settings UI.</summary>
    private void SetupUIEvents()
    {
        // Back Button.
        _backButton.onClick.AddListener(OnBackButtonPressed);

        // Controls Settings.
        _toggleCrouchToggle.onValueChanged.AddListener(OnToggleCrouchChanged);
        _toggleSprintToggle.onValueChanged.AddListener(OnToggleSprintChanged);
        _invertYAxisToggle.onValueChanged.AddListener(OnInvertYAxisChanged);
        _lookSensitivitySlider.onValueChanged.AddListener(OnLookSensitivityChanged);

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

        // Ensure that we are selecting the first selected element.
        EventSystem.current.SetSelectedGameObject(_backButton.gameObject);

        // Update our displayed values to match what they are currently set to.
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Load settings from PlayerPrefs
        _toggleCrouchToggle.isOn = PlayerPrefs.GetInt("ToggleCrouch", 0) == 1;
        _toggleSprintToggle.isOn = PlayerPrefs.GetInt("ToggleSprint", 0) == 1;
        _invertYAxisToggle.isOn = PlayerPrefs.GetInt("InvertYAxis", 0) == 1;
        _vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;
        _showFPSToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

        float savedSensitivity = PlayerPrefs.GetFloat("LookSensitivity", 50.0f); // Default to 50 if not saved
        _lookSensitivitySlider.value = savedSensitivity;

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        _masterVolumeSlider.value = masterVolume;

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        _musicVolumeSlider.value = musicVolume;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        _sfxVolumeSlider.value = sfxVolume;

        InitializeDisplayModeOptions();
        InitializeResolutionOptions();

        // Lead FOV setting and set the dropdown value
        int savedFOV = PlayerPrefs.GetInt("FOV", 60);
        _fovDropdown.value = savedFOV == 60 ? 0 : (savedFOV == 75 ? 1 : 2);  // 0 = 60, 1 = 75, 2 = 90
        _fovDropdown.RefreshShownValue();

        // FPS display 
        if (_fpsDisplay != null)
        {
            _fpsDisplay.SetActive(_showFPSToggle.isOn);
        }
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

    // Event handlers for settings
    private void OnToggleCrouchChanged(bool value) => SettingsManager.Instance.SetToggleCrouch(value);
    private void OnToggleSprintChanged(bool value) => SettingsManager.Instance.SetToggleSprint(value);
    private void OnInvertYAxisChanged(bool value) => SettingsManager.Instance.SetInvertYAxis(value);
    private void OnLookSensitivityChanged(float value) => SettingsManager.Instance.SetLookSensitivity(value);


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


    private void OnMasterVolumeChanged(float value) => SettingsManager.Instance.SetMasterVolume(value);
    private void OnMusicVolumeChanged(float value) => SettingsManager.Instance.SetMusicVolume(value);
    private void OnSFXVolumeChanged(float value) => SettingsManager.Instance.SetSFXVolume(value);


    private void OnBackButtonPressed() => _onBackButtonPressed?.Invoke();

    #endregion
}
