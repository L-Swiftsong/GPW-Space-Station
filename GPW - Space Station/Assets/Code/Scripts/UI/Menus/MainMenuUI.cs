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
        [SerializeField] private ForegroundSceneTransition _firstSceneTransition;


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
        [SerializeField] private GameObject _loadSavesMenu;
        [SerializeField] private Button _loadSavesBackButton;

        [Space(5)]
        [SerializeField] private Transform _loadSaveOptionsContainer;
        [SerializeField] private Button _loadSaveGameButtonPrefab;
        private System.IO.FileInfo[] _saveGameFiles = null;


        private void Awake() => SceneManager.LoadSceneAsync(_persistentScene, LoadSceneMode.Additive);
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;

            // Update and populate the saved games list & UI.
            UpdateSavedGames();


            // Start with only the main menu enabled.
            _loadSavesMenu.SetActive(false);
            _settingsMenu.SetActive(false);

            _mainMenu.SetActive(true);

            // Start with the 'New Game' button selected.
            EventSystem.current.SetSelectedGameObject(_newGameButton.gameObject);
        }


        private void UpdateSavedGames()
        {
            _saveGameFiles = SaveManager.GetAllSaveFiles(ordered: true);

            if (_saveGameFiles.Length <= 0)
            {
                // We have no save files.
                _continueGameButton.interactable = false;
                _loadSavesButton.interactable = false;
            }
            else
            {
                // We have save files.
                _continueGameButton.interactable = true;
                _loadSavesButton.interactable = true;

                RegenerateLoadSaveUI();
            }
        }
        private void RegenerateLoadSaveUI()
        {
            // Remove the old UI elements.
            for(int i = 0; i < _loadSaveOptionsContainer.childCount; i++)
            {
                // Unsubscribe from the button's events.
                Button loadSaveButton = _loadSaveOptionsContainer.GetChild(i).GetComponent<Button>();
                loadSaveButton.onClick.RemoveAllListeners();

                // Destroy the button.
                Destroy(loadSaveButton.gameObject);
            }

            // Add in the new UI elements.
            List<Button> loadSaveButtons = new List<Button>();
            for (int i = 0; i < _saveGameFiles.Length; i++)
            {
                // Instantiate the button instance.
                Button loadSaveButtonInstance = Instantiate(_loadSaveGameButtonPrefab, _loadSaveOptionsContainer);
                loadSaveButtons.Add(loadSaveButtonInstance);

                // Assign this button to load the corresponding save file when clicked.
                System.IO.FileInfo fileInfoRef = _saveGameFiles[i];
                loadSaveButtonInstance.onClick.AddListener(() => LoadSave(fileInfoRef));

                // Set the button's text to match its corresponding save file's name.
                TMP_Text loadSaveButtonText = loadSaveButtonInstance.GetComponentInChildren<TMP_Text>();
                loadSaveButtonText.text = _saveGameFiles[i].Name;
            }

            // Setup navigation.
            for(int i = 0; i < loadSaveButtons.Count; i++)
            {
                if (i == 0)
                {
                    // This is the first button.
                    loadSaveButtons[i].SetupNavigation(selectOnUp: _loadSavesBackButton, selectOnDown: loadSaveButtons[i + 1]);
                }
                else if (i == loadSaveButtons.Count - 1)
                {
                    // This is the last button.
                    loadSaveButtons[i].SetupNavigation(selectOnUp: loadSaveButtons[i - 1], selectOnDown: _loadSavesBackButton);
                    _loadSavesBackButton.SetupNavigation(selectOnUp: loadSaveButtons[i], selectOnDown: loadSaveButtons[0]);
                }
                else
                {
                    // This is a middle button.
                    loadSaveButtons[i].SetupNavigation(selectOnUp: loadSaveButtons[i - 1], selectOnDown: loadSaveButtons[i + 1]);
                }
            }
        }


        public void LoadSave(System.IO.FileInfo fileInfo)
        {
            Debug.Log("Loading Save: '" + fileInfo.Name + "' at path: " + fileInfo.FullName);
            SaveManager.StartLoadFromFileInfo(fileInfo);
        }


        public void ContinueFromMostRecentSave()
        {
            Debug.Log("Continue from Most Recent Save");
            SaveManager.StartLoadFromFileInfo(_saveGameFiles[0]);
        }
        public void StartNewGame()
        {
            Debug.Log("Start New Game");
            SceneLoader.Instance.PerformTransition(_firstSceneTransition);
        }


        public void OpenLoadSavesMenu()
        {
            // Disable the main menu.
            _mainMenu.SetActive(false);

            // Enable the load menu.
            _loadSavesMenu.SetActive(true);

            // Set the selected button.
            EventSystem.current.SetSelectedGameObject(_loadSavesBackButton.gameObject);
        }
        public void CloseLoadSavesMenu()
        {
            // Disable the load menu.
            _loadSavesMenu.SetActive(false);

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
    }
}