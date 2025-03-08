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
        public float FontSize = 72.0f;
        public float IconSize = 100.0f;

        [Space(5)]
        public bool DisplayOnMultipleLines = false;
        public bool KeepIconCentred = false;


        [Header("Custom Text")]
        public bool UseCustomText = false;
        public string CustomPreText;
        public Sprite CustomSprite;
        public string CustomPostText;


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
    [System.Serializable]
    public class ScreenSpacePopupSetupInformation
    {
        [Header("Popup Positioning")]
        // Default Position is offset from the top left.
        public Vector2 AnchoredPosition = new Vector2(0.0f, -100.0f);
        public Vector2 Pivot = new Vector2(1.0f, 1.0f);
        public Vector2 AnchorMin = new Vector2(1.0f, 1.0f);
        public Vector2 AnchorMax = new Vector2(1.0f, 1.0f);


        [Header("Popup Information")]
        public string PopupPreText = "Press";
        public InteractionType InteractionType = InteractionType.DefaultInteract;
        public string PopupPostText = "to Interact";

        [Space(5)]
        public float FontSize = 36.0f;
        public float IconSize = 50.0f;

        [Space(5)]
        public bool DisplayOnMultipleLines = false;
        public bool KeepIconCentred = false;


        [Header("Custom Text")]
        public bool UseCustomText = false;
        public string CustomPreText;
        public Sprite CustomSprite;
        public string CustomPostText;


        [Header("General Deactivation Settings")]
        public float PopupLifetime = -1;


        [Header("Advanced Deactivation Settings")]
        public GameObject LinkedInteractable;

        [Space(5)]
        public bool LinkToSuccess = true;
        public bool LinkToFailure = true;
    }
}
