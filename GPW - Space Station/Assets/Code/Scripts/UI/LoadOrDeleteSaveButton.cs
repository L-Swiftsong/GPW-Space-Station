using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class LoadOrDeleteSaveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button _loadSaveButton;
        [SerializeField] private TMP_Text _loadSaveButtonText;
        private string _loadSaveQuery;

        [SerializeField] private Button _deleteSaveButton;
        private string _deleteSaveQuery;

        
        private ConfirmationUI _confirmationUI;
        private ScrollRect _parentScrollrect;


        public event System.Action OnLoadSaveCallback;
        public event System.Action OnDeleteSaveCallback;

        public event System.Action OnConfirmationStartedCallback;
        public event System.Action OnConfirmationCancelledCallback;


        public LoadOrDeleteSaveButton Setup(ConfirmationUI confirmationUI, ScrollRect parentScrollRect, string fileName)
        {
            this._loadSaveButtonText.text = fileName;
            this._loadSaveQuery = string.Concat("Load Save\n", fileName, "?");
            this._deleteSaveQuery = string.Concat("Delete Save\n", fileName, "?");

            this._confirmationUI = confirmationUI;
            this._parentScrollrect = parentScrollRect;

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


        private bool _wasSelectedObjectLastFrame;
        private bool _isHovered;
        private void Update()
        {
            // Replace with detecting when the buttons are selected.
            if (EventSystem.current.currentSelectedGameObject == _loadSaveButton.gameObject || EventSystem.current.currentSelectedGameObject == _deleteSaveButton.gameObject)
            {
                if (_wasSelectedObjectLastFrame)
                    return;
                _wasSelectedObjectLastFrame = true;

                if (_isHovered)
                    return;

                RectTransform rectTransform = this.transform as RectTransform;
                _parentScrollrect.content.localPosition = GetSnapToPositionToBringChildIntoView(_parentScrollrect, rectTransform) + new Vector2(0.0f, rectTransform.sizeDelta.y / 2.0f);
            }
            else
            {
                _wasSelectedObjectLastFrame = false;
            }
        }
        // Source: 'https://stackoverflow.com/a/50191835'.
        private Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;

            Vector2 result = new Vector2(
                -(viewportLocalPosition.x + childLocalPosition.x),
                -(viewportLocalPosition.y + childLocalPosition.y));

            // Ensure we don't change our content's position on an axis that is locked.
            if (!instance.horizontal) result.x = instance.content.localPosition.x;
            if (!instance.vertical) result.y = instance.content.localPosition.y;

            Debug.Log(child.GetComponentInChildren<TMPro.TMP_Text>().text + ": " + child.localPosition);
            return result;
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
        

        public void OnPointerEnter(PointerEventData eventData) => _isHovered = true;
        public void OnPointerExit(PointerEventData eventData) => _isHovered = false;
    }
}
