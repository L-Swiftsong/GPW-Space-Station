using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Code courtesy of 'glitchers' via this link: 'https://discussions.unity.com/t/inspector-field-for-scene-asset/40763'.

namespace SceneManagement
{
    /// <summary> A class which allows us to direcly set references to a scene in the inspector.</summary>
    [System.Serializable]
    public class SceneField
    {
        [SerializeField] private Object _sceneAsset;

        [SerializeField] private string _sceneName = "";
        public string SceneName => _sceneName;

        // This function makes the SceneField work with existing Unity methods (Such as LoadLevel/LoadScene).
        public static implicit operator string(SceneField sceneField) => sceneField.SceneName;
    }


    #if UNITY_EDITOR
    // A custom inspector to allow us to set the SceneAsset values in the inspector.
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            // Find the properties of the SceneField.
            SerializedProperty sceneAsset = property.FindPropertyRelative("_sceneAsset");
            SerializedProperty sceneName = property.FindPropertyRelative("_sceneName");

            // Create and store the position of a label to display the property within.
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (sceneAsset != null )
            {
                // Display an object field for us to set the value of the sceneAsset property.
                sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

                if (sceneAsset.objectReferenceValue != null)
                {
                    // Set the value of sceneName to the name of the sceneAsset.
                    sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
                }
            }

            EditorGUI.EndProperty();
        }
    }
    #endif
}