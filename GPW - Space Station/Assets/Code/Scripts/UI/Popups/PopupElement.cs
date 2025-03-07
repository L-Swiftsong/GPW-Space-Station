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

        [Space(5)]
        [SerializeField] private TMP_Text _preText;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _postText;


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
            _preText.text = preText;
            _image.sprite = sprite;
            _postText.text = postText;
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
    }
}
