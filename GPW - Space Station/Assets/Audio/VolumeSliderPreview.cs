using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSliderPreview : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private AudioSource previewAudioSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedParameterName;
    [SerializeField] private string playerPrefKey;

    private Slider slider;

    private const float minSliderValue = 1f;
    private const float maxSliderValue = 100f;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);

        // Load saved value
        float savedValue = PlayerPrefs.GetFloat(playerPrefKey, maxSliderValue);
        slider.value = savedValue;

        // Set the volume
        SetAudioMixerVolume(savedValue);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayPreviewSound();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopPreviewSound();
    }

    private void OnValueChanged(float value)
    {
        PlayerPrefs.SetFloat(playerPrefKey, value);
        PlayerPrefs.Save();

        SetAudioMixerVolume(value);
    }

    private void SetAudioMixerVolume(float sliderValue)
    {
        //slider value
        float normalizedValue = Mathf.Clamp((sliderValue - minSliderValue) / (maxSliderValue - minSliderValue), 0.0001f, 1f);

        // Calculate volume in decibels
        float volumeInDecibels = Mathf.Log10(normalizedValue) * 20;

        // Set the AudioMixer volume
        audioMixer.SetFloat(exposedParameterName, volumeInDecibels);
    }

    private void PlayPreviewSound()
    {
        if (previewAudioSource != null && !previewAudioSource.isPlaying)
        {
            previewAudioSource.Play();
        }
    }

    private void StopPreviewSound()
    {
        if (previewAudioSource != null && previewAudioSource.isPlaying)
        {
            previewAudioSource.Stop();
        }
    }
}
