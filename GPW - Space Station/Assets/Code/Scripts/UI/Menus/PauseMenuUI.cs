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
        [SerializeField] private GameObject _mainButtonsContainer;

        [Space(5)]
        [SerializeField] private ConfirmationUI _confirmationUI;
        private GameObject _lastUsedButtonGO;

        [Space(5)]
        [SerializeField] private LoadSaveUI _loadSaveUI;
        [SerializeField] private Button _loadLastSaveButton;
        [SerializeField] private Button _loadSavesButton;


        private void Awake() => _loadSaveUI.SetCallbacks(onSaveCountChanged: UpdateMainSaveButtons, onConfirmationQueryOpenedCallback: HideUIForConfirmation, onConfirmationQueryFinishedCallback: ShowLoadSaveUIAfterConfirmation);
        private void OnEnable()
        {
            _loadSaveUI.UpdateSavedGames();
            _loadSaveUI.gameObject.SetActive(false);
            _confirmationUI.gameObject.SetActive(false);
        }



        private void HideUIForConfirmation()
        {
            _mainButtonsContainer.SetActive(false);
            _loadSaveUI.gameObject.SetActive(false);
        }
        private void OnConfirmationCancelled()
        {
            ShowSelf();
            EventSystem.current.SetSelectedGameObject(_lastUsedButtonGO);
        }
        private void ShowLoadSaveUIAfterConfirmation()
        {
            _mainButtonsContainer.SetActive(false);
            _confirmationUI.gameObject.SetActive(false);

            _loadSaveUI.gameObject.SetActive(true);
        }
        private void ShowSelf() => _mainButtonsContainer.SetActive(true);
        private void HideSelf() => _mainButtonsContainer.SetActive(false);


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

            HideUIForConfirmation();

            _confirmationUI.RequestConfirmation(CreateConfirmationString("Load the Last Save"),
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => _loadSaveUI.LoadMostRecentSave());

            HideSelf();
        }
        public void LoadSpecificSave(GameObject button, System.IO.FileInfo fileInfo)
        {
            _lastUsedButtonGO = button;

            HideUIForConfirmation();

            _confirmationUI.RequestConfirmation(CreateConfirmationString("Load this Save"),
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => _loadSaveUI.LoadSaveFromFile(fileInfo));

            HideSelf();
        }
        public void ExitToMainMenu(GameObject button)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation(CreateConfirmationString("Quit to the Main Menu"),
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => SceneLoader.Instance.ReloadToMainMenu());

            HideSelf();
        }
        public void ExitToDesktop(GameObject button)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation(CreateConfirmationString("Quit to the Desktop"),
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => Application.Quit());

            HideSelf();
        }
        private string CreateConfirmationString(string query) => string.Concat("Are you sure you want to\n", query, "?");

        #endregion
    }
}