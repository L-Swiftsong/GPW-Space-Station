using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManagement;
using UnityEngine.SceneManagement;

public class PrototypeInitialiser : MonoBehaviour
{
    [SerializeField] private ForegroundSceneTransition _prototypeHubTransition;

    [SerializeField] private SceneField _persistentScene;
    [SerializeField] private SceneField _playerScene;


    private void Awake()
    {
        // Load the persistent scene.
        SceneManager.LoadSceneAsync(_persistentScene, LoadSceneMode.Additive);

        // Load the player scene.
        SceneManager.LoadSceneAsync(_playerScene, LoadSceneMode.Additive);
    }

    private void Start()
    {
        // Request transition to prototype hub (Should automatically unload this scene).
        SceneLoader.Instance.PerformTransition(_prototypeHubTransition);
    }

}
