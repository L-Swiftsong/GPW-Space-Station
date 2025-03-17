using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneManagement
{
    public abstract class SceneTransition : ScriptableObject, ISceneTransition
    {
        public SceneField[] ScenesToLoad => _scenesToLoad;
        [SerializeField] private SceneField[] _scenesToLoad;

        [Space(5)]
        [SerializeField, Min(-1)] private int _activeSceneIndex = 0;
        public string ActiveSceneName => _activeSceneIndex < 0 ? null : _scenesToLoad[_activeSceneIndex];
    }
}