using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    public struct HardLoadData
    {
        public SceneField[] ScenesToLoad;
        public string ActiveSceneName;
        public SceneField[] ScenesToForceUnload;

        public bool IsHubTransition;

        public bool AlterPlayerLocation;
        public Vector3 EntryPosition;
        public float EntryRotation;


        public HardLoadData(SceneField[] scenesToLoad, string activeSceneName, SceneField[] scenesToForceUnload, bool isHubTransition)
        {
            this.ScenesToLoad = scenesToLoad;
            this.ActiveSceneName = activeSceneName;
            this.ScenesToForceUnload = scenesToForceUnload;

            this.IsHubTransition = isHubTransition;
            
            this.AlterPlayerLocation = false;
            this.EntryPosition = Vector3.zero;
            this.EntryRotation = 0.0f;
        }
        public HardLoadData(SceneField[] scenesToLoad, string activeSceneName, SceneField[] scenesToForceUnload, bool isHubTransition, Vector3 entryPosition, float entryRotation)
        {
            this.ScenesToLoad = scenesToLoad;
            this.ActiveSceneName = activeSceneName;
            this.ScenesToForceUnload = scenesToForceUnload;

            this.IsHubTransition = isHubTransition;

            this.AlterPlayerLocation = true;
            this.EntryPosition = entryPosition;
            this.EntryRotation = entryRotation;
        }
    }
    public struct SoftLoadData
    {
        public SceneField[] ScenesToLoad;
        public string ActiveSceneName;

        public SceneField[] ScenesToUnload;


        public SoftLoadData(SceneField[] scenesToLoad, string activeSceneName, SceneField[] scenesToUnload)
        {
            this.ScenesToLoad = scenesToLoad;
            this.ActiveSceneName = activeSceneName;
            this.ScenesToUnload = scenesToUnload;
        }
    }
}