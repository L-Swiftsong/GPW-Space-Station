using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MouseOnlySlider : Slider
    {
        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            // Allow for navigation to other UI elements based on our navigation settings.
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (FindSelectableOnLeft() != null)
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (FindSelectableOnRight() != null)
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (FindSelectableOnUp() != null)
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (FindSelectableOnDown() != null)
                        base.OnMove(eventData);
                    break;
            }
        }
    }
}