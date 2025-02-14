using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI.Menus.Settings
{
    public abstract class SettingsSubmenuUI : MonoBehaviour
    {
        protected SettingsMenuUI SettingsMenu;

        [SerializeField] private Selectable _firstSelectedElement;
        [SerializeField] private Selectable _linkedButton;

        public void Bind(SettingsMenuUI settingsMenu) => this.SettingsMenu = settingsMenu;


        protected virtual void Awake() => SubscribeToUIEvents();
        protected virtual void OnDestroy() => UnsubscribeFromUIEvents();

        protected abstract void SubscribeToUIEvents();
        protected abstract void UnsubscribeFromUIEvents();


        protected virtual void OnEnable()
        {
            // Select the proper element.
            EventSystem.current.SetSelectedGameObject(_firstSelectedElement.gameObject);

            UpdateSettings();
        }
        public abstract void UpdateSettings();


        protected virtual void SaveChanges() { }
        protected virtual void DiscardChanges() { }

        public void SaveButtonPressed()
        {
            SaveChanges();
            Debug.Log(SettingsMenu);
            Debug.Log(_linkedButton);
            SettingsMenu.CloseSubmenu(_linkedButton);
        }
        public void DiscardButtonPressed()
        {
            DiscardChanges();
            SettingsMenu.CloseSubmenu(_linkedButton);
        }
    }
}
