using System;
using UnityEngine;

namespace UI.Popups
{
    public class ScreenSpacePopupElement : PopupElement
    {
        [Header("Screen Space Element Settings")]
        [SerializeField] private RectTransform _rootTransform;

        public void SetupWithInformation(ScreenSpacePopupSetupInformation setupInformation, Sprite contentsSprite, Action onDisableCallback)
        {
            // Position Setup.
            SetupPosition(setupInformation.AnchoredPosition, setupInformation.Pivot, setupInformation.AnchorMin, setupInformation.AnchorMax);

            // Contents Setup.
            if (setupInformation.UseCustomText)
            {
                SetupCustomText(setupInformation.CustomPreText, setupInformation.CustomSprite, setupInformation.CustomPostText);
            }
            else
            {
                SetupContents(setupInformation.PopupPreText, contentsSprite, setupInformation.PopupPostText);
            }
            SetContentsSize(setupInformation.FontSize, setupInformation.IconSize);
            ToggleBackground(setupInformation.ShowBackground);

            UpdateTextWidth(setupInformation.KeepIconCentred);
            StartCoroutine(UpdateContentsRootSizeAndReadyAfterDelay()); // Invoked after a single frame delay so that bounds properly update.

            // General Disabling Setup.
            OnDisableCallback = onDisableCallback;
            SetupLifetimeDisabling(setupInformation.PopupLifetime);

            // Interaction Disabling Setup.
            if (setupInformation.LinkedInteractable != null)
            {
                SetupInteractionDisabling(setupInformation.LinkedInteractable, setupInformation.LinkToSuccess, setupInformation.LinkToFailure);
            }
        }
        private void SetupPosition(Vector2 anchoredPosition, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax)
        {
            _rootTransform.pivot = pivot;
            _rootTransform.anchorMin = anchorMin;
            _rootTransform.anchorMax = anchorMax;
            _rootTransform.anchoredPosition = anchoredPosition;
        }
    }
}
