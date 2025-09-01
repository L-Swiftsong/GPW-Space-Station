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



        public void SetupWithInformation(WorldSpacePopupSetupInformation setupInformation, PopupTextData textData, Transform pivotTransform, Vector3 popupPosition, bool rotateInPlace, GameObject linkedInteractable, bool linkToSuccess, bool linkToFailure, Action onDisableCallback)
        {
            // Position Setup.
            SetupPosition(pivotTransform, popupPosition, rotateInPlace);

            // Contents Setup.
            SetupContents(textData);
            SetContentsSize(setupInformation.FontSize);
            ToggleBackground(setupInformation.ShowBackground);

            StartCoroutine(UpdateContentsRootSizeAndReadyAfterDelay()); // Invoked after a single frame delay so that bounds properly update.

            // General Disabling Setup.
            OnDisableCallback = onDisableCallback;
            SetupLifetimeDisabling(setupInformation.PopupLifetime);
            SetupGeneralDisabling(setupInformation, pivotTransform);

            // Interaction Disabling Setup.
            if (linkedInteractable != null)
            {
                SetupInteractionDisabling(linkedInteractable, linkToSuccess, linkToFailure);
            }
        }
        private void SetupPosition(Transform pivotTransform, Vector3 popupOffset, bool rotateInPlace)
        {
            if (pivotTransform == null)
            {
                Debug.LogWarning("WARNING: You are trying to create a WorldSpacePopup without assigning a pivot transform.\nEnsure the pivot transform is not null or missing.");
                Destroy(this.gameObject);
                return;
            }

            if (rotateInPlace)
            {
                _pivotTransform.position = pivotTransform.position + popupOffset;
                _offsetTransform.localPosition = Vector3.zero;
            }
            else
            {
                _pivotTransform.position = pivotTransform.position;
                _offsetTransform.localPosition = popupOffset;
            }
        }
        /// <summary>
        ///     Setup our values for the most common methods which we will be disabling this popup.
        /// </summary>
        /// <remarks> Distance, Obstruction, Pivot Loss. </remarks>
        private void SetupGeneralDisabling(WorldSpacePopupSetupInformation popupSetupInformation, Transform pivotTransform)
        {
            _maxPlayerDistance = popupSetupInformation.MaxDistance;
            _fadeIfOutwithDistance = popupSetupInformation.OnlyFadeIfOutwithMaxDistance;

            _disableIfObstructed = popupSetupInformation.DisableIfObstructed;
            _fadeIfObstructed = popupSetupInformation.OnlyFadeIfObstructed;

            _disableIfPivotLost = popupSetupInformation.DisableIfPivotDestroyed;
            _linkedPivot = pivotTransform;
        }
    }
}