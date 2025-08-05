using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Popups;
using TMPro;

namespace UI.Icons
{
    public class TextIconTester : MonoBehaviour
    {
        [SerializeField] private InteractionType _interactionType = InteractionType.DefaultInteract;
        [SerializeField] private TMP_Text _testText;


        [SerializeField] private TMP_SpriteAsset _mouseAndKeyboardAsset;
        [SerializeField] private TMP_SpriteAsset _gamepadAsset;


        private void Update()
        {
            DebugStringForInputAction();
        }
        [ContextMenu(itemName: "Debug String for Input Action")]
        private void DebugStringForInputAction()
        {
            switch (PlayerInput.LastUsedDevice)
            {
                case PlayerInput.DeviceType.MnK:
                    _testText.spriteAsset = _mouseAndKeyboardAsset;
                    break;
                case PlayerInput.DeviceType.Gamepad:
                    _testText.spriteAsset = _gamepadAsset;
                    break;
            }

            string spriteIdentifier = InteractionTypeExtension.GetInteractionSpriteIdentifierFromInteractionType(_interactionType);
            _testText.text = spriteIdentifier;
        }
    }
}