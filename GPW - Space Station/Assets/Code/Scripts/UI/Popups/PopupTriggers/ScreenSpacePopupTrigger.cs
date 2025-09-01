using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    public class ScreenSpacePopupTrigger : PopupTrigger
    {
        [SerializeField] private ScreenSpacePopupSetupInformation _popupSetupInformation;


        [Header("Popup Early Disable")]
        [SerializeField] private GameObject _linkedInteractable;

        [Space(5)]
        [SerializeField] private bool _linkToSuccess = true;
        [SerializeField] private bool _linkToFailure = true;


        public override void Trigger() => PopupManager.CreateScreenSpacePopup(_popupSetupInformation, _linkedInteractable, _linkToSuccess, _linkToFailure, TextData);
    }
}
