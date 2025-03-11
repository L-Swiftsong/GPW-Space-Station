using UnityEngine;

namespace UI.Popups
{
    [System.Serializable]
    public class WorldSpacePopupSetupInformation : PopupSetupInformation
    {
        [Header("World Space Deactivation Settings")]
        public float MaxDistance = -1;
        public bool OnlyFadeIfOutwithMaxDistance = true;

        [Space(5)]
        public bool DisableIfObstructed = true;
        public bool OnlyFadeIfObstructed = true;

        [Space(5)]
        public bool DisableIfPivotDestroyed = true;
        
        
        [Header("Popup Positioning")]
        public Transform PivotTransform;
        public Vector3 PopupOffset = Vector3.zero;
        public bool RotateInPlace = true;



        private WorldSpacePopupSetupInformation() : base(fontSize: 72.0f, iconSize: 100.0f, lifetime: -1.0f, showBackground: false) { }
        public WorldSpacePopupSetupInformation(Transform pivotTransform) : base(fontSize: 72.0f, iconSize: 100.0f, lifetime: -1.0f, showBackground: false) => this.PivotTransform = pivotTransform;
        public WorldSpacePopupSetupInformation(PopupSetupInformation other) : base(other) { }
    }
}
