using UnityEngine;

namespace UI.Popups
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Popups/Setup Information/New World Space Setup Information")]
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



        private WorldSpacePopupSetupInformation() : base(fontSize: 72.0f, lifetime: -1.0f, showBackground: false) { }
        public WorldSpacePopupSetupInformation(PopupSetupInformation other) : base(other) { }
    }
}
