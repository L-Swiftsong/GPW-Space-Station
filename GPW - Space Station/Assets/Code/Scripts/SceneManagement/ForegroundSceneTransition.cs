using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Transitions/Foreground Scene Transition", fileName = "NewForegroundTransition")]
    public class ForegroundSceneTransition : SceneTransition
    {
        [Header("Positioning Settings")]
        public bool AlterPlayerLocation;
        public Vector3 EntryPosition;
        public float EntryRotation;


        [Header("Special Settings")]
        [SerializeField] private SceneField[] _scenesToForceUnload = null;
        public SceneField[] ScenesToForceUnload => _scenesToForceUnload;


        public HardLoadData GetLoadData()
        {
            return new HardLoadData(
                scenesToLoad: ScenesToLoad, activeSceneName: ActiveSceneName,
                scenesToForceUnload: ScenesToForceUnload,
                EntryPosition, EntryRotation);
        }
    }
}