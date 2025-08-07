using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    [CreateAssetMenu(menuName = "Popups/New Popup Text Data")]
    public class PopupTextData : ScriptableObject
    {
        [SerializeField] [TextArea] private string _popupText;
        [SerializeField] private InteractionType[] _interactionTypes;


        public string PopupText => _popupText;
        public InteractionType[] InteractionTypes => _interactionTypes;


        public string GetFormattedText()
        {
            string[] interactionIdentifiers = new string[_interactionTypes.Length];
            for (int i = 0; i < _interactionTypes.Length; ++i)
                interactionIdentifiers[i] = InteractionTypeExtension.GetInteractionSpriteIdentifierFromInteractionType(_interactionTypes[i]);

            return string.Format(_popupText, interactionIdentifiers);
        }


        #if UNITY_EDITOR
        public void SetTutorialText_Editor(string tutorialText) => _popupText = tutorialText;
        public void SetInteractionTypesLength_Editor(int length)
        {
            // Generate a new array of InteractionTypes to replace our old one.
            InteractionType[] newInteractionTypes = new InteractionType[length];

            // Populate the new array with our current values (Not exceeding either array).
            int lowestLength = Mathf.Min(length, _interactionTypes.Length);
            for(int i = 0; i < lowestLength; ++i)
            {
                newInteractionTypes[i] = _interactionTypes[i];
            }

            // Set our array.
            _interactionTypes = newInteractionTypes;
        } 
        #endif
    }
}