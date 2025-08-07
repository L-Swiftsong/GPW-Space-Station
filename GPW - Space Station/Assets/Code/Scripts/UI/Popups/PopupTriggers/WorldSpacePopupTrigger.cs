using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class WorldSpacePopupTrigger : PopupTrigger
    {
        [SerializeField] private WorldSpacePopupSetupInformation _popupSetupInformation;


        [Header("Popup Positioning")]
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Vector3 _popupOffset = Vector3.zero;
        [SerializeField] private bool _rotateInPlace = true;


        public override void Trigger() => PopupManager.CreateWorldSpacePopup(_popupSetupInformation, TextData, _pivotTransform, _popupOffset, _rotateInPlace);


        private void OnDrawGizmosSelected()
        {
            if (_pivotTransform != null)
            {
                Gizmos.color = Color.yellow;
                if (_rotateInPlace)
                {
                    Gizmos.DrawSphere(_pivotTransform.position + _popupOffset, 0.25f);
                }
                else
                {
                    // Draw a circle a the horizontal pivot line.
                    Gizmos.DrawSphere(_pivotTransform.position + Vector3.up * _popupOffset.y, 0.1f);

                    // Draw a line to represent the horizontal offset.
                    Gizmos.DrawRay(_pivotTransform.position + Vector3.up * _popupOffset.y, new Vector3(_popupOffset.x, 0.0f, _popupOffset.z));
                }
            }
        }
    }
}