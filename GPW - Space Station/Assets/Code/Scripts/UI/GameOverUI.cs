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


        private const string LOAD_PROTOTYPE_HUB_TRANSITION_PATH = "Transitions/PrototypeHub_FT";


        private void Awake()
        {
            s_instance = this;

            _container.SetActive(false);
        }
        private void OnEnable()
        {
            SceneLoader.OnReloadFinished += HideGameOverUI;
            SceneLoader.OnReloadToHubFinished += HideGameOverUI;
        }
        private void OnDisable()
        {
            SceneLoader.OnReloadFinished -= HideGameOverUI;
            SceneLoader.OnReloadToHubFinished -= HideGameOverUI;
        }

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


        public void RestartFromCheckpoint()
        {
            SceneLoader.Instance.ResetActiveScenes();
            HideGameOverUI();
        }
        public void RestartFromPrototypeHub()
        {
            SceneLoader.Instance.ReloadToHub();
            HideGameOverUI();
        }
        public void ExitToDesktop() => Application.Quit();
    }
}