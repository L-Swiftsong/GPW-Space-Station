using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private int _buildIndex = -1;
        public string SceneName => _sceneName;
        public int BuildIndex => _buildIndex;


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
            SerializedProperty buildIndex = property.FindPropertyRelative("_buildIndex");

            // Create and store the position of a label to display the property within.
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (sceneAsset != null )
            {
                // Display an object field for us to set the value of the sceneAsset property.
                sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

                if (sceneAsset.objectReferenceValue != null)
                {
                    // Get the scene name of the sceneAsset object and check that we haven't already setup this SceneField with the correct values.
                    string sceneNameValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
                    if (sceneName.stringValue == sceneNameValue && buildIndex.intValue != -1)
                    {
                        // We've already got a correct reference for the set scene.
                        return;
                    }

                    // Set the value of sceneName to the name of the sceneAsset.
                    sceneName.stringValue = sceneNameValue;

                    // Get the value of the scene's build index by looping through and comparing the name of the desired scenes with all scenes in the build order.
                    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
                    {
                        string builtScenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        string builtSceneName = System.IO.Path.GetFileNameWithoutExtension(builtScenePath);
                        if (builtSceneName == sceneNameValue)
                        {
                            buildIndex.intValue = i;
                            break;
                        }
                    }

                    if (buildIndex.intValue == -1)
                    {
                        // We didn't find our scene in the build list, likely due to us not having added the scene to the build list.
                        throw new System.ArgumentException($"The scene with name {sceneNameValue} is not included in the build, and thus cannot be referenced via a SceneField instance.");
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
    #endif
}