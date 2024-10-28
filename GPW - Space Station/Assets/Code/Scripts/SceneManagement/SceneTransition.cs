using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Transitions/New Scene Transition", fileName = "NewSceneTransition")]
    public class SceneTransition : ScriptableObject
    {
        [Header("Transition")]
        public SceneField[] ScenesToLoad;
        public SceneField[] ScenesToUnload;

        [Min(-1)] public int ActiveSceneIndex = 0;
        public string ActiveScene => ActiveSceneIndex < 0 ? null : ScenesToLoad[ActiveSceneIndex];

        [Header("Loading Settings")]
        [Tooltip("Set to true if you want the scenes to load and unload in the background (No loading screen).")] public bool LoadInBackground;


        [Header("Positioning Settings")]
        public bool AlterPlayerLocation;
        public Vector3 EntryPosition;
        public Vector3 EntryRotation;
    }
}