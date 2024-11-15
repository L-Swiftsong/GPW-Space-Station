using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Settings UI Elements")]
    [SerializeField] private Toggle toggleCrouchToggle;
    [SerializeField] private Toggle toggleSprintToggle;
    [SerializeField] private Toggle invertYAxisToggle;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown FOVSetting;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Toggle showFPSToggle;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider lookSensitivitySlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Preview")]
    [SerializeField] private AudioSource masterPreviewAudioSource;
    [SerializeField] private AudioSource musicPreviewAudioSource;
    [SerializeField] private AudioSource sfxPreviewAudioSource;

    [Header("Fps Display")]
    [SerializeField] private GameObject fpsDisplay;

    private void Awake()
    {
        // Set up listeners for settings UI
        toggleCrouchToggle.onValueChanged.AddListener(OnToggleCrouchChanged);
        toggleSprintToggle.onValueChanged.AddListener(OnToggleSprintChanged);
        invertYAxisToggle.onValueChanged.AddListener(OnInvertYAxisChanged);
        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        lookSensitivitySlider.onValueChanged.AddListener(OnLookSensitivityChanged);
        FOVSetting.onValueChanged.AddListener(OnFOVChanged);
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Load settings from PlayerPrefs
        toggleCrouchToggle.isOn = PlayerPrefs.GetInt("ToggleCrouch", 0) == 1;
        toggleSprintToggle.isOn = PlayerPrefs.GetInt("ToggleSprint", 0) == 1;
        invertYAxisToggle.isOn = PlayerPrefs.GetInt("InvertYAxis", 0) == 1;
        vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;
        showFPSToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

        float savedSensitivity = PlayerPrefs.GetFloat("LookSensitivity", 50.0f); // Default to 50 if not saved
        lookSensitivitySlider.value = savedSensitivity;

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        masterVolumeSlider.value = masterVolume;

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        musicVolumeSlider.value = musicVolume;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        sfxVolumeSlider.value = sfxVolume;

        InitializeDisplayModeOptions();
        InitializeResolutionOptions();

        // Lead FOV setting and set the dropdown value
        int savedFOV = PlayerPrefs.GetInt("FOV", 60);
        FOVSetting.value = savedFOV == 60 ? 0 : 1;

        // FPS display 
        if (fpsDisplay != null)
        {
            fpsDisplay.SetActive(showFPSToggle.isOn);
        }
    }

    private void InitializeDisplayModeOptions()
    {
        displayModeDropdown.options.Clear();
        displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
        displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
        displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Borderless Window"));

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
        displayModeDropdown.value = index;
        displayModeDropdown.RefreshShownValue();
    }

    private void InitializeResolutionOptions()
    {
        resolutionDropdown.options.Clear();
        resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("2560 x 1440"));
        resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1920 x 1080"));
        resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1600 x 900"));
        resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1280 x 720"));

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

        resolutionDropdown.value = index;
        resolutionDropdown.RefreshShownValue();
    }

    public void OnFOVChanged(int index)
    {
        int selectedFOV = index == 0 ? 60 : 90;
        SettingsManager.Instance.SetFOV(selectedFOV);
    }

    // Event handlers for settings
    private void OnToggleCrouchChanged(bool value)
    {
        SettingsManager.Instance.SetToggleCrouch(value);
    }

    private void OnToggleSprintChanged(bool value)
    {
        SettingsManager.Instance.SetToggleSprint(value);
    }

    private void OnInvertYAxisChanged(bool value)
    {
        SettingsManager.Instance.SetInvertYAxis(value);
    }

    private void OnVSyncChanged(bool value)
    {
        SettingsManager.Instance.SetVSync(value);
    }

    private void OnShowFPSChanged(bool value)
    {
        SettingsManager.Instance.SetShowFPS(value);

        if (fpsDisplay != null)
        {
            fpsDisplay.SetActive(value);
        }
    }

    private void OnDisplayModeChanged(int index)
    {
        SettingsManager.Instance.SetDisplayMode(index);
    }

    private void OnResolutionChanged(int index)
    {
        SettingsManager.Instance.SetResolution(index);
    }

    private void OnLookSensitivityChanged(float value)
    {
        SettingsManager.Instance.SetLookSensitivity(value);
    }

    private void OnMasterVolumeChanged(float value)
    {
        SettingsManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SettingsManager.Instance.SetSFXVolume(value);
    }
}
