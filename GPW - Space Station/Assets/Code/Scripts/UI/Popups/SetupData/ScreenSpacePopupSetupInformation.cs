using UnityEngine;

namespace UI.Popups
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Popups/Setup Information/New Screen Space Setup Information")]
    public class ScreenSpacePopupSetupInformation : PopupSetupInformation
    {
        [Header("Popup Positioning")]
        // Default Position is offset from the top left.
        public Vector2 PositionOffset = new Vector2(0.0f, -100.0f);
        public AnchorPosition AnchorPosition = AnchorPosition.TopRight;

        [Space(5)]
        public Vector2 Bounds = new Vector2(660.0f, 110.0f);


        private ScreenSpacePopupSetupInformation() : base(fontSize: 36.0f, lifetime: 3.0f, showBackground: true) { }
        public ScreenSpacePopupSetupInformation(PopupSetupInformation other) : base(other) { }
    }


    [System.Serializable]
    public enum AnchorPosition
    {
        TopLeft,
        TopMiddle,
        TopRight,


        [InspectorName(null)]
        ValueCount
    };
    public static class AnchorPositionExtensions
    {
        /// <summary>
        ///     Get the values for AnchorMin, AnchorMax, & Pivot for this value of AnchorPosition.
        /// </summary>
        public static void GetAnchorValues(this AnchorPosition anchorPosition, out Vector2 anchors, out Vector2 pivot)
        {
            switch(anchorPosition)
            {
                case AnchorPosition.TopLeft:
                    anchors = new Vector2(0.0f, 1.0f);
                    pivot = new Vector2(0.0f, 1.0f);
                    break;
                case AnchorPosition.TopMiddle:
                    anchors = new Vector2(0.5f, 1.0f);
                    pivot = new Vector2(0.5f, 1.0f);
                    break;
                case AnchorPosition.TopRight:
                    anchors = new Vector2(1.0f, 1.0f);
                    pivot = new Vector2(1.0f, 1.0f);
                    break;
                default:
                    // No implementation for the input enum value.
                    throw new System.NotImplementedException();
            }

        }
    }
}
