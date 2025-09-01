using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.World;
using Interaction;

namespace Computers
{
    public class TerminalLogUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WorldSpaceButton _logNameButton;
        [SerializeField] private WorldSpaceButton _logMainButton;


        private void Awake() => SubscribeToButtonEvents();
        public void SetupLogUI(TerminalLogSO terminalLogData)
        {
            _logNameButton.TrySetText(terminalLogData.LogName);
            _logMainButton.TrySetText(terminalLogData.LogText);
        }


        private void SubscribeToButtonEvents()
        {
            _logNameButton.OnSuccessfulInteraction += ShowAccessibleText;
            _logMainButton.OnSuccessfulInteraction += ShowAccessibleText;
        }
        private void UnsubscribeFromButtonEvents()
        {
            _logNameButton.OnSuccessfulInteraction -= ShowAccessibleText;
            _logMainButton.OnSuccessfulInteraction -= ShowAccessibleText;
        }

        private void ShowAccessibleText() => ShowAccessibleText(_logMainButton.TryGetText(), _logNameButton.TryGetText());
        private void ShowAccessibleText(string bodyText, string titleText = null)
        {
            AccessibilityText.DisplayAccessibleText(bodyText, titleText);

            PlayerInput.OnInteractPerformed += PlayerInput_OnInteractPerformed;

            UnsubscribeFromButtonEvents();
            PlayerInteraction.SetCurrentInteractableOverride(_logNameButton);
        }
        
        
        private void PlayerInput_OnInteractPerformed()
        {
            AccessibilityText.StopDisplayingAccessibleText();

            PlayerInput.OnInteractPerformed -= PlayerInput_OnInteractPerformed;

            SubscribeToButtonEvents();
            PlayerInteraction.ResetCurrentInteractableOverride();
        }

    }
}
