using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScriptedEvents
{
    //[CustomPropertyDrawer(typeof(CutsceneComponent))]
    public class CutsceneElementDrawer : PropertyDrawer
    {
        private const float FOLDOUT_HEIGHT = 16.0f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = FOLDOUT_HEIGHT;

            if (property.isExpanded)
            {
                height = 4 * FOLDOUT_HEIGHT + 50.0f;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect foldoutRect = new(position.x, position.y, position.width, FOLDOUT_HEIGHT);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            if (property.isExpanded)
            {
                var triggerTimeRect = new Rect(foldoutRect.position.x, foldoutRect.position.y + 20.0f, foldoutRect.width, FOLDOUT_HEIGHT);
                var onTriggeredRect = new Rect(foldoutRect.position.x, foldoutRect.position.y + 40.0f, foldoutRect.width, FOLDOUT_HEIGHT);

                EditorGUI.PropertyField(triggerTimeRect, property.FindPropertyRelative("TriggerTime"), new GUIContent("Trigger Time"));
                EditorGUI.PropertyField(onTriggeredRect, property.FindPropertyRelative("OnTriggered"), new GUIContent("On Triggered"));
            }

            EditorGUI.EndProperty();
        }
    }
}
