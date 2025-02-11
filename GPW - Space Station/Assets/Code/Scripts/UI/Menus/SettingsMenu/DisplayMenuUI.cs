using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Menus.Settings
{
    public class DisplayMenuUI : SettingsSubmenuUI
    {
        [Header("Display Settings")]
        [SerializeField] private TMP_Dropdown _displayModeDropdown;
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private TMP_Dropdown _fovDropdown;

        [Space(5)]
        [SerializeField] private Toggle _vSyncToggle;
        [SerializeField] private Toggle _showFPSToggle;


        protected override void Awake()
        {
            base.Awake();
            InitializeDisplayModeOptions();
            InitializeResolutionOptions();
        }

        protected override void SubscribeToUIEvents()
        {
            _displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
            _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            _vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            _showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);
            _fovDropdown.onValueChanged.AddListener(OnFOVChanged);
        }
        protected override void UnsubscribeFromUIEvents()
        {
            _displayModeDropdown.onValueChanged.RemoveListener(OnDisplayModeChanged);
            _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            _vSyncToggle.onValueChanged.RemoveListener(OnVSyncChanged);
            _showFPSToggle.onValueChanged.RemoveListener(OnShowFPSChanged);
            _fovDropdown.onValueChanged.RemoveListener(OnFOVChanged);
        }
        private void InitializeDisplayModeOptions()
        {
            _displayModeDropdown.options.Clear();
            _displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
            _displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
            _displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("Borderless Window"));

            UpdateDisplayModeOptions();
        }
        private void InitializeResolutionOptions()
        {
            _resolutionDropdown.options.Clear();
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("2560 x 1440"));
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1920 x 1080"));
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1600 x 900"));
            _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1280 x 720"));

            UpdateResolutionDropdownOptions();
        }


        public override void UpdateSettings()
        {
            UpdateDisplayModeOptions();
            UpdateResolutionDropdownOptions();

            _vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;
            _showFPSToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

            // Lead FOV setting and set the dropdown value
            int savedFOV = PlayerPrefs.GetInt("FOV", 60);
            _fovDropdown.value = savedFOV == 60 ? 0 : (savedFOV == 75 ? 1 : 2);  // 0 = 60, 1 = 75, 2 = 90
            _fovDropdown.RefreshShownValue();

            // FPS display 
            FPSDisplay.SetEnabled(_showFPSToggle.isOn);
        }
        private void UpdateDisplayModeOptions()
        {
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
        private void UpdateResolutionDropdownOptions()
        {
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

        private void OnDisplayModeChanged(int index) => SettingsManager.Instance.SetDisplayMode(index);
        private void OnResolutionChanged(int index) => SettingsManager.Instance.SetResolution(index);
        private void OnVSyncChanged(bool value) => SettingsManager.Instance.SetVSync(value);

        public void OnFOVChanged(int index)
        {
            int selectedFOV = index == 0 ? 60 : (index == 1 ? 75 : 90);
            SettingsManager.Instance.SetFOV(selectedFOV);
        }
        private void OnShowFPSChanged(bool value)
        {
            SettingsManager.Instance.SetShowFPS(value);
            FPSDisplay.SetEnabled(value);
        }

        #endregion
    }
}
