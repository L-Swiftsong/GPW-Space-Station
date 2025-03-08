using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class PopupTrigger : MonoBehaviour
    {
        [Header("World Space")]
        [SerializeField] private PopupSetupInformation _popupSetupInformation;

        [Header("Screen Space")]
        [SerializeField] private ScreenSpacePopupSetupInformation _screenSpaceSetupInformation;


        public void TriggerWorldSpace()
        {
            PopupManager.CreateWorldSpacePopup(_popupSetupInformation);
        }
        public void TriggerScreenSpace()
        {
            PopupManager.CreateScreenPopup(_screenSpaceSetupInformation);
        }


        private void OnDrawGizmosSelected()
        {
            if (_popupSetupInformation.PivotTransform != null)
            {
                Gizmos.color = Color.yellow;
                if (_popupSetupInformation.RotateInPlace)
                {
                    Gizmos.DrawSphere(_popupSetupInformation.PivotTransform.position + _popupSetupInformation.PopupOffset, 0.25f);
                }
                else
                {
                    // Draw a circle a the horizontal pivot line.
                    Gizmos.DrawSphere(_popupSetupInformation.PivotTransform.position + Vector3.up * _popupSetupInformation.PopupOffset.y, 0.1f);

                    // Draw a line to represent the horizontal offset.
                    Gizmos.DrawRay(_popupSetupInformation.PivotTransform.position + Vector3.up * _popupSetupInformation.PopupOffset.y, new Vector3(_popupSetupInformation.PopupOffset.x, 0.0f, _popupSetupInformation.PopupOffset.z));
                }
            }
        }
    }
}