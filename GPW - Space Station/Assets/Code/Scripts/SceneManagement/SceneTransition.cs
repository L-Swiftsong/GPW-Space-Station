using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneManagement
{
    public abstract class SceneTransition : ScriptableObject
    {
        public SceneField[] ScenesToLoad;
        
        [Space(5)]
        [Min(-1)] public int ActiveSceneIndex = 0;
        public string ActiveScene => ActiveSceneIndex < 0 ? null : ScenesToLoad[ActiveSceneIndex];
    }
}