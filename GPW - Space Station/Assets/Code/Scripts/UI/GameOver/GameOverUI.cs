using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using Saving;
using UnityEngine.UI;

namespace UI.GameOver
{
    public class GameOverUI : MonoBehaviour
    {
        private static GameOverUI s_instance;
        public static GameOverUI Instance => s_instance;


        [SerializeField] private GameObject _container;
        [SerializeField] private GameObject _firstSelectedElement;


        [Header("Buttons")]
        [SerializeField] private Button _checkpointSaveButton;
        [SerializeField] private Button _hubSaveButton;


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

            UpdateGameOverButtons();

            _container.SetActive(true);
            PlayerInput.PreventAllActions(typeof(GameOverUI));
        }
        public void HideGameOverUI()
        {
            Cursor.lockState = CursorLockMode.Locked;

            _container.SetActive(false);
            PlayerInput.RemoveAllActionPrevention(typeof(GameOverUI));
        }


        private void UpdateGameOverButtons()
        {
            throw new System.NotImplementedException();
            //_checkpointSaveButton.interactable = SaveManager.HasCheckpointSave();
            //_hubSaveButton.interactable = SaveManager.HasHubSave();
        }


        public void ReloadLastSave()
        {
            HideGameOverUI();
            SaveManager.Instance.LoadMostRecentSave();
        }
        public void RestartFromHub()
        {
            HideGameOverUI();
            throw new System.NotImplementedException();
            //SaveManager.ReloadHubSave();
        }
        public void ExitToMainMenu()
        {
            HideGameOverUI();
            SceneLoader.Instance.ReloadToMainMenu();
        }
        public void ExitToDesktop() => Application.Quit();
    }
}