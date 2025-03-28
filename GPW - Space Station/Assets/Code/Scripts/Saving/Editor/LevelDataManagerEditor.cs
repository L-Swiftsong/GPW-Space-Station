using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Saving.LevelData
{
    [CustomEditor(typeof(LevelDataManager))]
    public class LevelDataManagerEditor : Editor
    {
        private const string SAVE_DATA_PROPERTY_NAME = "_saveData";

        private const string SAVEABLE_OBJECTS_ARRAY_PROPERTY_NAME = "_saveableObjects";

        public override void OnInspectorGUI()
        {
            LevelDataManager levelDataManager = (LevelDataManager)target;


            EditorGUILayout.LabelField("Level Data Manager Data", EditorStyles.whiteLargeLabel);
            EditorGUILayout.Space(5);

            if (levelDataManager.Editor_IsSceneInvalid())
            {
                EditorGUILayout.LabelField("ERROR: Invalid Scene (Not in Build).", EditorStyles.boldLabel);
            }

            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(SAVE_DATA_PROPERTY_NAME), true);
            GUI.enabled = true;

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Linked Saveable Objects", EditorStyles.whiteLargeLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Link Saveable Objects"))
            {
                LinkSaveableObjects(levelDataManager);
            }
            if (levelDataManager.Editor_AreAnySaveableObjectsInvalid())
            {
                EditorGUILayout.LabelField("ERROR: Invalid Entry in SaveableObjects array.", EditorStyles.boldLabel);
            }
            
            EditorGUILayout.Space(5);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty(SAVEABLE_OBJECTS_ARRAY_PROPERTY_NAME), true);
            GUI.enabled = true;
        }


        private void LinkSaveableObjects(LevelDataManager levelDataManager)
        {
            const string QUERY = "Would you like to display logs of all GameObjects found with ISaveable components?";

            int option = EditorUtility.DisplayDialogComplex("Display Logs?", QUERY, "Yes", "Cancel", "No");
            switch (option)
            {
                case 0: // Yes - Display Additional Logs.
                    levelDataManager.Editor_FindAllSaveableObjects(true);
                    break;
                case 2: // No - Display No Additional Logs (Only count).
                    levelDataManager.Editor_FindAllSaveableObjects(false);
                    break;
            }

            // Mark the object as dirty so that our changes persist after a scene load.
            EditorUtility.SetDirty(levelDataManager);
        }
    }
}
