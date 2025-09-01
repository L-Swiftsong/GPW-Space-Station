using UnityEngine;
using UnityEditor;

namespace UI.Popups
{
    [CustomEditor(typeof(PopupTextData))]
    public class PopupTextDataEditor : Editor
    {
        private SerializedProperty _popupText;
        private SerializedProperty _interactionTypes;

        private void OnEnable()
        {
            _popupText = serializedObject.FindProperty("_popupText");
            _interactionTypes = serializedObject.FindProperty("_interactionTypes");
        }
        public override void OnInspectorGUI()
        {
            PopupTextData data = (PopupTextData)target;
            serializedObject.Update();

            // Validation.
            string validationOutput = Validate(data);

            // Display Data.
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            GUIStyle textAreaStype = new GUIStyle(EditorStyles.textArea);
            EditorGUILayout.TextArea(validationOutput, textAreaStype, GUILayout.Height(80));
            
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_popupText);
            EditorGUILayout.PropertyField(_interactionTypes);

            serializedObject.ApplyModifiedProperties();
        }


        private string Validate(PopupTextData data)
        {
            if (data.PopupText.Length <= 0)
                return "";

            int identifiersCount = data.PopupText.Split('{').Length - 1;
            if (data.InteractionTypes.Length != identifiersCount)
                data.SetInteractionTypesLength_Editor(identifiersCount);

            try
            {
                return data.GetFormattedText();
            }
            catch
            {
                return "Error: Invalid Data";
            }
        }
    }
}