using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    private Camera _mainCamera;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;

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
        ApplySettings();
    }

    public void ApplySettings()
    {
        // Apply VSync
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 0) == 1 ? 1 : 0;

        // Apply Audio Settings
        SetVolume(VolumeType.MasterVolume, PlayerPrefs.GetFloat(VolumeType.MasterVolume.ToString(), 1f));
        SetVolume(VolumeType.MusicVolume, PlayerPrefs.GetFloat(VolumeType.MusicVolume.ToString(), 1f));
        SetVolume(VolumeType.SFXVolume, PlayerPrefs.GetFloat(VolumeType.SFXVolume.ToString(), 1f));

        // Apply FOV setting to camera
        int savedFOV = PlayerPrefs.GetInt("FOV", 60);
        SetFOV(savedFOV);
    }

    enum VolumeType { MasterVolume, MusicVolume, SFXVolume } // Case-Sensitive (Converted into strings for the PlayerPrefs & AudioMixer)
    private void SetVolume(VolumeType volumeType, float value)
    {
        Debug.Log("Base: " + value);
        Debug.Log("Log10: " + Mathf.Log10(value));
        Debug.Log("Log10 * 20: " + Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(volumeType.ToString(), value);
        audioMixer.SetFloat(volumeType.ToString(), Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
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

    public void SetMasterVolume(float value) => SetVolume(VolumeType.MasterVolume, value);

    public void SetMusicVolume(float value) => SetVolume(VolumeType.MusicVolume, value);

    public void SetSFXVolume(float value) => SetVolume(VolumeType.SFXVolume, value);

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
}
