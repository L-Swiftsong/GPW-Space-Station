using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Saving;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;

namespace UI
{
    public class LoadSaveUI : MonoBehaviour
    {
        private FileInfo[] _saveGameFiles = null;

        private System.Action<bool> _onSaveCountChangedCallback;
        private System.Action<GameObject, FileInfo> _onLoadButtonPressedCallback;


        [Header("References")]
        [SerializeField] private GameObject _loadSaveUIContainer;

        [Space(5)]
        [SerializeField] private Transform _loadSaveOptionsContainer;
        [SerializeField] private Button _loadSaveGameButtonPrefab;

        [Space(5)]
        [SerializeField] private Button _loadSavesBackButton;
        


        public void SetCallbacks(System.Action<bool> onSaveCountChanged, System.Action<GameObject, FileInfo> onLoadButtonPressed)
        {
            this._onSaveCountChangedCallback = onSaveCountChanged;
            this._onLoadButtonPressedCallback = onLoadButtonPressed;
        }
        public bool HasSaves() => _saveGameFiles != null && _saveGameFiles.Length > 0;


        public void Show()
        {
            _loadSaveUIContainer.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_loadSavesBackButton.gameObject);
        }
        public void Hide()
        {
            _loadSaveUIContainer.SetActive(false);
        }


        public void UpdateSavedGames()
        {
            _saveGameFiles = SaveManager.GetAllSaveFiles(ordered: true);

            if (HasSaves())
            {
                _onSaveCountChangedCallback?.Invoke(true);
                RegenerateLoadSaveUI();
            }
            else
            {
                _onSaveCountChangedCallback?.Invoke(false);
            }
        }
        private void RegenerateLoadSaveUI()
        {
            // Remove the old UI elements.
            for (int i = 0; i < _loadSaveOptionsContainer.childCount; i++)
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
                loadSaveButtonInstance.onClick.AddListener(() => _onLoadButtonPressedCallback?.Invoke(loadSaveButtonInstance.gameObject, fileInfoRef));

                // Set the button's text to match its corresponding save file's name.
                TMP_Text loadSaveButtonText = loadSaveButtonInstance.GetComponentInChildren<TMP_Text>();
                loadSaveButtonText.text = _saveGameFiles[i].Name;
            }

            // Setup navigation.
            if (loadSaveButtons.Count > 1)
            {
                for (int i = 0; i < loadSaveButtons.Count; i++)
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
            else if (loadSaveButtons.Count == 1)
            {
                loadSaveButtons[0].SetupNavigation(selectOnUp: _loadSavesBackButton, selectOnDown: _loadSavesBackButton);
                _loadSavesBackButton.SetupNavigation(selectOnUp: loadSaveButtons[0], selectOnDown: loadSaveButtons[0]);
            }
        }


        public void LoadMostRecentSave() => SaveManager.Instance.LoadMostRecentSave();
        public void LoadSaveFromFile(FileInfo fileInfo) => SaveManager.Instance.LoadGame(fileInfo);
    }
}
