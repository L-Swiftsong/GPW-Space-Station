using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class WorldSpacePopupTrigger : PopupTrigger
    {
        [SerializeField] private WorldSpacePopupSetupInformation _popupSetupInformation;

        public override void Trigger() => PopupManager.CreateWorldSpacePopup(_popupSetupInformation);


#if UNITY_EDITOR

        [ContextMenu("Breaks Connections/Convert to Screen-Space")]
        private void Editor_ConvertToScreenSpace()
        {
            ScreenSpacePopupTrigger popupTrigger = gameObject.AddComponent<ScreenSpacePopupTrigger>();
            popupTrigger.Editor_SetupFromPopupInfo(this._popupSetupInformation);
            DestroyImmediate(this);
        }
        public void Editor_SetupFromPopupInfo(PopupSetupInformation previousSetupInformation)
        {
            _popupSetupInformation = new WorldSpacePopupSetupInformation(previousSetupInformation);
        }

#endif


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