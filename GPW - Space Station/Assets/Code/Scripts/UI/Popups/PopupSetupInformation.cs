using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UI.Popups.PopupManager;

namespace UI.Popups
{
    [System.Serializable]
    public class PopupSetupInformation
    {
        [Header("Popup Positioning")]
        public Transform PivotTransform;
        public Vector3 PopupOffset = Vector3.zero;
        public bool RotateInPlace = true;


        [Header("Popup Information")]
        public string PopupPreText = "Press";
        public InteractionType InteractionType = InteractionType.DefaultInteract;
        public string PopupPostText = "to Interact";

        [Space(5)]
        public bool DisplayOnMultipleLines = false;


        [Header("General Deactivation Settings")]
        public float PopupLifetime = -1;

        [Space(5)]
        public float MaxDistance = -1;
        public bool OnlyFadeIfOutwithMaxDistance = true;

        [Space(5)]
        public bool DisableIfObstructed = true;
        public bool OnlyFadeIfObstructed = true;

        [Space(5)]
        public bool DisableIfPivotDestroyed = true;


        [Header("Advanced Deactivation Settings")]
        public GameObject LinkedInteractable;

        [Space(5)]
        public bool LinkToSuccess = true;
        public bool LinkToFailure = true;



        private PopupSetupInformation() { }
        public PopupSetupInformation(Transform pivotTransform) => this.PivotTransform = pivotTransform;
    }
}
