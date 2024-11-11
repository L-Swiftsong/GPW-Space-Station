using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Transitions/Foreground Scene Transition", fileName = "NewForegroundTransition")]
    public class ForegroundSceneTransition : SceneTransition
    {
        public bool IsHubTransition = false;


        [Header("Positioning Settings")]
        public bool AlterPlayerLocation;
        public Vector3 EntryPosition;
        public Vector3 EntryRotation;
    }
}