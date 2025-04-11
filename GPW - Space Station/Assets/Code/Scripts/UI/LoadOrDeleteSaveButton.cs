using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class LoadOrDeleteSaveButton : MonoBehaviour
    {
        [SerializeField] private Button _loadSaveButton;
        [SerializeField] private TMP_Text _loadSaveButtonText;
        private string _loadSaveQuery;

        [SerializeField] private Button _deleteSaveButton;
        private string _deleteSaveQuery;

        
        private ConfirmationUI _confirmationUI;


        public event System.Action OnLoadSaveCallback;
        public event System.Action OnDeleteSaveCallback;

        public event System.Action OnConfirmationStartedCallback;
        public event System.Action OnConfirmationCancelledCallback;

        public LoadOrDeleteSaveButton Setup(ConfirmationUI confirmationUI, string fileName)
        {
            this._loadSaveButtonText.text = fileName;
            this._loadSaveQuery = string.Concat("Load Save\n", fileName, "?");
            this._deleteSaveQuery = string.Concat("Delete Save\n", fileName, "?");

            this._confirmationUI = confirmationUI;

            // Fluent Interface.
            return this;
        }
        public LoadOrDeleteSaveButton SetupNavigation(LoadOrDeleteSaveButton aboveLoadOrDeleteButton, Button downLeft, Button downRight) => SetupNavigation(aboveLoadOrDeleteButton._loadSaveButton, aboveLoadOrDeleteButton._deleteSaveButton, downLeft, downRight);
        public LoadOrDeleteSaveButton SetupNavigation(Button upLeft, Button upRight, LoadOrDeleteSaveButton belowLoadOrDeleteButton) => SetupNavigation(upLeft, upRight, belowLoadOrDeleteButton._loadSaveButton, belowLoadOrDeleteButton._deleteSaveButton);
        public LoadOrDeleteSaveButton SetupNavigation(LoadOrDeleteSaveButton aboveLoadOrDeleteButton, LoadOrDeleteSaveButton belowLoadOrDeleteButton) => SetupNavigation(aboveLoadOrDeleteButton._loadSaveButton, aboveLoadOrDeleteButton._deleteSaveButton, belowLoadOrDeleteButton._loadSaveButton, belowLoadOrDeleteButton._deleteSaveButton);
        public LoadOrDeleteSaveButton SetupNavigation(Button upLeft, Button upRight, Button downLeft, Button downRight)
        {
            // Setup button navigation, maintaining the left/right options.
            _loadSaveButton.SetupNavigation(upLeft, downLeft, _loadSaveButton.navigation.selectOnLeft, _loadSaveButton.navigation.selectOnRight);
            _deleteSaveButton.SetupNavigation(upRight, downRight, _deleteSaveButton.navigation.selectOnLeft, _deleteSaveButton.navigation.selectOnRight);

            // Fluent Interface.
            return this;
        }


        public void LoadSave()
        {
            OnConfirmationStartedCallback?.Invoke();
            _confirmationUI.RequestConfirmation(_loadSaveQuery, onCancelCallback: OnConfirmationCancelled, onConfirmCallback: OnLoadSaveCallback);
        }
        public void DeleteSave()
        {
            OnConfirmationStartedCallback?.Invoke();
            _confirmationUI.RequestConfirmation(_deleteSaveQuery, onCancelCallback: OnConfirmationCancelled, onConfirmCallback: OnDeleteSaveCallback);
        }

        private void OnConfirmationCancelled()
        {
            EventSystem.current.SetSelectedGameObject(_loadSaveButton.gameObject);
            OnConfirmationCancelledCallback?.Invoke();
        }


        public Selectable GetDefaultSelectable() => _loadSaveButton;
    }
}
