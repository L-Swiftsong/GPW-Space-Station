using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Source: 'https://github.com/adammyhre/Unity-Inventory-System/blob/master/Assets/_Project/Scripts/Inventory/Editor/SerializableGuidDrawer.cs'.
[CustomPropertyDrawer(typeof(SerializableInstanceGuid))]
public class SerializableInstanceGuidDrawer : PropertyDrawer
{
    private static readonly string[] s_allGuidParts = { "LinkedGameObject", "Part2", "Part3", "Part4" };
    private static readonly string[] s_editableGuidParts = { "Part2", "Part3", "Part4" };

    private static SerializedProperty[] GetEditableGuidParts(SerializedProperty property)
    {
        SerializedProperty[] values = new SerializedProperty[s_editableGuidParts.Length];
        for (int i = 0; i < s_editableGuidParts.Length; ++i)
        {
            values[i] = property.FindPropertyRelative(s_editableGuidParts[i]);
        }

        return values;
    }
    private static SerializedProperty[] GetAllGuidParts(SerializedProperty property)
    {
        SerializedProperty[] values = new SerializedProperty[s_allGuidParts.Length];
        for (int i = 0; i < s_allGuidParts.Length; ++i)
        {
            values[i] = property.FindPropertyRelative(s_allGuidParts[i]);
        }

        return values;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty[] displayGuidProperties = GetAllGuidParts(property);
        if (displayGuidProperties.All(x => x != null))
        {
            if (displayGuidProperties[0].objectReferenceValue != null)
            {
                EditorGUI.LabelField(position, BuildGuidStringForDisplay(displayGuidProperties));
            }
            else
            {
                EditorGUI.SelectableLabel(position, "GUID GameObject Not Initialised");
            }
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
        menu.AddItem(new GUIContent("Reset GUID"), false, () => ResetGuid(property));
        menu.AddItem(new GUIContent("Regenerate GUID"), false, () => RegenerateGuid(property));
        menu.ShowAsContext();
    }

    private void CopyGuid(SerializedProperty property)
    {
        if (GetAllGuidParts(property).Any(x => x == null))
        {
            return;
        }

        string guid = BuildGuidStringForCopying(GetAllGuidParts(property));
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
        foreach (var part in GetEditableGuidParts(property))
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
        SerializedProperty[] guidParts = GetEditableGuidParts(property);

        for (int i = 0; i < s_editableGuidParts.Length; ++i)
        {
            guidParts[i].uintValue = BitConverter.ToUInt32(bytes, (i + 1) * 4);
        }

        property.serializedObject.ApplyModifiedProperties();
        Debug.Log("GUID has been regenerated.");
    }


    private static string BuildGuidStringForCopying(SerializedProperty[] guidParts)
        => new StringBuilder()
            .AppendFormat("{0:X8}", guidParts[0].intValue)
            .AppendFormat("{0:X8}", guidParts[1].uintValue)
            .AppendFormat("{0:X8}", guidParts[2].uintValue)
            .AppendFormat("{0:X8}", guidParts[3].uintValue)
            .ToString();
    private static string BuildGuidStringForDisplay(SerializedProperty[] guidParts)
    {
        if (guidParts[0].objectReferenceValue != null)
        {
            return new StringBuilder()
                .AppendFormat("{0:X8}", UnityEngine.Mathf.Abs(guidParts[0].objectReferenceValue.GetInstanceID()))
                .AppendFormat("{0:X8}", guidParts[1].uintValue)
                .AppendFormat("{0:X8}", guidParts[2].uintValue)
                .AppendFormat("{0:X8}", guidParts[3].uintValue)
                .ToString();
        }
        else
        {
            return "[Instance ID]-" + new StringBuilder()
                .AppendFormat("{0:X8}", guidParts[1].uintValue)
                .AppendFormat("{0:X8}", guidParts[2].uintValue)
                .AppendFormat("{0:X8}", guidParts[3].uintValue)
                .ToString();
        }
    }
}
