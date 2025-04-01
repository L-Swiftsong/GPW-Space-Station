#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScriptedEvents
{
    [CustomEditor(typeof(Cutscene)), CanEditMultipleObjects]
    public class CutsceneEditor : Editor
    {
        private static GUIContent s_removeButtonContent = new GUIContent("-", "Remove Element");
        private static GUIContent s_addButtonContent = new GUIContent("+", "Add Element");

        private static GUIContent s_moveUpButtonContext = new GUIContent("\u2191", "Add Element");
        private static GUIContent s_moveDownButtonContext = new GUIContent("\u2193", "Add Element");

        private static GUILayoutOption s_miniButtonWidth = GUILayout.Width(20.0f);


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Show(serializedObject.FindProperty("_cutsceneComponents"));

            serializedObject.ApplyModifiedProperties();
        }

        
        public static void Show(SerializedProperty list)
        {
            EditorGUILayout.LabelField(list.displayName, EditorStyles.boldLabel);

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUIContent label = new GUIContent(list.GetArrayElementAtIndex(i).FindPropertyRelative("TriggerTime").floatValue.ToString() + " seconds");
                list.GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.Foldout(list.GetArrayElementAtIndex(i).isExpanded, label);
                if (ShowButtons(list, i))
                {
                    return;
                }

                EditorGUILayout.EndHorizontal();
                
                if (list.GetArrayElementAtIndex(i).isExpanded)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                }
            }


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(s_addButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }
            if (GUILayout.Button(s_removeButtonContent, EditorStyles.miniButton))
            {
                list.arraySize -= 1;
            }
            EditorGUILayout.EndHorizontal();
        }
        private static bool ShowButtons(SerializedProperty list, int index)
        {
            GUI.enabled = index != 0;
            if (GUILayout.Button(s_moveUpButtonContext, EditorStyles.miniButtonLeft, s_miniButtonWidth))
            {
                list.MoveArrayElement(index, index - 1);
                return true;
            }
            GUI.enabled = true;

            GUI.enabled = index != list.arraySize - 1;
            if (GUILayout.Button(s_moveDownButtonContext, EditorStyles.miniButtonRight, s_miniButtonWidth))
            {
                list.MoveArrayElement(index, index + 1);
                return true;
            }
            GUI.enabled = true;

            if (GUILayout.Button(s_addButtonContent, EditorStyles.miniButtonLeft, s_miniButtonWidth))
            {
                list.InsertArrayElementAtIndex(index);
                return true;
            }
            if (GUILayout.Button(s_removeButtonContent, EditorStyles.miniButtonRight, s_miniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);

                // Ensure the element was deleted (Editor weirdness).
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
                return true;
            }
            
            return false;
        }
    }
}
#endif