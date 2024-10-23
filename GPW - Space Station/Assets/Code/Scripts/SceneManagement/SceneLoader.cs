using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public static class SceneLoader
    {
        private static List<AsyncOperation> _scenesUnloading = new List<AsyncOperation>();
        private static List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();


        public static event Action OnHardLoadStarted; // Hard Load -> Loading Screen.
        public static event Action OnSoftLoadStarted; // Soft Load -> Load in Background.


        public static void PerformTransition(SceneTransition transition)
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
        }

        /// <summary> Outputs the current progress of the scene loading in terms of a float value between 0.0f &amp; 1.0f representing a percentage between 0% &amp; 100%.</summary>
        /// <returns> True if there are currently scenes loading. False if there are no scenes loading.</returns>
        public static bool TryGetSceneLoadProgress(out float currentSceneProgress, out float totalLoadProgress)
        {
            // Set default values for progress.
            totalLoadProgress = 0.0f;
            currentSceneProgress = 0.0f;

            if (_scenesLoading == null || _scenesLoading.Count < 0)
            {
                // No scenes are being loaded.
                return false;
            }
            
            // Loop through each currently loading scene and total their progress.
            float perScenePercentage = 1.0f / _scenesLoading.Count;
            for (int i = 0; i < _scenesLoading.Count; i++)
            {
                if (_scenesLoading[i].isDone)
                {
                    // This scene has already been fully loaded.
                    totalLoadProgress += perScenePercentage;
                    continue;
                }

                // This scene has not been fully loaded.
                currentSceneProgress = _scenesLoading[i].progress;
                return true;
            }

            return true;
        }
        public static float GetSceneLoadProgress()
        {
            if (_scenesLoading == null || _scenesLoading.Count == 0)
            {
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
    }
}