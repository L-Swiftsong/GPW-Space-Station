using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorials
{
    [CreateAssetMenu(menuName = "Tutorials/New Tutorial Text Data")]
    public class TutorialTextData : ScriptableObject
    {
        [SerializeField] [TextArea] private string _tutorialText;
        [SerializeField] private InteractionType[] _interactionTypes;


        public string TutorialText => _tutorialText;
        public InteractionType[] InteractionTypes => _interactionTypes;


        public string GetFormattedTutorialText()
        {
            string[] interactionIdentifiers = new string[_interactionTypes.Length];
            for (int i = 0; i < _interactionTypes.Length; ++i)
                interactionIdentifiers[i] = InteractionTypeExtension.GetInteractionSpriteIdentifierFromInteractionType(_interactionTypes[i]);

            return string.Format(_tutorialText, interactionIdentifiers);
        }


        #if UNITY_EDITOR
        public void SetTutorialText_Editor(string tutorialText) => _tutorialText = tutorialText;
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