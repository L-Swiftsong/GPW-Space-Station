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
        [SerializeField] private ConfirmationUI _confirmationUI;
        private GameObject _lastUsedButtonGO;


        private void OnConfirmationCancelled()
        {
            ShowSelf();
            EventSystem.current.SetSelectedGameObject(_lastUsedButtonGO);
        }
        private void ShowSelf() => this.gameObject.SetActive(true);
        private void HideSelf() => this.gameObject.SetActive(false);


        #region Button Subscription Functions

        public void OpenSavesMenu()
        {

        }
        public void CloseSavesMenu()
        {

        }

        public void ReloadLastCheckpoint(GameObject button)
        {
            _lastUsedButtonGO = button;

            _confirmationUI.RequestConfirmation("Reload",
                onCancelCallback: OnConfirmationCancelled,
                onConfirmCallback: () => SaveManager.ReloadCheckpointSave());

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