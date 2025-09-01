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
        private System.Action _onConfirmationQueryOpenedCallback;
        private System.Action _onConfirmationQueryFinishedCallback;


        [Header("References")]
        [SerializeField] private GameObject _loadSaveUIContainer;

        [Space(5)]
        [SerializeField] private Transform _loadSaveOptionsContainer;
        [SerializeField] private LoadOrDeleteSaveButton _loadSaveGameButtonPrefab;

        [Space(5)]
        [SerializeField] private Button _loadSavesBackButton;

        [Space(5)]
        [SerializeField] private ConfirmationUI _confirmationUI;
        [SerializeField] private ScrollRect _scrollRect;



        public void SetCallbacks(System.Action<bool> onSaveCountChanged, System.Action onConfirmationQueryOpenedCallback = null, System.Action onConfirmationQueryFinishedCallback = null)
        {
            this._onSaveCountChangedCallback = onSaveCountChanged;
            this._onConfirmationQueryOpenedCallback = onConfirmationQueryOpenedCallback;
            this._onConfirmationQueryFinishedCallback = onConfirmationQueryFinishedCallback;
        }
        public bool HasSaves() => _saveGameFiles != null && _saveGameFiles.Length > 0;


        public void Show()
        {
            _loadSaveUIContainer.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_loadSavesBackButton.gameObject);
        }
        public void Hide() => _loadSaveUIContainer.SetActive(false);


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
                DeleteOldSaveUI();
            }
        }

        private void DeleteOldSaveUI()
        {
            for (int i = 0; i < _loadSaveOptionsContainer.childCount; i++)
            {
                Destroy(_loadSaveOptionsContainer.GetChild(i).gameObject);
            }
        }
        private void RegenerateLoadSaveUI()
        {
            // Remove the old UI elements.
            DeleteOldSaveUI();

            // Create the new UI elements.
            List<LoadOrDeleteSaveButton> loadSaveButtons = new List<LoadOrDeleteSaveButton>();
            for (int i = 0; i < _saveGameFiles.Length; i++)
            {
                // Instantiate the button instance.
                LoadOrDeleteSaveButton loadSaveButtonInstance = Instantiate(_loadSaveGameButtonPrefab, _loadSaveOptionsContainer);
                loadSaveButtons.Add(loadSaveButtonInstance);

                // Assign this button to load the corresponding save file when clicked.
                System.IO.FileInfo fileInfoRef = _saveGameFiles[i];
                loadSaveButtonInstance.OnLoadSaveCallback += () => LoadSaveFromFile(fileInfoRef);
                loadSaveButtonInstance.OnDeleteSaveCallback += () => DeleteSaveFile(fileInfoRef);

                loadSaveButtonInstance.OnConfirmationStartedCallback += _onConfirmationQueryOpenedCallback;
                loadSaveButtonInstance.OnConfirmationCancelledCallback += _onConfirmationQueryFinishedCallback;


                // Set the button's text to match its corresponding save file's name.
                loadSaveButtonInstance.Setup(_confirmationUI, _scrollRect, fileInfoRef.Name);
            }

            // Setup navigation for the new elements.
            if (loadSaveButtons.Count > 1)
            {
                for (int i = 0; i < loadSaveButtons.Count; i++)
                {
                    if (i == 0)
                    {
                        // This is the first button.
                        loadSaveButtons[i].SetupNavigation(upLeft: _loadSavesBackButton, upRight: _loadSavesBackButton, belowLoadOrDeleteButton: loadSaveButtons[i + 1]);
                    }
                    else if (i == loadSaveButtons.Count - 1)
                    {
                        // This is the last button.
                        loadSaveButtons[i].SetupNavigation(aboveLoadOrDeleteButton: loadSaveButtons[i - 1], downLeft: _loadSavesBackButton, downRight: _loadSavesBackButton);
                        _loadSavesBackButton.UpdateNavigation(selectOnUp: loadSaveButtons[i].GetDefaultSelectable(), selectOnDown: loadSaveButtons[0].GetDefaultSelectable());
                    }
                    else
                    {
                        // This is a middle button.
                        loadSaveButtons[i].SetupNavigation(aboveLoadOrDeleteButton: loadSaveButtons[i - 1], belowLoadOrDeleteButton: loadSaveButtons[i + 1]);
                    }
                }
            }
            else if (loadSaveButtons.Count == 1)
            {
                loadSaveButtons[0].SetupNavigation(upLeft: _loadSavesBackButton, upRight: _loadSavesBackButton, downLeft: _loadSavesBackButton, downRight: _loadSavesBackButton);
                _loadSavesBackButton.UpdateNavigation(selectOnUp: loadSaveButtons[0].GetDefaultSelectable(), selectOnDown: loadSaveButtons[0].GetDefaultSelectable());
            }
        }


        public void RequestAllSaveDeletion()
        {
            _onConfirmationQueryOpenedCallback?.Invoke();
            _confirmationUI.RequestConfirmation("Are you sure you wish to\nDelete All Existing Saves?", onCancelCallback: _onConfirmationQueryFinishedCallback, onConfirmCallback: DeleteAllSaves);
        }


        public void LoadMostRecentSave() => SaveManager.Instance.LoadMostRecentSave();
        public void LoadSaveFromFile(FileInfo fileInfo) => SaveManager.Instance.LoadGame(fileInfo);
        public void DeleteSaveFile(FileInfo fileInfo)
        {
            SaveManager.DeleteSave(fileInfo);
            UpdateSavedGames();
            _onConfirmationQueryFinishedCallback?.Invoke();
        }
        private void DeleteAllSaves()
        {
            SaveManager.DeleteAllSaves();
            UpdateSavedGames();
        }
    }
}
