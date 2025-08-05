using UnityEngine;
using UnityEditor;

namespace Tutorials
{
    [CustomEditor(typeof(TutorialTextData))]
    public class TutorialTextDataEditor : Editor
    {
        private SerializedProperty _tutorialText;
        private SerializedProperty _interactionTypes;

        private void OnEnable()
        {
            _tutorialText = serializedObject.FindProperty("_tutorialText");
            _interactionTypes = serializedObject.FindProperty("_interactionTypes");
        }
        public override void OnInspectorGUI()
        {
            TutorialTextData data = (TutorialTextData)target;
            serializedObject.Update();

            // Validation.
            string validationOutput = Validate(data);

            // Display Data.
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            GUIStyle textAreaStype = new GUIStyle(EditorStyles.textArea);
            EditorGUILayout.TextArea(validationOutput, textAreaStype, GUILayout.Height(80));

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_tutorialText);
            EditorGUILayout.PropertyField(_interactionTypes);

            serializedObject.ApplyModifiedProperties();
        }


        private string Validate(TutorialTextData data)
        {
            int identifiersCount = data.TutorialText.Split('{').Length - 1;
            if (data.InteractionTypes.Length != identifiersCount)
                data.SetInteractionTypes_Editor(new InteractionType[identifiersCount]);

            try
            {
                return data.GetFormattedTutorialText();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return "Error: Invalid Data";
            }
        }
    }
}