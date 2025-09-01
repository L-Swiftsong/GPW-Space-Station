using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
using SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

namespace UI.Menus
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private SceneField _persistentScene;
        private static bool s_shouldLoadPersistentScene = true;
        [SerializeField] private MainMenuEntryTransition _firstSceneTransition;


        [Header("Main Menu")]
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _mainMenuFirst;
        
        [Space(5)]
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _loadSavesButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsMenuButton;


        [Header("Settings Menu")]
        [SerializeField] private GameObject _settingsMenu;


        [Header("Load Saves Menu")]
        [SerializeField] private LoadSaveUI _loadSavesMenu;


        private void Awake()
        {
            if (s_shouldLoadPersistentScene)
            {
                SceneManager.LoadSceneAsync(_persistentScene, LoadSceneMode.Additive);
                s_shouldLoadPersistentScene = false;
            }
        }
        private void Start()
        {
            Debug.Log("Set Lock State");
            Cursor.lockState = CursorLockMode.None;

            _loadSavesMenu.SetCallbacks(onSaveCountChanged: UpdateMainSaveButtons);
            _loadSavesMenu.UpdateSavedGames();

            // Start with only the main menu enabled.
            _loadSavesMenu.Hide();
            _settingsMenu.SetActive(false);

            _mainMenu.SetActive(true);

            // Start with the 'New Game' button selected.
            EventSystem.current.SetSelectedGameObject(_newGameButton.gameObject);
        }


        public void StartNewGame()
        {
            SaveManager.Instance.NewGame();
            SceneLoader.Instance.PerformTransition(_firstSceneTransition, isNewGameLoad: true);
        }


        private void UpdateMainSaveButtons(bool hasSaves)
        {
            _continueGameButton.interactable = hasSaves;
            _loadSavesButton.interactable = hasSaves;
        }
        public void ContinueGame() => _loadSavesMenu.LoadMostRecentSave();
        public void OpenLoadSavesMenu()
        {
            // Disable the main menu.
            _mainMenu.SetActive(false);

            // Enable the load menu.
            _loadSavesMenu.Show();
        }
        public void CloseLoadSavesMenu()
        {
            // Disable the load menu.
            _loadSavesMenu.Hide();

            // Enable the main menu.
            _mainMenu.SetActive(true);

            // Set the selected button.
            EventSystem.current.SetSelectedGameObject(_loadSavesButton.gameObject);
        }
        

        public void OpenSettingsMenu()
        {
            // Disable the main menu.
            _mainMenu.SetActive(false);

            // Enable the settings menu.
            _settingsMenu.SetActive(true);
        }
        public void CloseSettingsMenu()
        {
            // Disable the settings menu.
            _settingsMenu.SetActive(false);

            // Enable the main menu.
            _mainMenu.SetActive(true);

            // Set the selected button.
            EventSystem.current.SetSelectedGameObject(_settingsMenuButton.gameObject);
        }


        public void ExitToDesktop() => Application.Quit();



        public static void SetEntryFromOtherScene() => s_shouldLoadPersistentScene = false;
    }
}