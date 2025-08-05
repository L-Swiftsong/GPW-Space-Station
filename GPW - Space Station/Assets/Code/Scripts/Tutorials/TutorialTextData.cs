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
        public void SetInteractionTypes_Editor(InteractionType[] interactionTypes) => _interactionTypes = interactionTypes; 
        #endif
    }
}