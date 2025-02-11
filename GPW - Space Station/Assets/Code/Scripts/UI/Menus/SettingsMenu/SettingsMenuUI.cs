using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Menus.Settings
{
    public class SettingsMenuUI : MonoBehaviour
    {
        [Header("Settings Menu")]
        [SerializeField] private GameObject _settingsTitleText;
        [SerializeField] private Transform _buttonsContainer;
        [SerializeField] private List<SettingsSubmenuUI> _submenuList;



        private void Awake()
        {
            foreach(SettingsSubmenuUI submenu in _submenuList)
            {
                submenu.Bind(this);
                submenu.gameObject.SetActive(false);
            }

            _buttonsContainer.gameObject.SetActive(true);
            _settingsTitleText.SetActive(true);
        }
        private void OnEnable()
        {
            if (SettingsManager.Instance == null)
            {
                // The settings manager has not been initialised.
                return;
            }

            // Select the first button.
            EventSystem.current.SetSelectedGameObject(_buttonsContainer.GetChild(0).gameObject);

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

            // Update Submenus (Currently done in each submenu).
        }


        public void OpenSubmenu(SettingsSubmenuUI submenu)
        {
            // Disable the Buttons & Title Text.
            _buttonsContainer.gameObject.SetActive(false);
            _settingsTitleText.SetActive(false);

            // Disable all Submenus.
            for (int i = 0; i < _submenuList.Count; ++i)
            {
                _submenuList[i].gameObject.SetActive(false);
            }

            // Enable the selected submenu.
            submenu.gameObject.SetActive(true);
        }
        public void CloseSubmenu(Selectable selectedButton)
        {
            // Disable all Submenus.
            for (int i = 0; i < _submenuList.Count; ++i)
            {
                _submenuList[i].gameObject.SetActive(false);
            }

            // Enable the buttons & Title Text.
            _buttonsContainer.gameObject.SetActive(true);
            _settingsTitleText.SetActive(true);

            // Select the proper button.
            EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
        }
    }
}