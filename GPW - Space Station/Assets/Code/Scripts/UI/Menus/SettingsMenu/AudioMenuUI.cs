using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace UI.Menus.Settings
{
    public class AudioMenuUI : SettingsSubmenuUI
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;


        protected override void SubscribeToUIEvents()
        {
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        protected override void UnsubscribeFromUIEvents()
        {
            _masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }


        public override void UpdateSettings()
        {
            _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        }


        #region UI Element Functions

        private void OnMasterVolumeChanged(float value) => SettingsManager.Instance.SetMasterVolume(value);
        private void OnMusicVolumeChanged(float value) => SettingsManager.Instance.SetMusicVolume(value);
        private void OnSFXVolumeChanged(float value) => SettingsManager.Instance.SetSFXVolume(value);

        #endregion
    }
}
