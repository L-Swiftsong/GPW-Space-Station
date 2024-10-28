using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        #region Singleton

        private static SceneLoader _instance;
        public static SceneLoader Instance
        {
            get => _instance;
            private set
            {
                if (Instance != null)
                {
                    Debug.LogError("Error: A SceneLoader instance already exists: " + Instance.name + ". \nDestroying " + value.name);
                    Destroy(value.gameObject);
                    return;
                }
                
                _instance = value;
            }
        }

        #endregion


        #region Scene Loading

        private List<AsyncOperation> _scenesUnloading = new List<AsyncOperation>();
        private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();

        private const float SCRIPT_INITIALISATION_DELAY = 0.25f;
        private string _activeScene = null;

        private float _previousTimeScale = 1.0f;

        #endregion


        public static event Action OnHardLoadStarted; // Hard Load -> Loading Screen.
        public static event Action OnSoftLoadStarted; // Soft Load -> Load in Background.

        public static event Action OnLoadFinished;


        private void Awake() => Instance = this;


        public void PerformTransition(SceneTransition transition)
        {
            // Cancel current loading.


            // Notify loading start.
            if (transition.LoadInBackground)
            {
                // Soft/Background load.
                OnSoftLoadStarted?.Invoke();
            }
            else
            {
                // Hard/Foreground load.
                OnHardLoadStarted?.Invoke();
            }


            // Save what scene we want to have as our active scene.
            _activeScene = transition.ActiveScene;


            // Determine if we are unloading all active scenes (For error prevention).
            bool unloadAllActiveScenes = SceneManager.loadedSceneCount <= transition.ScenesToUnload.Length;
            if (unloadAllActiveScenes)
            {
                // Load desired scenes.
                for (int i = 0; i < transition.ScenesToLoad.Length; i++)
                {
                    _scenesLoading.Add(SceneManager.LoadSceneAsync(transition.ScenesToLoad[i], (i == 0) ? LoadSceneMode.Single : LoadSceneMode.Additive));
                }
            }
            else
            {
                // Load desired scenes.
                for (int i = 0; i < transition.ScenesToLoad.Length; i++)
                {
                    _scenesLoading.Add(SceneManager.LoadSceneAsync(transition.ScenesToLoad[i], LoadSceneMode.Additive));
                }

                // Unload desired scenes (Done after loading in case we are unloading the only active scene).
                for (int i = 0; i < transition.ScenesToUnload.Length; i++)
                {
                    _scenesUnloading.Add(SceneManager.UnloadSceneAsync(transition.ScenesToUnload[i]));
                }
            }

            StartCoroutine(NotifyWhenScenesAreLoaded(transition));
        }
        private IEnumerator NotifyWhenScenesAreLoaded(SceneTransition transition)
        {
            // Wait until all scenes are loaded.
            yield return new WaitUntil(() => _scenesUnloading.All(t => t.isDone) && _scenesLoading.All(t => t.isDone));

            if (_activeScene != null)
            {
                // Set the active scene.
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_activeScene));
            }

            _previousTimeScale = Time.timeScale;
            if (!transition.LoadInBackground)
            {
                Time.timeScale = 0.0f;
            }


            // Update the player's position.
            if (transition.AlterPlayerLocation)
            {
                PlayerManager.Instance.SetPlayerPositionAndRotation(transition.EntryPosition, transition.EntryRotation);
            }


            // Wait for script inialisation.
            yield return new WaitForSecondsRealtime(SCRIPT_INITIALISATION_DELAY);

            // Once we recieve player input, continue.
            if (transition.LoadInBackground)
            {
                FinishLoading();
            }
            else
            {
#if ENABLE_INPUT_SYSTEM
                InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishLoading());
#elif ENABLE_LEGACY_INPUT_MANAGER
                yield return new WaitUntil(() => Input.anyKeyDown);
                FinishLoading();
#endif
            }
        }



        #region Progress Accessing 

        public float GetSceneLoadProgress()
        {
            if (_scenesLoading == null || _scenesLoading.Count == 0)
            {
                // No scenes are currently loading.
                return -1.0f;
            }
            
            float perScenePercentage = 1.0f / _scenesLoading.Count;
            for (int i = 0; i < _scenesLoading.Count; i++)
            {
                if (_scenesLoading[i].isDone)
                {
                    // This scene has already been fully loaded.
                    continue;
                }

                // The scene has not been fully loaded.
                // Loaded percentage = (Number of loaded scenes * percentage per scene) + current progress scaled.
                return (i * perScenePercentage) + (_scenesLoading[i].progress * perScenePercentage);
            }

            return 1.0f;
        }

        #endregion


        private void FinishLoading()
        {
            OnLoadFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }
    }
}