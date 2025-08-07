using System;
using UnityEngine;

namespace UI.Popups
{
    public class ScreenSpacePopupElement : PopupElement
    {
        [Header("Screen Space Element Settings")]
        [SerializeField] private RectTransform _rootTransform;

        public void SetupWithInformation(ScreenSpacePopupSetupInformation setupInformation, PopupTextData textData, Action onDisableCallback)
        {
            // Position Setup.
            SetupPosition(setupInformation.AnchoredPosition, setupInformation.Anchors, setupInformation.Pivot, setupInformation.Bounds);

            // Contents Setup.
            SetupContents(textData);
            SetContentsSize(setupInformation.FontSize);
            ToggleBackground(setupInformation.ShowBackground);

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
        private void SetupPosition(Vector2 position, Vector2 anchors, Vector2 pivot, Vector2 bounds)
        {
            _rootTransform.pivot = pivot;

            _rootTransform.sizeDelta = bounds;

            _rootTransform.anchorMin = anchors;
            _rootTransform.anchorMax = anchors;
            _rootTransform.anchoredPosition = position;
        }
    }
}
