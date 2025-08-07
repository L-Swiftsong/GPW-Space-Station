using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Interaction;
using UI.Icons;

namespace UI.Popups
{
    public abstract class PopupElement : MonoBehaviour
    {
        [SerializeField] private RectTransform _contentsContainer;
        [SerializeField] private Vector2 _contentsPadding = new Vector2(5.0f, 5.0f); // Padding in each direction.
        [SerializeField] private bool _dontUpdateContentsRootPosition = false;


        [Header("Alpha")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Space(5)]
        [SerializeField, Range(0.0f, 0.5f)] private float _fadeDuration = 0.1f;
        [SerializeField, Range(0.0f, 0.5f)] private float _showDuration = 0.2f;
        private bool _isReady;


        [Header("Contents")]
        [SerializeField] private TMP_Text _popupText;
        private PopupTextData _popupTextData;


        [Header("Background")]
        [SerializeField] private RectTransform _backgroundRoot;


        [Header("Deactivation")]
        [SerializeField, ReadOnly] private float _maxLifetime;
        [SerializeField, ReadOnly] private float _currentLifetime;


        [Header("Interaction Disabling")]
        private IInteractable _linkedInteractable;


        protected bool ShouldShow;

        protected bool IsDisabled;
        protected Action OnDisableCallback;


        protected void OnEnable()
        {
            _canvasGroup.alpha = 0.0f;
            ShouldShow = true;
            _isReady = false;
            IsDisabled = false;

            // Ensure we have the correct sprite assets in use.
            PlayerInput.OnInputDeviceChanged += OnInputDeviceChanged;
        }
        protected void OnDisable()
        {
            if (_linkedInteractable != null)
            {
                _linkedInteractable.OnSuccessfulInteraction -= Deactivate;
                _linkedInteractable.OnFailedInteraction -= Deactivate;
            }

            PlayerInput.OnInputDeviceChanged -= OnInputDeviceChanged;
        }

        protected virtual void Update()
        {
            if (!_isReady)
            {
                return;
            }

            if (IsDisabled)
            {
                HandleCanvasAlpha(false);
                if (_canvasGroup.alpha <= 0.0f)
                {
                    OnDisableCallback?.Invoke();
                }

                return;
            }


            if (CheckForDeactivation())
            {
                Deactivate();
            }

            HandleCanvasAlpha(ShouldShow);
        }
        private void HandleCanvasAlpha(bool show) => _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, show ? 1.0f : 0.0f, (1.0f / (show ? _showDuration : _fadeDuration)) * Time.deltaTime);


        private void OnInputDeviceChanged()
        {
            UpdateSpriteAsset();
            UpdateText();
        }


        protected virtual bool CheckForDeactivation()
        {
            ShouldShow = true;

            if (CheckLifetime())
            {
                return true;
            }

            return false;
        }
        private bool CheckLifetime()
        {
            if (_maxLifetime > 0)
            {
                _currentLifetime += Time.deltaTime;

                if (_currentLifetime > _maxLifetime)
                {
                    // Lifetime Elapsed.
                    return true;
                }
            }

            // Lifetime hasn't elapsed.
            return false;
        }


        protected void Deactivate() => IsDisabled = true;
        


        protected void SetupContents(PopupTextData popupTextData)
        {
            this._popupTextData = popupTextData;

            UpdateSpriteAsset();
            UpdateText();
        }
        private void UpdateText() => _popupText.text = _popupTextData.GetFormattedText();
        private void UpdateSpriteAsset() => _popupText.spriteAsset = InputIconManager.GetSpriteAsset(PlayerInput.LastUsedDevice);
        


        protected void SetContentsSize(float textSize)
        {
            _popupText.fontSize = textSize;
        }
        protected IEnumerator UpdateContentsRootSizeAndReadyAfterDelay()
        {
            yield return null;
            UpdateContentsRootSize();
            _isReady = true;
        }
        protected void UpdateContentsRootSize()
        {
            if (_backgroundRoot == null)
                return;

            // Calculate corners.
            Vector2 topLeft = _popupText.rectTransform.localPosition + new Vector3(_popupText.rectTransform.rect.xMin, _popupText.rectTransform.rect.yMax);
            Vector2 bottomLeft = _popupText.rectTransform.localPosition + new Vector3(_popupText.rectTransform.rect.xMin, _popupText.rectTransform.rect.yMin);

            Vector2 bottomRight = _popupText.rectTransform.localPosition + new Vector3(_popupText.rectTransform.rect.xMax, _popupText.rectTransform.rect.yMin);
            Vector2 topRight = _popupText.rectTransform.localPosition + new Vector3(_popupText.rectTransform.rect.xMax, _popupText.rectTransform.rect.yMax);

            // Calculate centre position & desired size.
            Vector2 centre = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
            float width = Mathf.Max(topRight.x, bottomRight.x) - Mathf.Min(topLeft.x, bottomLeft.x);
            float height = Mathf.Max(topRight.y, topLeft.y) - Mathf.Min(bottomRight.y, bottomLeft.y);


            // Set the position and size of the background.
            if (!_dontUpdateContentsRootPosition)
            {
                _contentsContainer.localPosition = centre;
            }
            _contentsContainer.sizeDelta = new Vector2(width + (_contentsPadding.x * 2.0f), height + (_contentsPadding.y * 2.0f));
        }
        protected void ToggleBackground(bool enableBackground) => _backgroundRoot.gameObject.SetActive(enableBackground);
        protected void SetupLifetimeDisabling(float lifetime)
        {
            _maxLifetime = lifetime;
            _currentLifetime = 0.0f;
        }
        protected void SetupInteractionDisabling(GameObject linkedInteractable, bool linkToSuccess, bool linkToFailure)
        {
            if (linkedInteractable.TryGetComponent(out IInteractable interactableScript) == false)
            {
                throw new ArgumentException($"The GameObject '{linkedInteractable.name}' does not contain a script that inherits from IInteractable.");
            }


            _linkedInteractable = interactableScript;

            if (linkToSuccess)
                _linkedInteractable.OnSuccessfulInteraction += Deactivate;
            if (linkToFailure)
                _linkedInteractable.OnFailedInteraction += Deactivate;
        }
    }
}
