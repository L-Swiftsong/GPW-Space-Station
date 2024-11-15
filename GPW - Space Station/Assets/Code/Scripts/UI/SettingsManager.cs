using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    private Camera _mainCamera;
    private PlayerController playerController;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Find and store reference camera
            _mainCamera = Camera.main;

            ApplySettings();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Locates the main camera and player controller after each scene has loaded
        _mainCamera = Camera.main;
        playerController = FindObjectOfType<PlayerController>();
        ApplySettings();
    }

    public void ApplySettings()
    {
        // Apply VSync
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 0) == 1 ? 1 : 0;

        // Apply Audio Settings
        SetVolume("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetVolume("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 1f));
        SetVolume("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 1f));

        // Apply settings to player controller
        ApplySettingsToPlayerController();

        // Apply FOV setting to camera
        int savedFOV = PlayerPrefs.GetInt("FOV", 60);
        SetFOV(savedFOV);

        float sensitivity = PlayerPrefs.GetFloat("LookSensitivity", 50.0f); //default Sensitivity at 50 / 100
        if (playerController != null)
        {
            playerController.SetLookSensitivity(sensitivity);
        }
    }

    private void ApplySettingsToPlayerController()
    {
        if (playerController != null)
        {
            // Apply toggle crouch setting
            bool toggleCrouch = PlayerPrefs.GetInt("ToggleCrouch", 0) == 1;
            playerController.SetToggleCrouch(toggleCrouch);

            // Apply invert Y-axis setting
            bool invertYAxis = PlayerPrefs.GetInt("InvertYAxis", 0) == 1;
            playerController.SetInvertYAxis(invertYAxis);


            // Apply other settings (e.g., sprint toggle, sensitivity)
            bool toggleSprint = PlayerPrefs.GetInt("ToggleSprint", 0) == 1;
            playerController.SetToggleSprint(toggleSprint);

            float lookSensitivity = PlayerPrefs.GetFloat("LookSensitivity", 50.0f); // set Default speed to 50
            playerController.SetLookSensitivity(lookSensitivity);


        }
    }

    private void SetVolume(string parameter, float value)
    {
        PlayerPrefs.SetFloat(parameter, value);
        audioMixer.SetFloat(parameter, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public void SetFOV(int fov)
    {
        PlayerPrefs.SetInt("FOV", fov);
        PlayerPrefs.Save();

        if (_mainCamera != null)
        {
            _mainCamera.fieldOfView = fov;
        }
    }

    public void SetMasterVolume(float value) => SetVolume("MasterVolume", value);

    public void SetMusicVolume(float value) => SetVolume("MusicVolume", value);

    public void SetSFXVolume(float value) => SetVolume("SFXVolume", value);

    public void SetVSync(bool value)
    {
        PlayerPrefs.SetInt("VSync", value ? 1 : 0);
        QualitySettings.vSyncCount = value ? 1 : 0;
        PlayerPrefs.Save();
    }

    public void SetShowFPS(bool value)
    {
        PlayerPrefs.SetInt("ShowFPS", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDisplayMode(int index)
    {
        FullScreenMode mode = index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.Windowed,
            2 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.ExclusiveFullScreen
        };

        Screen.fullScreenMode = mode;
        PlayerPrefs.SetInt("DisplayMode", index);
        PlayerPrefs.Save();
    }

    public void SetResolution(int index) // screen resolution
    {
        (int width, int height) = index switch
        {
            0 => (2560, 1440),
            1 => (1920, 1080),
            2 => (1600, 900),
            3 => (1280, 720),
            _ => (1920, 1080)
        };

        Screen.SetResolution(width, height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    public void SetInvertYAxis(bool value)
    {
        // Save the preference
        PlayerPrefs.SetInt("InvertYAxis", value ? 1 : 0);
        PlayerPrefs.Save();

        // Apply to the PlayerController
        if (playerController != null)
        {
            playerController.SetInvertYAxis(value);
        }
    }

    public void SetToggleCrouch(bool value)
    {
        // Save preference
        PlayerPrefs.SetInt("ToggleCrouch", value ? 1 : 0);
        PlayerPrefs.Save();

        // Apply preference to the PlayerController
        if (playerController != null)
        {
            playerController.SetToggleCrouch(value);
        }
    }

    public void SetToggleSprint(bool value)
    {
        // Save preference
        PlayerPrefs.SetInt("ToggleSprint", value ? 1 : 0);
        PlayerPrefs.Save();

        // Apply preference to the PlayerController
        if (playerController != null)
        {
            playerController.SetToggleSprint(value);
        }
    }

    public void SetLookSensitivity(float value)
    {
        // Save preference
        PlayerPrefs.SetFloat("LookSensitivity", value);
        PlayerPrefs.Save();

        // Apply sensitivity to the PlayerController
        if (playerController != null)
        {
            float normalizedSensitivity = value / 10f;
            playerController.SetLookSensitivity(normalizedSensitivity);
        }
    }



}
