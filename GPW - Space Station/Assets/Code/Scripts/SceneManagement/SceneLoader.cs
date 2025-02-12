using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using Entities.Player;


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

        private float _previousTimeScale = 1.0f;

        
        [Tooltip("When performing a Foreground Transition, these are the scenes that will not be unloaded")]
            [SerializeField] private List<SceneField> _foregroundTransitionScenesToKeep = new List<SceneField>();

        [SerializeField] private SceneField _mainMenuScene;

        #endregion


        public static event Action OnHardLoadStarted; // Hard Load -> Loading Screen.
        public static event Action OnSoftLoadStarted; // Soft Load -> Load in Background.

        public static event Action OnMainMenuReloadFinished;
        public static event Action OnLoadFinished;
        public static event Action OnHubLoadFinished;


        public static bool s_HasGameStarted = false;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            
            Instance = this;
        }


        private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name != "MainMenuScene" && scene.name != "PersistentScene")
            {
                s_HasGameStarted = true;
            }
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
        
        private IEnumerator PerformForegroundTransition(ForegroundSceneTransition transition)
        {
            // Hard/Foreground load.
            OnHardLoadStarted?.Invoke();


            // Save what scene we want to have as our active scene.
            var activeScene = transition.ActiveScene;


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


            if (activeScene != null)
            {
                // Set the active scene.
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(activeScene));
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
            var activeScene = transition.ActiveScene;


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


            if (activeScene != null)
            {
                // Set the active scene.
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(activeScene));
            }


            // Wait for script inialisation.
            yield return new WaitForSecondsRealtime(SCRIPT_INITIALISATION_DELAY);

            // Finish loading.
            FinishLoading();
        }


        public void ReloadToMainMenu()
        {
            UI.Menus.MainMenuUI.SetEntryFromOtherScene();
            StartCoroutine(PerformLoadFromBuildIndices(new int[1] { _mainMenuScene.BuildIndex }, 0, transitionType: TransitionType.Menu));
        }
        public void LoadFromSave(int[] buildIndices, int activeSceneBuildIndex, System.Action onCompleteCallback) => StartCoroutine(PerformLoadFromBuildIndices(buildIndices, activeSceneBuildIndex, onCompleteCallback));
        
        private enum TransitionType { Default, Hub, Menu };
        private IEnumerator PerformLoadFromBuildIndices(int[] buildIndices, int activeSceneBuildIndex, System.Action onCompleteCallback = null, TransitionType transitionType = TransitionType.Default)
        {
            // Notify listeners that a foreground load has started.
            OnHardLoadStarted?.Invoke();


            // Unload all non-persistent scenes.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == "PersistentScene")
                {
                    continue;
                }

                _scenesUnloading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex));
            }
            yield return new WaitUntil(() => _scenesUnloading.All(t => t.isDone));


            // Load desired scenes.
            for (int i = 0; i < buildIndices.Length; i++)
            {
                _scenesLoading.Add(SceneManager.LoadSceneAsync(buildIndices[i], LoadSceneMode.Additive));
            }
            yield return new WaitUntil(() => _scenesLoading.All(t => t.isDone));

            // Set the active scene.
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(activeSceneBuildIndex));


            // Stop time while things load.
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;


            // Wait for script inialisation.
            yield return new WaitForSecondsRealtime(SCRIPT_INITIALISATION_DELAY);


            onCompleteCallback?.Invoke();


            // Once we recieve player input, continue and perform our 'Finish Loading' function based on the type of transition this is.
#if ENABLE_INPUT_SYSTEM
            switch (transitionType)
            {
                case TransitionType.Menu:
                    InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishMenuReload());
                    break;
                case TransitionType.Hub:
                    InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishHubLoading());
                    break;
                default:
                    InputSystem.onAnyButtonPress.CallOnce(ctrl => FinishLoading());
                    break;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            yield return new WaitUntil(() => Input.anyKeyDown);

            switch (transitionType)
            {
                case TransitionType.Menu:
                    FinishMenuReload();
                    break;
                case TransitionType.Hub:
                    ctrl => FinishHubLoading();
                    break;
                default:
                    ctrl => FinishLoading();
                    break;
            }
#endif
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


        private void FinishLoading() => StartCoroutine(PerformAfterFrame(() => {
            OnLoadFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }));
        private void FinishHubLoading() => StartCoroutine(PerformAfterFrame(() => {
            OnLoadFinished?.Invoke();
            OnHubLoadFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }));
        private void FinishMenuReload() => StartCoroutine(PerformAfterFrame(() => {
            OnMainMenuReloadFinished?.Invoke();
            Time.timeScale = _previousTimeScale;
        }));

        private IEnumerator PerformAfterFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }



        /// <summary> Get an array of the build indices of the currently loaded scenes.</summary>
        /// <param name="ignorePersistents"> If true, we don't include scenes such as the 'PersistentScene' and 'PlayerScene' in the array.</param>
        public static int[] GetLoadedSceneBuildIndices(bool ignorePersistents = true)
        {
            List<int> sceneIndexes = new List<int>();
            for (int i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (ignorePersistents && (scene.name == "PersistentScene"))
                {
                    continue;
                }

                sceneIndexes.Add(scene.buildIndex);
            }

            return sceneIndexes.ToArray();
        }
        public static int GetActiveSceneBuildIndex() => SceneManager.GetActiveScene().buildIndex;
    }
}