#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorList
{
    public static void Show(SerializedProperty list, EditorListOptions options = EditorListOptions.Default)
    {
        bool showListSize = options.HasFlag(EditorListOptions.ListSize);
        bool showListLabel = options.HasFlag(EditorListOptions.ListLabel);

        if (showListLabel)
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel++;
        }

        if (!showListLabel || list.isExpanded)
        {
            if (showListSize)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            }
            ShowElements(list, options);
        }

        if (showListLabel)
        {
            EditorGUI.indentLevel--;
        }
    }
    public static void ShowElements(SerializedProperty list, EditorListOptions options)
    {
        bool showElementLabels = options.HasFlag(EditorListOptions.ElementLabels);

        for (int i = 0; i < list.arraySize; i++)
        {
            if (showElementLabels)
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            }
            else
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
            }
        }
    }
}


[System.Serializable]
[System.Flags]
public enum EditorListOptions
{
    None = 1,
    ListSize = 1 << 0,
    ListLabel = 1 << 1,
    ElementLabels = 1 << 2,

    Default = ListSize | ListLabel | ElementLabels,
    NoElementLabels = ListSize | ListLabel,
}
#endif