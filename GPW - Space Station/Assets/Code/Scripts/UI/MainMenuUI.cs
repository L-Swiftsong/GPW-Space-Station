using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
using SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private SceneField _persistentScene;
        [SerializeField] private ForegroundSceneTransition _firstSceneTransition;


        [Header("Menu References")]
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _settingsMenu;
        [SerializeField] private GameObject _loadSavesMenu;

        [Header("Button References")]
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _loadSavesButton;


        [Header("Saving/Loading")]
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
            EnableMenu(_mainMenu);
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
            foreach(Transform child in _loadSaveOptionsContainer)
            {
                // Unsubscribe from the button's events.
                Button loadSaveButton = child.GetComponent<Button>();
                loadSaveButton.onClick.RemoveAllListeners();

                // Destroy the button.
                Destroy(child.gameObject);
            }

            // Add in the new UI elements.
            for (int i = 0; i < _saveGameFiles.Length; i++)
            {
                // Instantiate the button instance.
                Button loadSaveButtonInstance = Instantiate(_loadSaveGameButtonPrefab, _loadSaveOptionsContainer);

                // Assign this button to load the corresponding save file when clicked.
                System.IO.FileInfo fileInfoRef = _saveGameFiles[i];
                loadSaveButtonInstance.onClick.AddListener(() => LoadSave(fileInfoRef));

                // Set the button's text to match its corresponding save file's name.
                TMP_Text loadSaveButtonText = loadSaveButtonInstance.GetComponentInChildren<TMP_Text>();
                loadSaveButtonText.text = _saveGameFiles[i].Name;
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
        

        public void OpenLoadSavesMenu() => EnableMenu(_loadSavesMenu);
        public void OpenSettingsMenu() => EnableMenu(_settingsMenu);
        public void ReturnToMainMenu() => EnableMenu(_mainMenu);

        private void EnableMenu(GameObject menuToEnable)
        {
            // Disable active menus.
            _mainMenu.SetActive(false);
            _loadSavesMenu.SetActive(false);
            _settingsMenu.SetActive(false);

            // Enable desired menu.
            menuToEnable.SetActive(true);
        }


        public void ExitToDesktop() => Application.Quit();
    }
}