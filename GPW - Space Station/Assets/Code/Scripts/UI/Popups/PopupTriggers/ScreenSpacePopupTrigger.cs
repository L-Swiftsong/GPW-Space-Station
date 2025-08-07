using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class ScreenSpacePopupTrigger : PopupTrigger
    {
        [SerializeField] private ScreenSpacePopupSetupInformation _popupSetupInformation;

        public override void Trigger() => PopupManager.CreateScreenSpacePopup(_popupSetupInformation, TextData);
    }
}
