using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Entities.Player;
using Interaction;

namespace UI.Popups
{
    public class PopupElement : MonoBehaviour
    {
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Transform _offsetTransform;

        [Space(5)]
        [SerializeField] private CanvasGroup _canvasGroup; 


        [Header("Contents")]
        [SerializeField] private TMP_Text _preText;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _postText;
        [SerializeField] private LayoutGroup _elementLayoutGroup;


        [Header("Background")]
        [SerializeField] private RectTransform _backgroundRoot; // Leave null to not have a background.
        [SerializeField] private Vector2 _backgroundPadding = new Vector2(5.0f, 5.0f); // Padding in each direction.


        [Header("Deactivation")]
        [SerializeField, ReadOnly] private float _maxLifetime;
        [SerializeField, ReadOnly] private float _currentLifetime;

        [Space(5)]
        [SerializeField, ReadOnly] private float _maxPlayerDistance;
        [SerializeField, ReadOnly] private bool _fadeIfOutwithDistance;

        [Space(5)]
        [SerializeField, ReadOnly] private bool _disableIfObstructed;
        [SerializeField, ReadOnly] private bool _fadeIfObstructed;
        [SerializeField] private LayerMask _obstructionLayers;

        [Space(5)]
        [SerializeField, ReadOnly] private bool _disableIfPivotLost;
        [SerializeField, ReadOnly] private Transform _linkedPivot; 


        [Header("Interaction Disabling")]
        private IInteractable _linkedInteractable;


        private bool _shouldShow;
        private Action _onDisableCallback;


        private void OnEnable()
        {
            _canvasGroup.alpha = 1.0f;
            _shouldShow = true;
        }
        private void OnDisable()
        {
            if (_linkedInteractable != null)
            {
                _linkedInteractable.OnSuccessfulInteraction -= Deactivate;
                _linkedInteractable.OnFailedInteraction -= Deactivate;
            }
        }


        private void Update()
        {
            CheckForDeactivation();

            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _shouldShow ? 1.0f : 0.0f, 5.0f * Time.deltaTime);

            RotateToFacePlayerCamera();
        }


