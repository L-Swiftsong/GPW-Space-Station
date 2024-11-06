using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Transitions/Background Scene Transition", fileName = "NewBackgroundTransition")]
    public class BackgroundSceneTransition : SceneTransition
    {
        [Header("Transition")]
        public SceneField[] ScenesToUnload;
    }
}