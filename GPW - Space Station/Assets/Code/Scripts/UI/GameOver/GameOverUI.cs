using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using Saving;

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

        public void ShowGameOverUI()
        {
            Debug.Log("Show Game Over UI");
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
            HideGameOverUI();
            SaveManager.ReloadCheckpointSave();
        }
        public void RestartFromHub()
        {
            HideGameOverUI();
            SaveManager.ReloadHubSave();
        }
        public void ExitToDesktop() => Application.Quit();
    }
}