        private void CheckForDeactivation()
        {
            _shouldShow = true;

            if (CheckLifetime() || CheckDistance() || CheckObstruction() || CheckPivotLoss())
            {
                Deactivate();
            }
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
        private bool CheckDistance()
        {
            if (_maxPlayerDistance <= 0)
            {
                // We aren't considering player distance for deactivation/fading.
                return false;
            }

            if (Vector3.Distance(_offsetTransform.position, PlayerManager.Instance.GetPlayerCameraTransform().position) >= _maxPlayerDistance)
            {
                // Outwith Max Distance.
                if (_fadeIfOutwithDistance)
                {
                    // We only want to fade when outwith the max distance.
                    _shouldShow = false;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
            return false;
        }
        private bool CheckObstruction()
        {
            if (_disableIfObstructed && Physics.Linecast(_offsetTransform.position, PlayerManager.Instance.GetPlayerCameraTransform().position, _obstructionLayers, QueryTriggerInteraction.Ignore))
            {
                if (_fadeIfObstructed)
                {
                    // We only want to fade when the player's camera is obstructed.
                    _shouldShow = false;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
        private bool CheckPivotLoss() => _disableIfPivotLost && _pivotTransform == null;


        private void Deactivate() => _onDisableCallback?.Invoke();


        private void RotateToFacePlayerCamera()
        {
            Transform playerCameraTransform = PlayerManager.Instance.GetPlayerCameraTransform();

            // Rotate Pivot around the Y-Axis.
            Vector3 pivotLookDirection = _pivotTransform.position - playerCameraTransform.position;
            float yDegrees = Mathf.Atan2(pivotLookDirection.x, pivotLookDirection.z) * Mathf.Rad2Deg;
            _pivotTransform.LookAt(new Vector3(playerCameraTransform.position.x, transform.position.y, playerCameraTransform.position.z));

            // Rotate Offset around the X-Axis.
            Vector3 offsetLookDirection = _offsetTransform.position - playerCameraTransform.position;
            float xDegrees = Mathf.Atan2(offsetLookDirection.x, offsetLookDirection.y) * Mathf.Rad2Deg;
            _offsetTransform.LookAt(playerCameraTransform.position);
            _offsetTransform.localRotation = Quaternion.Euler(-_offsetTransform.localRotation.eulerAngles.x, -180.0f, 0.0f);
        }



        public void SetupWithInformation(PopupSetupInformation setupInformation, Sprite contentsSprite, Action onDisableCallback)
        {
            // Position Setup.
            SetupPosition(setupInformation);

            // Contents Setup.
            SetupContents(setupInformation.PopupPreText, contentsSprite, setupInformation.PopupPostText);
            UpdateTextWidth(setupInformation.KeepIconCentred);
            StartCoroutine(InvokeAfterFrameDelay(UpdateBackgroundSize)); // Invoked after a single frame delay so that bounds properly update.

            // General Disabling Setup.
            _onDisableCallback = onDisableCallback;
            SetupGeneralDisabling(setupInformation);

            // Interaction Disabling Setup.
            if (setupInformation.LinkedInteractable != null)
            {
                SetupInteractionDisabling(setupInformation.LinkedInteractable, setupInformation.LinkToSuccess, setupInformation.LinkToFailure);
            }
        }
        private void SetupPosition(PopupSetupInformation popupSetupInformation)
        {
            if (popupSetupInformation.RotateInPlace)
            {
                _pivotTransform.position = popupSetupInformation.PivotTransform.position + popupSetupInformation.PopupOffset;
                _offsetTransform.localPosition = Vector3.zero;
            }
            else
            {
                _pivotTransform.position = popupSetupInformation.PivotTransform.position;
                _offsetTransform.localPosition = popupSetupInformation.PopupOffset;
            }
        }
        private void SetupContents(string preText, Sprite sprite, string postText)
        {
            // Set Values.
            _preText.text = preText;
            _image.sprite = sprite;
            _postText.text = postText;
        }
        private void UpdateTextWidth(bool keepIconCentred)
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
                _preText.rectTransform.sizeDelta = new Vector2(_preText.textBounds.size.x, _preText.rectTransform.sizeDelta.y);
                _postText.ForceMeshUpdate();
                _postText.rectTransform.sizeDelta = new Vector2(_postText.textBounds.size.x, _postText.rectTransform.sizeDelta.y);
            }
        }
        private IEnumerator InvokeAfterFrameDelay(System.Action callback)
        {
            yield return null;
            callback?.Invoke();
        }
        private void UpdateBackgroundSize()
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
            _backgroundRoot.localPosition = centre;
            _backgroundRoot.sizeDelta = new Vector2(width + (_backgroundPadding.x * 2.0f), height + (_backgroundPadding.y * 2.0f));
        }
        private void SetupGeneralDisabling(PopupSetupInformation popupSetupInformation)
        {
            _maxLifetime = popupSetupInformation.PopupLifetime;
            _currentLifetime = 0;

            _maxPlayerDistance = popupSetupInformation.MaxDistance;
            _fadeIfOutwithDistance = popupSetupInformation.OnlyFadeIfOutwithMaxDistance;

            _disableIfObstructed = popupSetupInformation.DisableIfObstructed;
            _fadeIfObstructed = popupSetupInformation.OnlyFadeIfObstructed;

            _disableIfPivotLost = popupSetupInformation.DisableIfPivotDestroyed;
            _linkedPivot = popupSetupInformation.PivotTransform;
        }
        private void SetupInteractionDisabling(GameObject linkedInteractable, bool linkToSuccess, bool linkToFailure)
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


        [ContextMenu("Test Background")]
        private void TestBackground()
        {
            UpdateBackgroundSize();
        }


        [Header("Gizmos")]
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private bool _isMultiLine;
        private void OnDrawGizmos()
        {
            if (_canvasTransform == null)
                return;

            Vector2 topLeft = _canvasTransform.TransformPoint(_preText.rectTransform.localPosition + new Vector3(_preText.rectTransform.rect.xMin, _preText.rectTransform.rect.yMax));
            Vector2 bottomLeft;
            Vector2 topRight;
            Vector2 bottomRight = _canvasTransform.TransformPoint(_postText.rectTransform.localPosition + new Vector3(_postText.rectTransform.rect.xMax, _postText.rectTransform.rect.yMin));
            if (_isMultiLine)
            {
                topRight = _canvasTransform.TransformPoint(_preText.rectTransform.localPosition + new Vector3(_preText.rectTransform.rect.xMax, _preText.rectTransform.rect.yMax));
                bottomLeft = _canvasTransform.TransformPoint(_postText.rectTransform.localPosition + new Vector3(_postText.rectTransform.rect.xMin, _postText.rectTransform.rect.yMin));
            }
            else
            {
                bottomLeft = _canvasTransform.TransformPoint(_preText.rectTransform.localPosition + new Vector3(_preText.rectTransform.rect.xMin, _preText.rectTransform.rect.yMin));
                topRight = _canvasTransform.TransformPoint(_postText.rectTransform.localPosition + new Vector3(_postText.rectTransform.rect.xMax, _postText.rectTransform.rect.yMax));
            }

            Vector2 centre = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
            float width = Mathf.Max(topRight.x, bottomRight.x) - Mathf.Min(topLeft.x, bottomLeft.x);
            float height = Mathf.Max(topRight.y, topLeft.y) - Mathf.Min(bottomRight.y, bottomLeft.y);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(topLeft, 0.01f);
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(bottomLeft, 0.01f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(topRight, 0.01f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bottomRight, 0.01f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(centre, 0.01f);
            Gizmos.DrawLine(centre + (Vector2.left * width / 2.0f), centre + (Vector2.right * width / 2.0f));
            Gizmos.DrawLine(centre + (Vector2.down * height / 2.0f), centre + (Vector2.up * height / 2.0f));
        }
    }
}
