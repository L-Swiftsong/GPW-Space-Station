using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Saving
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            string filePath = "TestSaveData";

            DrawDefaultInspector();

            if (GUILayout.Button("New Game"))
            {
                SaveManager.Instance.NewGame();
            }

            if (GUILayout.Button("Save Game"))
            {
                SaveManager.Instance.SaveGameManual();
            }

            if (GUILayout.Button("Load Game"))
            {
                SaveManager.Instance.LoadGame(filePath);
            }

            if (GUILayout.Button("Delete Game"))
            {
                SaveManager.DeleteGame(filePath);
            }
        }
    }
}
