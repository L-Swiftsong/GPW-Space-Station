#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;


// Credit: 'https://discussions.unity.com/t/load-required-scenes-before-awake-start-of-objects-when-entering-play-mode-from-level-scene/849922/8'.
[InitializeOnLoad]
public class EditorInitialiser
{
    // Persistent Scene Path.
    private const string PERSISTENT_SCENE_PATH = "Assets/Level/Scenes/PersistentScene.unity";

    // Editor Preferences save names.
    private const string ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER = "PreviousScenePath";
    private const string EDITOR_INITIALISED_PREF_IDENTIFIER = "EditorInitialisation";


    // The names of the scenes which we wish to not do the Editor Initialisation from.
    private static List<string> _invalidScenes = new List<string>
    {
        Path.GetFileNameWithoutExtension(PERSISTENT_SCENE_PATH),
        "MainMenuScene",
    };
    // The names of the scenes which we only wish to load the persistent scene from, not the additional scenes.
    private static List<string> _persistentOnly = new List<string>
    {
        "EndCreditsScene",
    };
    // The names of the scenes which we wish to load in addition to the first scene. Loaded in the order they appear in the List.
    private static List<string> _extraScenesToLoad = new List<string>
    {
        "PlayerScene",
    };


    static EditorInitialiser()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    ~EditorInitialiser()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (!ContainsActiveScene(_invalidScenes, out string activeSceneName))
            {
                EditorPrefs.SetString(ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER, activeSceneName);
                EditorPrefs.SetBool(EDITOR_INITIALISED_PREF_IDENTIFIER, true);
                SetStartScene(PERSISTENT_SCENE_PATH);
            }
            else
            {
                SetStartScene(SceneManager.GetActiveScene().path);
            }
        }

        if (state == PlayModeStateChange.EnteredPlayMode && EditorPrefs.GetBool(EDITOR_INITIALISED_PREF_IDENTIFIER))
        {
            // We're entering Play mode haven't already loaded the extra scenes.
            LoadExtraScenes();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorPrefs.SetBool(EDITOR_INITIALISED_PREF_IDENTIFIER, false);
        }
    }

    private static void SetStartScene(string scenePath)
    {
        SceneAsset firstSceneToLoad = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

        if (firstSceneToLoad != null)
        {
            // We found the scene we wished to load. Load it.
            EditorSceneManager.playModeStartScene = firstSceneToLoad;
        }
        else
        {
            // We couldn't find the scene we wished to load.
            Debug.LogErrorFormat("Could not find the first scene to load (Path: {0})", scenePath);
        }
    }

    private static void LoadExtraScenes()
    {
        // Load extra scenes.
        if (!ContainsActiveScene(_persistentOnly, out string _))
        {
            foreach (string scenePath in _extraScenesToLoad)
            {
                SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
            }
        }

        // Load the original scene.
        string originalScene = EditorPrefs.GetString(ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER);
        AsyncOperation async = SceneManager.LoadSceneAsync(originalScene, LoadSceneMode.Additive);

        // Ensure that the original scene is set to the active scene once it has been loaded.
        async.completed += OriginalSceneLoading_Completed;
    }
    private static void OriginalSceneLoading_Completed(AsyncOperation operation)
    {
        operation.completed -= OriginalSceneLoading_Completed;

        string originalScene = EditorPrefs.GetString(ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(originalScene));

        EditorUtils.EditorSpawnPoint.SetPlayerSpawn();
        SceneManagement.SceneLoader.Instance.Editor_EditorInitialiserLoadComplete();
    }


    private static bool ContainsActiveScene(List<string> invalidScenes, out string sceneName)
    {
        sceneName = SceneManager.GetActiveScene().name;
        return invalidScenes.Contains(sceneName);
    }
}
#endif