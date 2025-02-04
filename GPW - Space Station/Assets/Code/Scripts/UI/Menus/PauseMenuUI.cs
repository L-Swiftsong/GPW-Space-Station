using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Saving;
using SceneManagement;

namespace UI.Menus
{
    public class PauseMenuUI : MonoBehaviour
    {
        private bool _isActive = false;


        [Header("References")]
        [SerializeField] private GameObject _container;

        [Space(5)]
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _settingsMenu;

        [Space(5)]
        [SerializeField] private GameObject _pauseMenuFirstSelectedElement;
        [SerializeField] private GameObject _settingsMenuButtonGO;
    

        private void Awake()
        {
            // Start the pause menu hidden.
            _container.SetActive(false);
            _isActive = false;
        }
        private void OnEnable()
        {
            // Subscribe to input events.
            PlayerInput.OnPauseGamePerformed += PlayerInput_OnPauseGamePerformed;
        }
        private void OnDisable()
        {
            // Unsubscribe from input events.
            PlayerInput.OnPauseGamePerformed -= PlayerInput_OnPauseGamePerformed;
            HideWithoutCursorLocking();
        }


        private void PlayerInput_OnPauseGamePerformed()
        {
            if (_isActive)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }


        private void Show()
        {
            Cursor.lockState = CursorLockMode.Confined;

            if (_isActive)
            {
                // We are already active.
                return;
            }

            // Ensure that the pause menu is the open element.
            OpenPauseMenu();

            // Show the UI.
            _isActive = true;
            _container.SetActive(true);

            // Pause the game.
            Time.timeScale = 0.0f;

            // Prevent player input.
            PlayerInput.PreventAllActions(typeof(PauseMenuUI));
        }
        private void Hide()
        {
            Cursor.lockState = CursorLockMode.Locked;
            HideWithoutCursorLocking();
        }

        private void HideWithoutCursorLocking()
        {
            // Hide the UI.
            _isActive = false;
            _container.SetActive(false);

            // Unpause the game.
            Time.timeScale = 1.0f;

            // Allow player input.
            PlayerInput.RemoveAllActionPrevention(typeof(PauseMenuUI));
        }


        #region Button Subscription Functions

        private void OpenPauseMenu()
        {
            // Hide all other menus.
            _settingsMenu.SetActive(false);

            // Show the primary pause menu.
            _pauseMenu.SetActive(true);


            // Set the first selected button.
            EventSystem.current.SetSelectedGameObject(_pauseMenuFirstSelectedElement);
        }
        public void OpenSettingsMenu()
        {
            // Hide the primary pause menu.
            _pauseMenu.SetActive(false);

            // Show the settings menu.
            _settingsMenu.SetActive(true);
        }
        public void CloseSettingsMenu()
        {
            // Hide the settings menu.
            _settingsMenu.SetActive(false);

            // Show the primary pause menu.
            _pauseMenu.SetActive(true);


            // Set the first selected button.
            EventSystem.current.SetSelectedGameObject(_settingsMenuButtonGO.gameObject);
        }

        public void ResumeGame() => Hide();
        public void ReloadLastCheckpoint() => SaveManager.ReloadCheckpointSave();
        public void ExitToMainMenu() => SceneLoader.Instance.ReloadToMainMenu();
        public void ExitToDesktop() => Application.Quit();

        #endregion
    }
}