using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;

namespace UI.GameOver
{
    public class GameOverUI : MonoBehaviour
    {
        private static GameOverUI s_instance;
        public static GameOverUI Instance => s_instance;


        [SerializeField] private GameObject _container;
        [SerializeField] private GameObject _firstSelectedElement;


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
            Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_firstSelectedElement);

            if (_container.activeSelf)
            {
                // We are already active. Don't proceed.
                return;
            }

            _container.SetActive(true);
            PlayerInput.PreventAllActions();
        }
        public void HideGameOverUI()
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (!_container.activeSelf)
            {
                // We are already hidden. Don't proceed.
                return;
            }

            _container.SetActive(false);
            PlayerInput.RemoveAllActionPrevention();
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