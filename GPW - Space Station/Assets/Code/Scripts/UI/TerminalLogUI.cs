using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
            _logNameButton.OnSuccessfulInteraction += ShowAccessibleTextForName;
            _logMainButton.OnSuccessfulInteraction += ShowAccessibleTextForMain;
        }
        private void UnsubscribeFromButtonEvents()
        {
            _logNameButton.OnSuccessfulInteraction -= ShowAccessibleTextForName;
            _logMainButton.OnSuccessfulInteraction -= ShowAccessibleTextForMain;
        }

        private void ShowAccessibleTextForName() => ShowAccessibleText(_logNameButton.TryGetText());
        private void ShowAccessibleTextForMain() => ShowAccessibleText(_logMainButton.TryGetText());
        private void ShowAccessibleText(string text)
        {
            AccessibilityText.DisplayAccessibleText(text);

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
