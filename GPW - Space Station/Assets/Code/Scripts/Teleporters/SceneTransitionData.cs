using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneManagement
{
    [CreateAssetMenu(menuName = "SceneTransitions/SceneTransitionData")]
    public class SceneTransitionData : ScriptableObject
    {
        public SceneIndex TargetSceneIndex;
        public Vector3 StartPosition;
    }
}