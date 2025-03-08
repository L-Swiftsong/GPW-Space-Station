using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Interaction;

namespace UI.Popups
{
    public abstract class PopupElement : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [Space(5)]
        [SerializeField, Range(0.0f, 0.5f)] private float _fadeDuration = 0.1f;
        [SerializeField, Range(0.0f, 0.5f)] private float _showDuration = 0.2f;
        private bool _isReady;


        [Header("Contents")]
        [SerializeField] private TMP_Text _preText;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _postText;

        [Space(5)]
        [SerializeField] private LayoutGroup _elementLayoutGroup;


        [Header("Background")]
        [SerializeField] private RectTransform _backgroundRoot; // Leave null to not have a background.
        [SerializeField] private Vector2 _backgroundPadding = new Vector2(5.0f, 5.0f); // Padding in each direction.
        [SerializeField] private bool _isMultiLine = false;
        [SerializeField] private bool _ignorePosition = false;


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
        }
        protected void OnDisable()
        {
            if (_linkedInteractable != null)
            {
                _linkedInteractable.OnSuccessfulInteraction -= Deactivate;
                _linkedInteractable.OnFailedInteraction -= Deactivate;
            }
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
        


        protected void SetupContents(string preText, Sprite sprite, string postText)
        {
            // Ensure the image is enabled.
            _image.gameObject.SetActive(true);

            // Set Values.
            _preText.text = preText;
            _image.sprite = sprite;
            _postText.text = postText;
        }
        protected void SetupCustomText(string customPreText = "", Sprite customSprite = null, string customPostText = "")
        {
            // Ensure the image is only active if we are supplying it with a value.
            _image.gameObject.SetActive(customSprite != null);

            // Set the value of the pre-text.
            _preText.text = customPreText;
            _image.sprite = customSprite;
            _postText.text = customPostText;
        }


        protected void SetContentsSize(float textSize, float imageSize)
        {
            _preText.fontSize = textSize;
            _postText.fontSize = textSize;

            _image.rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
        }
        protected void UpdateTextWidth(bool keepIconCentred)
        {
            if (keepIconCentred)
            {
                // Disable the Horizontal/Vertical Layout Group.
                _elementLayoutGroup.enabled = false;

                // Position the text boxes & image so that the icon is in the centre.
                if (_elementLayoutGroup is HorizontalLayoutGroup)
                {
                    // Position Horizontally.
                    float spacing = (_elementLayoutGroup as HorizontalLayoutGroup).spacing;

                    _image.rectTransform.localPosition = Vector2.zero;
                    _preText.rectTransform.localPosition = new Vector2(-(_image.rectTransform.sizeDelta.x / 2.0f + spacing), 0.0f);
                    _postText.rectTransform.localPosition = new Vector2(_image.rectTransform.sizeDelta.x / 2.0f + spacing, 0.0f);
                }
                else if (_elementLayoutGroup is VerticalLayoutGroup)
                {
                    // Position Vertically.
                    float spacing = (_elementLayoutGroup as VerticalLayoutGroup).spacing;

                    _image.rectTransform.localPosition = Vector2.zero;
                    _preText.rectTransform.localPosition = new Vector2(0.0f, _image.rectTransform.sizeDelta.y / 2.0f + spacing);
                    _postText.rectTransform.localPosition = new Vector2(0.0f, -(_image.rectTransform.sizeDelta.y / 2.0f + spacing));
                }

                _preText.ForceMeshUpdate();
                _postText.ForceMeshUpdate();
            }
            else
            {
                // Enable the Horizontal/Vertical Layout Group.
                _elementLayoutGroup.enabled = true;

                // Resize the Text Boxes to accurately represent the text size.
                _preText.ForceMeshUpdate();
                _preText.rectTransform.sizeDelta = _preText.text == string.Empty ? Vector2.zero : _preText.textBounds.size;

                _postText.ForceMeshUpdate();
                _postText.rectTransform.sizeDelta = _postText.text == string.Empty ? Vector2.zero : _postText.textBounds.size;
            }
        }
        protected IEnumerator InvokeAfterFrameDelay(System.Action callback)
        {
            yield return null;
            callback?.Invoke();
        }
        protected void UpdateBackgroundSize()
        {
            if (_backgroundRoot == null)
                return;

            // Calculate corners.
            Vector2 topLeft = _preText.rectTransform.localPosition + new Vector3(_preText.rectTransform.rect.xMin, _preText.rectTransform.rect.yMax);
            Vector2 bottomLeft = _isMultiLine ? _postText.rectTransform.localPosition + new Vector3(_postText.rectTransform.rect.xMin, _postText.rectTransform.rect.yMin)
                                                : _preText.rectTransform.localPosition + new Vector3(_preText.rectTransform.rect.xMin, _preText.rectTransform.rect.yMin);

            Vector2 bottomRight = _postText.rectTransform.localPosition + new Vector3(_postText.rectTransform.rect.xMax, _postText.rectTransform.rect.yMin);
            Vector2 topRight = _isMultiLine ? _preText.rectTransform.localPosition + new Vector3(_preText.rectTransform.rect.xMax, _preText.rectTransform.rect.yMax)
                                            : _postText.rectTransform.localPosition + new Vector3(_postText.rectTransform.rect.xMax, _postText.rectTransform.rect.yMax);

            // Calculate centre position & desired size.
            Vector2 centre = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
            float width = Mathf.Max(topRight.x, bottomRight.x) - Mathf.Min(topLeft.x, bottomLeft.x);
            float height = Mathf.Max(topRight.y, topLeft.y) - Mathf.Min(bottomRight.y, bottomLeft.y);

            // Account for image size in width & height calculations.
            width = Mathf.Max(width, _image.rectTransform.sizeDelta.x);
            height = Mathf.Max(height, _image.rectTransform.sizeDelta.y);


            // Set the position and size of the background.
            if (!_ignorePosition)
            {
                _backgroundRoot.localPosition = centre;
            }
            _backgroundRoot.sizeDelta = new Vector2(width + (_backgroundPadding.x * 2.0f), height + (_backgroundPadding.y * 2.0f));

            _isReady = true;
        }
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
