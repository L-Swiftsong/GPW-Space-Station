using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    ///     A scene transition which should only be used when transitioning into the game from the main menu.
    /// </summary>
    [CreateAssetMenu(menuName = "Scene Transitions/Main Menu Entry Transition", fileName = "NewMainMenuEntryTransition", order = 0)]
    public class MainMenuEntryTransition : ScriptableObject, ISceneTransition
    {
        [SerializeField] private SceneField _playerScene;
        public ForegroundSceneTransition CorrespondingTransition;


        public HardLoadData GetLoadData()
        {
            SceneField[] scenes = new SceneField[CorrespondingTransition.ScenesToLoad.Length + 1];
            scenes[0] = _playerScene;
            for (int i = 0; i < CorrespondingTransition.ScenesToLoad.Length; ++i)
            {
                scenes[i + 1] = CorrespondingTransition.ScenesToLoad[i];
            }


            return new HardLoadData(
                scenesToLoad: scenes, activeSceneName: CorrespondingTransition.ActiveSceneName,
                CorrespondingTransition.IsHubTransition,
                CorrespondingTransition.EntryPosition, CorrespondingTransition.EntryRotation);
        }
    }
}