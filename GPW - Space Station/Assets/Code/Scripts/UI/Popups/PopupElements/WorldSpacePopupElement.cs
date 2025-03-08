using System;
using UnityEngine;
using Entities.Player;

namespace UI.Popups
{
    public class WorldSpacePopupElement : PopupElement
    {
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Transform _offsetTransform;


        [Header("Deactivation")]
        [SerializeField, ReadOnly] private float _maxPlayerDistance;
        [SerializeField, ReadOnly] private bool _fadeIfOutwithDistance;

        [Space(5)]
        [SerializeField, ReadOnly] private bool _disableIfObstructed;
        [SerializeField, ReadOnly] private bool _fadeIfObstructed;
        [SerializeField] private LayerMask _obstructionLayers;

        [Space(5)]
        [SerializeField, ReadOnly] private bool _disableIfPivotLost;
        [SerializeField, ReadOnly] private Transform _linkedPivot;


        


        protected override void Update()
        {
            base.Update();

            RotateToFacePlayerCamera();
        }


        protected override bool CheckForDeactivation()
        {
            if (base.CheckForDeactivation() || CheckDistance() || CheckObstruction() || CheckPivotLoss())
            {
                return true;
            }
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
                    ShouldShow = false;
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
                    ShouldShow = false;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
        private bool CheckPivotLoss() => _disableIfPivotLost && _linkedPivot == null;


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



        public void SetupWithInformation(WorldSpacePopupSetupInformation setupInformation, Sprite contentsSprite, Action onDisableCallback)
        {
            // Position Setup.
            SetupPosition(setupInformation);

            // Contents Setup.
            if (setupInformation.UseCustomText)
            {
                SetupCustomText(setupInformation.CustomPreText, setupInformation.CustomSprite, setupInformation.CustomPostText);
            }
            else
            {
                SetupContents(setupInformation.PopupPreText, contentsSprite, setupInformation.PopupPostText);
            }
            SetContentsSize(setupInformation.FontSize, setupInformation.IconSize);

            UpdateTextWidth(setupInformation.KeepIconCentred);
            StartCoroutine(InvokeAfterFrameDelay(UpdateBackgroundSize)); // Invoked after a single frame delay so that bounds properly update.

            // General Disabling Setup.
            OnDisableCallback = onDisableCallback;
            SetupLifetimeDisabling(setupInformation.PopupLifetime);
            SetupGeneralDisabling(setupInformation);

            // Interaction Disabling Setup.
            if (setupInformation.LinkedInteractable != null)
            {
                SetupInteractionDisabling(setupInformation.LinkedInteractable, setupInformation.LinkToSuccess, setupInformation.LinkToFailure);
            }
        }
        private void SetupPosition(WorldSpacePopupSetupInformation popupSetupInformation)
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
        private void SetupGeneralDisabling(WorldSpacePopupSetupInformation popupSetupInformation)
        {
            _maxPlayerDistance = popupSetupInformation.MaxDistance;
            _fadeIfOutwithDistance = popupSetupInformation.OnlyFadeIfOutwithMaxDistance;

            _disableIfObstructed = popupSetupInformation.DisableIfObstructed;
            _fadeIfObstructed = popupSetupInformation.OnlyFadeIfObstructed;

            _disableIfPivotLost = popupSetupInformation.DisableIfPivotDestroyed;
            _linkedPivot = popupSetupInformation.PivotTransform;
        }
    }
}