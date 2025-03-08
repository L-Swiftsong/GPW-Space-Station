using UnityEngine;

namespace UI.Popups
{
    [System.Serializable]
    public class ScreenSpacePopupSetupInformation : PopupSetupInformation
    {
        [Header("Popup Positioning")]
        // Default Position is offset from the top left.
        public Vector2 AnchoredPosition = new Vector2(0.0f, -100.0f);
        public Vector2 Pivot = new Vector2(1.0f, 1.0f);
        public Vector2 AnchorMin = new Vector2(1.0f, 1.0f);
        public Vector2 AnchorMax = new Vector2(1.0f, 1.0f);


        private ScreenSpacePopupSetupInformation() : base(fontSize: 36.0f, iconSize: 50.0f, lifetime: 3.0f) { }
        public ScreenSpacePopupSetupInformation(PopupSetupInformation other) : base(other) { }
    }
}
