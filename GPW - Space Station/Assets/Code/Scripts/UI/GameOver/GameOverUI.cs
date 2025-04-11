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
        [SerializeField] private GameObject[] _initiallyActiveObjects;
        [SerializeField] private GameObject _firstSelectedElement;


        private void Awake()
        {
            s_instance = this;
            SceneLoader.OnHardLoadStarted += HideGameOverUI;
        }
        private void Start() => _container.SetActive(false);
        private void OnDestroy() => SceneLoader.OnHardLoadStarted -= HideGameOverUI;


        public void ShowGameOverUI()
        {
            Debug.Log("Show Game Over UI");
            Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_firstSelectedElement);


            _container.SetActive(true);
            foreach (Transform child in _container.transform)
                child.gameObject.SetActive(false);
            foreach (GameObject initiallyActiveObject in _initiallyActiveObjects)
                initiallyActiveObject.SetActive(true);


            PlayerInput.PreventAllActions(typeof(GameOverUI));
        }
        public void HideGameOverUI()
        {
            if (!_container.activeSelf)
                return;

            Cursor.lockState = CursorLockMode.Locked;

            _container.SetActive(false);
            PlayerInput.RemoveAllActionPrevention(typeof(GameOverUI));
        }
    }
}