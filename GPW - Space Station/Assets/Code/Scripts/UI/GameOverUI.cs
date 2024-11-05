using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
using SceneManagement;

namespace UI.GameOver
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject _container;
        private static GameOverUI s_instance;
        public static GameOverUI Instance => s_instance;


        private const string LOAD_PROTOTYPE_HUB_TRANSITION_PATH = "Transitions/PrototypeHub_Foreground";


        private void Awake()
        {
            s_instance = this;

            _container.SetActive(false);
        }
        private void OnEnable() => SceneLoader.OnReloadFinished += HideGameOverUI;
        private void OnDisable() => SceneLoader.OnReloadFinished -= HideGameOverUI;


        public void ShowGameOverUI()
        {
            _container.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
        public void HideGameOverUI()
        {
            _container.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }


        public void RestartFromCheckpoint() => SceneLoader.Instance.ReloadActiveScenes();
        public void RestartFromPrototypeHub()
        {
            SceneTransition prototypeHubTransition = Resources.Load<SceneTransition>(LOAD_PROTOTYPE_HUB_TRANSITION_PATH);
            SceneLoader.Instance.PerformTransition(prototypeHubTransition);
        }
        public void ExitToDesktop() => Application.Quit();
    }
}