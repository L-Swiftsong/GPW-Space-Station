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

        
        [Tooltip("When performing a Foreground Transition, these are the scenes that will not be unloaded")]
            [SerializeField] private List<SceneField> _foregroundTransitionScenesToKeep = new List<SceneField>();

        private const string RELOAD_TO_HUB_TRANSITION_PATH = "Transitions/PrototypeHub_Reload";

        #endregion


        public static event Action OnHardLoadStarted; // Hard Load -> Loading Screen.
        public static event Action OnSoftLoadStarted; // Soft Load -> Load in Background.

        public static event Action OnLoadFinished;
        public static event Action OnHubLoadFinished;

        public static event Action OnReloadToHubFinished;
        public static event Action OnReloadFinished;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            
            Instance = this;
        }


        public void PerformTransition(SceneTransition transition)
        {
            if (transition is ForegroundSceneTransition)
            {
                StartCoroutine(PerformForegroundTransition(transition as ForegroundSceneTransition));
            }
            else
            {
                StartCoroutine(PerformBackgroundTransition(transition as BackgroundSceneTransition));
            }
        }
        public void ResetActiveScenes() => StartCoroutine(ReloadActiveScenes());
        public void ReloadToHub() => StartCoroutine(ResetActiveAndLoadHubScene());

        private IEnumerator PerformForegroundTransition(ForegroundSceneTransition transition)
        {
            // Hard/Foreground load.
            OnHardLoadStarted?.Invoke();


            // Save what scene we want to have as our active scene.
            _activeScene = transition.ActiveScene;


            // Unload all non-persistent scenes.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (_foregroundTransitionScenesToKeep.Any(t => t == SceneManager.GetSceneAt(i).name))
                {
                    continue;
                }

                _scenesUnloading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex));
            }
            yield return new WaitUntil(() => _scenesUnloading.All(t => t.isDone));


            // Load desired scenes.
            for (int i = 0; i < transition.ScenesToLoad.Length; i++)
            {
                _scenesLoading.Add(SceneManager.LoadSceneAsync(transition.ScenesToLoad[i], LoadSceneMode.Additive));
            }
            yield return new WaitUntil(() => _scenesLoading.All(t => t.isDone));


            if (_activeScene != null)
            {
                // Set the active scene.
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_activeScene));
            }


            // Stop time while things load.
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;


            // Wait for script inialisation.
            yield return new WaitForSecondsRealtime(SCRIPT_INITIALISATION_DELAY);


            // Alter player rotation.
            if (transition.AlterPlayerLocation)
            {
                PlayerManager.Instance.SetPlayerPositionAndRotation(transition.EntryPosition, transition.EntryRotation);
            }


            // Once we recieve player input, continue.
#if ENABLE_INPUT_SYSTEM
            if (transition.IsHubTransition)
            {
                InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishHubLoading());
            }
            else
            {
                InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishLoading());
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            yield return new WaitUntil(() => Input.anyKeyDown);

            if (transition.IsHubTransition)
            {
                FinishHubLoading();
            }
            else
            {
                FinishLoading();
            }
#endif
        }
        private IEnumerator PerformBackgroundTransition(BackgroundSceneTransition transition)
        {
            // Notify background load.
            OnSoftLoadStarted?.Invoke();


            // Save what scene we want to have as our active scene.
            _activeScene = transition.ActiveScene;


            // Unload desired scenes.
            for (int i = 0; i < transition.ScenesToUnload.Length; i++)
            {
                _scenesUnloading.Add(SceneManager.UnloadSceneAsync(transition.ScenesToUnload[i]));
            }
            yield return new WaitUntil(() => _scenesUnloading.All(t => t.isDone));


            // Load desired scenes.
            for (int i = 0; i < transition.ScenesToLoad.Length; i++)
            {
                _scenesLoading.Add(SceneManager.LoadSceneAsync(transition.ScenesToLoad[i], LoadSceneMode.Additive));
            }
            yield return new WaitUntil(() => _scenesLoading.All(t => t.isDone));


            if (_activeScene != null)
            {
                // Set the active scene.
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_activeScene));
            }


            // Wait for script inialisation.
            yield return new WaitForSecondsRealtime(SCRIPT_INITIALISATION_DELAY);

            // Finish loading.
            FinishLoading();
        }
        private IEnumerator ReloadActiveScenes()
        {
            // Get the scenes we wish to reload.
            List<int> scenesToReload = new List<int>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == "PersistentScene")
                {
                    // We aren't wanting to reload the Persistent Scene.
                    continue;
                }

                scenesToReload.Add(SceneManager.GetSceneAt(i).buildIndex);
            }

            // Unload scenes.
            for (int i = 0; i < scenesToReload.Count; i++)
            {
                _scenesUnloading.Add(SceneManager.UnloadSceneAsync(scenesToReload[i]));
            }

            yield return new WaitUntil(() => _scenesUnloading.All(t => t.isDone));

            // Load scenes.
            for (int i = 0; i < scenesToReload.Count; i++)
            {
                _scenesLoading.Add(SceneManager.LoadSceneAsync(scenesToReload[i], LoadSceneMode.Additive));
            }

            yield return new WaitUntil(() => _scenesLoading.All(t => t.isDone));


            Debug.Log("Reload Finished");

            // Freeze time.
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;

            // Wait for script inialisation.
            yield return null;

#if ENABLE_INPUT_SYSTEM
            InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishReload());
#elif ENABLE_LEGACY_INPUT_MANAGER
            yield return new WaitUntil(() => Input.anyKeyDown);
            FinishReload();
#endif
        }
        private IEnumerator ResetActiveAndLoadHubScene()
        {
            // Retrieve the 'Reload to Hub' Transition.
            ForegroundSceneTransition hubTransition = Resources.Load<ForegroundSceneTransition>(RELOAD_TO_HUB_TRANSITION_PATH);


            // Save what scene we want to have as our active scene.
            _activeScene = hubTransition.ActiveScene;


            // Unload all scenes except from the persistent scene.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == "PersistentScene")
                {
                    // Don't unload the persistent scene.
                    continue;
                }

                _scenesUnloading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex));
            }
            yield return new WaitUntil(() => _scenesUnloading.All(t => t.isDone));


            // Load desired scenes.
            for (int i = 0; i < hubTransition.ScenesToLoad.Length; i++)
            {
                _scenesLoading.Add(SceneManager.LoadSceneAsync(hubTransition.ScenesToLoad[i], LoadSceneMode.Additive));
            }
            yield return new WaitUntil(() => _scenesLoading.All(t => t.isDone));


            if (_activeScene != null)
            {
                // Set the active scene.
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_activeScene));
            }


            // Stop time while things load.
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;


            // Wait long enough for Awake initialisations.
            yield return null;


            // Alter player position & rotation.
            if (hubTransition.AlterPlayerLocation)
            {
                PlayerManager.Instance.SetPlayerPositionAndRotation(hubTransition.EntryPosition, hubTransition.EntryRotation);
            }

            // Wait for script inialisation.
            yield return new WaitForSecondsRealtime(SCRIPT_INITIALISATION_DELAY);

            // Finish loading to the hub.
            FinishLoadToHub();
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
        private void FinishHubLoading()
        {
            OnLoadFinished?.Invoke();
            OnHubLoadFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }
        private void FinishLoadToHub()
        {
            OnReloadToHubFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }
        private void FinishReload()
        {
            OnReloadFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }
    }
}