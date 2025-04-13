using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Source: 'https://github.com/adammyhre/Unity-Inventory-System/blob/master/Assets/_Project/Scripts/Inventory/Editor/SerializableGuidDrawer.cs'.
[CustomPropertyDrawer(typeof(SerializableGuid))]
public class SerializableGuidDrawer : PropertyDrawer
{
    private static readonly string[] s_guidParts = { "Part1", "Part2", "Part3", "Part4" };

    private static SerializedProperty[] GetGuidParts(SerializedProperty property)
    {
        SerializedProperty[] values = new SerializedProperty[s_guidParts.Length];
        for (int i = 0; i < s_guidParts.Length; ++i)
        {
            values[i] = property.FindPropertyRelative(s_guidParts[i]);
        }

        return values;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (GetGuidParts(property).All(x => x != null))
        {
            EditorGUI.LabelField(position, BuildGuidString(GetGuidParts(property)));
        }
        else
        {
            EditorGUI.SelectableLabel(position, "GUID Not Initialised");
        }

        bool hasClicked = Event.current.type == EventType.MouseUp && Event.current.button == 1;
        if (hasClicked && position.Contains(Event.current.mousePosition))
        {
            ShowContextMenu(property);
            Event.current.Use();
        }

        EditorGUI.EndProperty();
    }


    private void ShowContextMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Copy GUID"), false, () => CopyGuid(property));
        menu.AddItem(new GUIContent("Copy Int GUID"), false, () => CopyIntGuid(property));
        menu.AddItem(new GUIContent("Reset GUID"), false, () => ResetGuid(property));
        menu.AddItem(new GUIContent("Regenerate GUID"), false, () => RegenerateGuid(property));
        menu.ShowAsContext();
    }

    private void CopyGuid(SerializedProperty property)
    {
        if (GetGuidParts(property).Any(x => x == null))
        {
            return;
        }

        string guid = BuildGuidString(GetGuidParts(property));
        EditorGUIUtility.systemCopyBuffer = guid;
        Debug.Log($"GUID copied to clipboard: {guid}");
    }
    private void CopyIntGuid(SerializedProperty property)
    {
        if (GetGuidParts(property).Any(x => x == null))
        {
            return;
        }

        SerializedProperty[] guidParts = GetGuidParts(property);
        string guid = string.Join('-', guidParts[0].uintValue, guidParts[1].uintValue, guidParts[2].uintValue, guidParts[3].uintValue);;
        EditorGUIUtility.systemCopyBuffer = guid;
        Debug.Log($"GUID copied to clipboard: {guid}");
    }
    private void ResetGuid(SerializedProperty property)
    {
        const string WARNING = "Are you sure you want to reset the GUID?";
        if (!EditorUtility.DisplayDialog("Reset GUID?", WARNING, "Yes", "No"))
        {
            // Cancel was chosen.
            return;
        }

        // Reset the GUID.
        foreach (var part in GetGuidParts(property))
        {
            part.uintValue = 0;
        }

        property.serializedObject.ApplyModifiedProperties();
        Debug.Log("GUID has been reset.");
    }
    private void RegenerateGuid(SerializedProperty property)
    {
        const string WARNING = "Are you sure you want to regenerate the GUID?";
        if (!EditorUtility.DisplayDialog("Reset GUID?", WARNING, "Yes", "No"))
        {
            // Cancel was chosen.
            return;
        }

        // Regenerate the GUID.

        byte[] bytes = Guid.NewGuid().ToByteArray();
        SerializedProperty[] guidParts = GetGuidParts(property);

        for(int i = 0; i < s_guidParts.Length; ++i)
        {
            guidParts[i].uintValue = BitConverter.ToUInt32(bytes, i * 4);
        }

        property.serializedObject.ApplyModifiedProperties();
        Debug.Log("GUID has been regenerated.");
    }


    private static string BuildGuidString(SerializedProperty[] guidParts)
        => new StringBuilder()
            .AppendFormat("{0:X8}", guidParts[0].uintValue)
            .AppendFormat("{0:X8}", guidParts[1].uintValue)
            .AppendFormat("{0:X8}", guidParts[2].uintValue)
            .AppendFormat("{0:X8}", guidParts[3].uintValue)
            .ToString();
}
