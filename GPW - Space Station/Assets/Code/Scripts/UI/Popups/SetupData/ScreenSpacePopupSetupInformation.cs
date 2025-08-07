using UnityEngine;

namespace UI.Popups
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Popups/Setup Information/New Screen Space Setup Information")]
    public class ScreenSpacePopupSetupInformation : PopupSetupInformation
    {
        [Header("Popup Positioning")]
        // Default Position is offset from the top left.
        public Vector2 AnchoredPosition = new Vector2(0.0f, -100.0f);
        public Vector2 Anchors = new Vector2(1.0f, 1.0f);
        public Vector2 Pivot = new Vector2(1.0f, 1.0f);

        [Space(5)]
        public Vector2 Bounds = new Vector2(660.0f, 110.0f);


        private ScreenSpacePopupSetupInformation() : base(fontSize: 36.0f, lifetime: 3.0f, showBackground: true) { }
        public ScreenSpacePopupSetupInformation(PopupSetupInformation other) : base(other) { }
    }
}
