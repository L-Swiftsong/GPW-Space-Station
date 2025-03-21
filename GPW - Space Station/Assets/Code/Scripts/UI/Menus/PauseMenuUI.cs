using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SceneManagement;

namespace UI.Menus
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private ConfirmationUI _confirmationUI;
        private GameObject _lastUsedButtonGO;

        [Space(5)]
        [SerializeField] private LoadSaveUI _loadSaveUI;
        [SerializeField] private Button _loadLastSaveButton;
        [SerializeField] private Button _loadSavesButton;


        private void Awake() => _loadSaveUI.SetCallbacks(onSaveCountChanged: UpdateMainSaveButtons, onLoadButtonPressed: LoadSpecificSave);
        private void OnEnable() => _loadSaveUI.UpdateSavedGames();



        private void OnConfirmationCancelled()
        {
            ShowSelf();
            EventSystem.current.SetSelectedGameObject(_lastUsedButtonGO);
        }
        private void ShowSelf() => this.gameObject.SetActive(true);
        private void HideSelf() => this.gameObject.SetActive(false);


        private void UpdateMainSaveButtons(bool hasSaves)
        {
            _loadLastSaveButton.interactable = hasSaves;
            _loadSavesButton.interactable = hasSaves;
        }


        #region Button Subscription Functions

        public void OpenSavesMenu(GameObject button)
        {
            _lastUsedButtonGO = button;
            _loadSaveUI.Show();
            HideSelf();
        }
        public void CloseSavesMenu()
        {
            _loadSaveUI.Hide();
            ShowSelf();
            EventSystem.current.SetSelectedGameObject(_lastUsedButtonGO);
        }


        public void LoadLastSave(GameObject button)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation("Load Last Save",
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => _loadSaveUI.LoadMostRecentSave());

            HideSelf();
        }
        public void LoadSpecificSave(GameObject button, System.IO.FileInfo fileInfo)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation("Load Save",
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => _loadSaveUI.LoadSaveFromFile(fileInfo));

            HideSelf();
        }
        public void ExitToMainMenu(GameObject button)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation("Quit",
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => SceneLoader.Instance.ReloadToMainMenu());

            HideSelf();
        }
        public void ExitToDesktop(GameObject button)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation("Quit",
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => Application.Quit());

            HideSelf();
        }

        #endregion
    }
}