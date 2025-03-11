using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class ScreenSpacePopupTrigger : PopupTrigger
    {
        [SerializeField] private ScreenSpacePopupSetupInformation _popupSetupInformation;

        public override void Trigger() => PopupManager.CreateScreenSpacePopup(_popupSetupInformation);


#if UNITY_EDITOR

        [ContextMenu("Breaks Connections/Convert to World-Space")]
        private void Editor_ConvertToWorldSpace()
        {
            WorldSpacePopupTrigger popupTrigger = gameObject.AddComponent<WorldSpacePopupTrigger>();
            popupTrigger.Editor_SetupFromPopupInfo(this._popupSetupInformation);
            DestroyImmediate(this);
        }
        public void Editor_SetupFromPopupInfo(PopupSetupInformation previousSetupInformation)
        {
            _popupSetupInformation = new ScreenSpacePopupSetupInformation(previousSetupInformation);
        }

#endif
    }
}
